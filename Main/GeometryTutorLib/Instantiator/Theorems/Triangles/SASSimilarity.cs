using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SASSimilarity : Theorem
    {
        private readonly static string NAME = "SAS Similarity";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SAS_SIMILARITY);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentAngles> candidateCongruentAngles = new List<CongruentAngles>();
        private static List<SegmentRatioEquation> candidatePropSegmentEquations = new List<SegmentRatioEquation>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateCongruentAngles.Clear();
            candidatePropSegmentEquations.Clear();
            candidateTriangles.Clear();
        }

        //
        // In order for two triangles to be Similar, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Proportional(Segment(A, B), Segment(D, E)),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Proportional(Segment(B, C), Segment(E, F)) -> Similar(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                  Proportional(Segment(A, C), Angle(D, F)),
        //                                                  Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                                  Congruent(Angle(B, C, A), Angle(E, F, D))
        //
        // Note: we need to figure out the proper order of the sides to guarantee similarity
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SAS_SIMILARITY;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If this is a new segment, check for Similar triangles with this new piece of information
            if (clause is SegmentRatioEquation)
            {
                SegmentRatioEquation newProp = clause as SegmentRatioEquation;

                // Check all combinations of triangles to see if they are Similar
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentAngles cas in candidateCongruentAngles)
                        {
                            newGrounded.AddRange(CollectAndCheckSAS(candidateTriangles[i], candidateTriangles[j], cas, newProp));
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidatePropSegmentEquations.Add(newProp);
            }
            else if (clause is CongruentAngles)
            {
                CongruentAngles newCas = clause as CongruentAngles;

                // Check all combinations of triangles to see if they are Similar
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (SegmentRatioEquation eq in candidatePropSegmentEquations)
                        {
                            newGrounded.AddRange(CollectAndCheckSAS(candidateTriangles[i], candidateTriangles[j], newCas, eq));
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateCongruentAngles.Add(newCas);
            }

            // If this is a new triangle, check for triangles which may be Similar to this new triangle
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                foreach (Triangle oldTriangle in candidateTriangles)
                {
                    foreach (CongruentAngles cas in candidateCongruentAngles)
                    {
                        foreach (SegmentRatioEquation eq in candidatePropSegmentEquations)
                        {
                            newGrounded.AddRange(CollectAndCheckSAS(newTriangle, oldTriangle, cas, eq));
                        }
                    }
                }

                // Add this triangle to the list of possible clauses to unify later
                candidateTriangles.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // 
        //
        private static List<EdgeAggregator> CollectAndCheckSAS(Triangle ct1, Triangle ct2, CongruentAngles cas, SegmentRatioEquation sre)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Proportions must actually equate
            //if (!pss1.ProportionallyEquals(pss2)) return newGrounded;

            //// The smaller and larger segments of the proportionality must be distinct, respectively.
            //if (!pss1.IsDistinctFrom(pss2)) return newGrounded;

            // The proportional relationships need to link the given triangles
            if (!cas.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!sre.LinksTriangles(ct1, ct2)) return newGrounded;
            //if (!pss1.LinksTriangles(ct1, ct2)) return newGrounded;
            //if (!pss2.LinksTriangles(ct1, ct2)) return newGrounded;

            // The smaller segments must belong to one triangle, same for larger segments.
            //if (!(ct1.HasSegment(pss1.smallerSegment) && ct1.HasSegment(pss2.smallerSegment) &&
            //      ct2.HasSegment(pss1.largerSegment) && ct2.HasSegment(pss2.largerSegment)) && 
            //    !(ct2.HasSegment(pss1.smallerSegment) && ct2.HasSegment(pss2.smallerSegment) &&
            //      ct1.HasSegment(pss1.largerSegment) && ct1.HasSegment(pss2.largerSegment)))
            //    return newGrounded;

            KeyValuePair<Segment, Segment> segsTri1 = sre.GetSegments(ct1);
            KeyValuePair<Segment, Segment> segsTri2 = sre.GetSegments(ct2);

            //Segment seg1Tri1 = ct1.GetSegment(pss1);
            //Segment seg2Tri1 = ct1.GetSegment(pss2);

            //Segment seg1Tri2 = ct2.GetSegment(pss1);
            //Segment seg2Tri2 = ct2.GetSegment(pss2);

            // Avoid redundant segments, if they arise
            if (segsTri1.Key.StructurallyEquals(segsTri1.Value)) return newGrounded;
            if (segsTri2.Key.StructurallyEquals(segsTri2.Value)) return newGrounded;
            //if (seg1Tri1.StructurallyEquals(seg2Tri1)) return newGrounded;
            //if (seg1Tri2.StructurallyEquals(seg2Tri2)) return newGrounded;

            Angle angleTri1 = ct1.AngleBelongs(cas);
            Angle angleTri2 = ct2.AngleBelongs(cas);

            // Check both triangles if this is the included angle; if it is, we have SAS
            if (!angleTri1.IsIncludedAngle(segsTri1.Key, segsTri1.Value)) return newGrounded;
            if (!angleTri2.IsIncludedAngle(segsTri2.Key, segsTri2.Value)) return newGrounded;

            //
            // Generate Similar Triangles
            //
            Point vertex1 = angleTri1.GetVertex();
            Point vertex2 = angleTri2.GetVertex();

            // Construct a list of pairs to return; this is the correspondence from triangle 1 to triangle 2
            List<KeyValuePair<Point, Point>> pairs = new List<KeyValuePair<Point, Point>>();

            // The vertices of the angles correspond
            pairs.Add(new KeyValuePair<Point, Point>(vertex1, vertex2));

            // For the segments, look at the congruences and select accordingly
            pairs.Add(new KeyValuePair<Point, Point>(segsTri1.Key.OtherPoint(vertex1), segsTri2.Key.OtherPoint(vertex2)));
            pairs.Add(new KeyValuePair<Point, Point>(segsTri1.Value.OtherPoint(vertex1), segsTri2.Value.OtherPoint(vertex2)));
            
            List<GroundedClause> simTriAntecedent = new List<GroundedClause>();
            simTriAntecedent.Add(ct1);
            simTriAntecedent.Add(ct2);
            simTriAntecedent.Add(cas);
            simTriAntecedent.Add(sre);

            newGrounded.AddRange(GenerateCorrespondingParts(pairs, simTriAntecedent, annotation));

            return newGrounded;
        }

        public static List<GenericInstantiator.EdgeAggregator> GenerateCorrespondingParts(List<KeyValuePair<Point, Point>> pairs, List<GroundedClause> antecedent, Hypergraph.EdgeAnnotation givenAnnotation)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If pairs is populated, we have a Similiarity
            if (!pairs.Any()) return newGrounded;

            // Create the similarity between the triangles
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();
            foreach (KeyValuePair<Point, Point> pair in pairs)
            {
                triangleOne.Add(pair.Key);
                triangleTwo.Add(pair.Value);
            }

            GeometricSimilarTriangles simTris = new GeometricSimilarTriangles(new Triangle(triangleOne), new Triangle(triangleTwo));

            newGrounded.Add(new EdgeAggregator(antecedent, simTris, givenAnnotation));

            // Add all the corresponding parts as new Similar clauses
            newGrounded.AddRange(SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}