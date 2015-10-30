using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentSegmentsImplySegmentRatioDefinition : Axiom
    {
        private readonly static string NAME = "Congruent Segments Imply Proportional Segments";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentSegments> candidateCongruentSegments = new List<CongruentSegments>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateCongruentSegments.Clear();
            candidateTriangles.Clear();
        }

        //
        // Generate Proportional relationships only if those proportions may be used by a figure (in this case, only triangles)
        //
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    
        //    Congruent(Segment(A, B), Segment(B, C)), Congruent(Segment(D, E), Segment(E, F)) -> Similar(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                  Proportional(Segment(A, B), Segment(D, E)),
        //                                                  Proportional(Segment(A, B), Segment(E, F)),
        //                                                  Proportional(Segment(B, C), Segment(D, E)),
        //                                                  Proportional(Segment(B, C), Segment(E, F)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Triangle) && !(c is CongruentSegments)) return newGrounded;

            // If this is a new triangle, check for triangles which may be Similar to this new triangle
            if (c is Triangle)
            {
                Triangle candidateTri = c as Triangle;

                foreach (Triangle oldTriangle in candidateTriangles)
                {
                    for (int m = 0; m < candidateCongruentSegments.Count - 1; m++)
                    {
                        for (int n = m + 1; n < candidateCongruentSegments.Count; n++)
                        {
                            newGrounded.AddRange(IfCongruencesApplyToTrianglesGenerate(candidateTri, oldTriangle, candidateCongruentSegments[m], candidateCongruentSegments[n]));
                        }
                    }
                }

                // Add this triangle to the list of possible clauses to unify later
                candidateTriangles.Add(candidateTri);
            }
            //
            // Two sets of congruent segments can lead to proportionality 
            //
            else if (c is CongruentSegments)
            {
                CongruentSegments newCss = c as CongruentSegments;

                // Avoid reflexive relationships since they will not lead to interesting proportional relationships
                if (newCss.IsReflexive()) return newGrounded;

                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentSegments css in candidateCongruentSegments)
                        {
                            newGrounded.AddRange(IfCongruencesApplyToTrianglesGenerate(candidateTriangles[i], candidateTriangles[j], css, newCss));
                        }
                    }
                }

                candidateCongruentSegments.Add(newCss);
            }

            return newGrounded;
        }

        //
        // 
        //
        private static List<EdgeAggregator> IfCongruencesApplyToTrianglesGenerate(Triangle ct1, Triangle ct2, CongruentSegments css1, CongruentSegments css2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // The congruent relationships need to link the given triangles
            //
            if (!css1.LinksTriangles(ct1, ct2)) return newGrounded;
            if (!css2.LinksTriangles(ct1, ct2)) return newGrounded;

            Segment seg1Tri1 = ct1.GetSegment(css1);
            Segment seg2Tri1 = ct1.GetSegment(css2);

            Segment seg1Tri2 = ct2.GetSegment(css1);
            Segment seg2Tri2 = ct2.GetSegment(css2);

            // Avoid redundant segments, if it arises
            if (seg1Tri1.StructurallyEquals(seg2Tri1)) return newGrounded;
            if (seg1Tri2.StructurallyEquals(seg2Tri2)) return newGrounded;

            //
            // Proportional Segments (we generate only as needed to avoid bloat in the hypergraph (assuming they are used by both triangles)
            // We avoid generating proportions if they are truly congruences.
            //
            List<GroundedClause> propAntecedent = new List<GroundedClause>();
            propAntecedent.Add(css1);
            propAntecedent.Add(css2);

            SegmentRatio ratio1 = new SegmentRatio(seg1Tri1, seg1Tri2);
            SegmentRatio ratio2 = new SegmentRatio(seg2Tri1, seg2Tri2);

            GeometricSegmentRatioEquation newEq = new GeometricSegmentRatioEquation(ratio1, ratio2);
            
            newGrounded.Add(new EdgeAggregator(propAntecedent, newEq, annotation));

            ////
            //// Only generate if ratios are not 1.
            ////



            //GeometricSegmentRatio newProp = null;
            //if (!Utilities.CompareValues(seg1Tri1.Length, seg1Tri2.Length))
            //{
            //    newProp = new GeometricSegmentRatio(seg1Tri1, seg1Tri2);
            //    newGrounded.Add(new EdgeAggregator(propAntecedent, newProp, annotation));
            //}
            //if (!Utilities.CompareValues(seg1Tri1.Length, seg2Tri2.Length))
            //{
            //    newProp = new GeometricSegmentRatio(seg1Tri1, seg2Tri2);
            //    newGrounded.Add(new EdgeAggregator(propAntecedent, newProp, annotation));
            //}
            //if (!Utilities.CompareValues(seg2Tri1.Length, seg1Tri2.Length))
            //{
            //    newProp = new GeometricSegmentRatio(seg2Tri1, seg1Tri2);
            //    newGrounded.Add(new EdgeAggregator(propAntecedent, newProp, annotation));
            //}
            //if (!Utilities.CompareValues(seg2Tri1.Length, seg2Tri2.Length))
            //{
            //    newProp = new GeometricSegmentRatio(seg2Tri1, seg2Tri2);
            //    newGrounded.Add(new EdgeAggregator(propAntecedent, newProp, annotation));
            //}

            return newGrounded;
        }
    }
}