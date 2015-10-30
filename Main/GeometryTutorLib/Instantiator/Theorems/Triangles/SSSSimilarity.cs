using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SSSSimilarity : Theorem
    {
        private readonly static string NAME = "SSS Similarity";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SSS_SIMILARITY);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<SegmentRatioEquation> candidateSegmentEquations = new List<SegmentRatioEquation>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateTriangles.Clear();
            candidateSegmentEquations.Clear();
        }

        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    SegmentRatio(Segment(A, B), Segment(D, E)),
        //    SegmentRatio(Segment(A, C), Segment(D, F)),
        //    SegmentRatio(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                  Congruent(Angles(A, B, C), Angle(D, E, F)),
        //                                                  Congruent(Angles(C, A, B), Angle(F, D, E)),
        //                                                  Congruent(Angles(B, C, A), Angle(E, F, D)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee congruence
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SSS_SIMILARITY;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If this is a new segment, check for congruent triangles with this new piece of information
            if (clause is SegmentRatioEquation)
            {
                SegmentRatioEquation newEq = clause as SegmentRatioEquation;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (SegmentRatioEquation oldEq in candidateSegmentEquations)
                        {
                            newGrounded.AddRange(CheckForSSS(candidateTriangles[i], candidateTriangles[j], newEq, oldEq));
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateSegmentEquations.Add(newEq);
            }
            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                foreach (Triangle oldTriangle in candidateTriangles)
                {
                    for (int m = 0; m < candidateSegmentEquations.Count - 1; m++)
                    {
                        for (int n = m + 1; n < candidateSegmentEquations.Count; n++)
                        {
                            newGrounded.AddRange(CheckForSSS(newTriangle, oldTriangle, candidateSegmentEquations[m], candidateSegmentEquations[n]));
                        }
                    }
                }

                // Add this triangle to the list of possible clauses to unify later
                candidateTriangles.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Of all the congruent segment pairs, choose a subset of 3. Exhaustively check all; if they work, return the set.
        //
        private static List<EdgeAggregator> CheckForSSS(Triangle ct1, Triangle ct2, SegmentRatioEquation sre1, SegmentRatioEquation sre2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // The proportional relationships need to link the given triangles
            //
            if (!sre1.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!sre2.LinksTriangles(ct1, ct2)) return newGrounded;

            //
            // Both equations must share a fraction (ratio)
            //
            if (!sre1.SharesRatio(sre2)) return newGrounded;

            //
            // Collect all of the applicable segments
            //
            SegmentRatio shared = sre1.GetSharedRatio(sre2);
            SegmentRatio other1 = sre1.GetOtherRatio(shared);
            SegmentRatio other2 = sre2.GetOtherRatio(shared);

            Segment seg1Tri1 = ct1.GetSegment(shared);
            Segment seg2Tri1 = ct1.GetSegment(other1);
            Segment seg3Tri1 = ct1.GetSegment(other2);

            if (seg1Tri1 == null || seg2Tri1 == null || seg3Tri1 == null) return newGrounded;

            Segment seg1Tri2 = ct2.GetSegment(shared);
            Segment seg2Tri2 = ct2.GetSegment(other1);
            Segment seg3Tri2 = ct2.GetSegment(other2);

            if (seg1Tri2 == null || seg2Tri2 == null || seg3Tri2 == null) return newGrounded;

            // Avoid redundant segments, if they arise
            if (seg1Tri1.StructurallyEquals(seg2Tri1) || seg1Tri1.StructurallyEquals(seg3Tri1) || seg2Tri1.StructurallyEquals(seg3Tri1)) return newGrounded;
            if (seg1Tri2.StructurallyEquals(seg2Tri2) || seg1Tri2.StructurallyEquals(seg3Tri2) || seg2Tri2.StructurallyEquals(seg3Tri2)) return newGrounded;

            //
            // Collect the corresponding points
            //
            List<KeyValuePair<Point, Point>> pointPairs = new List<KeyValuePair<Point, Point>>();
            pointPairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.SharedVertex(seg2Tri1), seg1Tri2.SharedVertex(seg2Tri2)));
            pointPairs.Add(new KeyValuePair<Point, Point>(seg1Tri1.SharedVertex(seg3Tri1), seg1Tri2.SharedVertex(seg3Tri2)));
            pointPairs.Add(new KeyValuePair<Point, Point>(seg2Tri1.SharedVertex(seg3Tri1), seg2Tri2.SharedVertex(seg3Tri2)));

            List<GroundedClause> simTriAntecedent = new List<GroundedClause>();
            simTriAntecedent.Add(ct1);
            simTriAntecedent.Add(ct2);
            simTriAntecedent.Add(sre1);
            simTriAntecedent.Add(sre2);

            newGrounded.AddRange(SASSimilarity.GenerateCorrespondingParts(pointPairs, simTriAntecedent, annotation));

            return newGrounded;
        }
    }
}