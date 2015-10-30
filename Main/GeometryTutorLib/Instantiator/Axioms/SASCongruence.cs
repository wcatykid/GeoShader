using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SASCongruence : CongruentTriangleAxiom
    {
        private readonly static string NAME = "SAS";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SAS_CONGRUENCE);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentAngles> candidateAngles = new List<CongruentAngles>();
        private static List<CongruentSegments> candidateSegments = new List<CongruentSegments>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateAngles.Clear();
            candidateSegments.Clear();
            candidateTriangles.Clear();
        }

        //      A             D
        //      /\           / \
        //     /  \         /   \
        //    /    \       /     \
        //   /______\     /_______\
        //  B        C   E         F
        //
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Segment(A, C), Angle(D, F)),
        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SAS_CONGRUENCE;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentSegments)
            {
                CongruentSegments newCss = clause as CongruentSegments;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentSegments oldCss in candidateSegments)
                        {
                            foreach (CongruentAngles cas in candidateAngles)
                            {
                                newGrounded.AddRange(InstantiateSAS(candidateTriangles[i], candidateTriangles[j], oldCss, newCss, cas));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateSegments.Add(newCss);
            }
            else if (clause is CongruentAngles)
            {
                CongruentAngles newCas = clause as CongruentAngles;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        for (int m = 0; m < candidateSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidateSegments.Count; n++) 
                            {
                                newGrounded.AddRange(InstantiateSAS(candidateTriangles[i], candidateTriangles[j], candidateSegments[m], candidateSegments[n], newCas));
                            }
                        }
                    }
                }

                candidateAngles.Add(newCas);
            }
            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                foreach (Triangle oldTri in candidateTriangles)
                {
                    foreach (CongruentAngles oldCas in candidateAngles)
                    {
                        for (int m = 0; m < candidateSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidateSegments.Count; n++)
                            {
                                newGrounded.AddRange(InstantiateSAS(newTriangle, oldTri, candidateSegments[m], candidateSegments[n], oldCas));
                            }
                        }
                    }
                }

                candidateTriangles.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Checks for SAS given the 5 values
        //
        private static List<EdgeAggregator> InstantiateSAS(Triangle tri1, Triangle tri2, CongruentSegments css1, CongruentSegments css2, CongruentAngles cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // All congruence pairs must minimally relate the triangles
            //
            if (!css1.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!css2.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!cas.LinksTriangles(tri1, tri2)) return newGrounded;

            // Is this angle an 'extension' of the actual triangle angle? If so, acquire the normalized version of
            // the angle, using only the triangle vertices to represent the angle
            Angle angleTri1 = tri1.NormalizeAngle(tri1.AngleBelongs(cas));
            Angle angleTri2 = tri2.NormalizeAngle(tri2.AngleBelongs(cas));

            Segment seg1Tri1 = tri1.GetSegment(css1);
            Segment seg1Tri2 = tri2.GetSegment(css1);

            Segment seg2Tri1 = tri1.GetSegment(css2);
            Segment seg2Tri2 = tri2.GetSegment(css2);

            if (!angleTri1.IsIncludedAngle(seg1Tri1, seg2Tri1) || !angleTri2.IsIncludedAngle(seg1Tri2, seg2Tri2)) return newGrounded;

            //
            // Construct the corrsesponding points between the triangles
            //
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();

            triangleOne.Add(angleTri1.GetVertex());
            triangleTwo.Add(angleTri2.GetVertex());

            triangleOne.Add(seg1Tri1.OtherPoint(angleTri1.GetVertex()));
            triangleTwo.Add(seg1Tri2.OtherPoint(angleTri2.GetVertex()));

            triangleOne.Add(seg2Tri1.OtherPoint(angleTri1.GetVertex()));
            triangleTwo.Add(seg2Tri2.OtherPoint(angleTri2.GetVertex()));

            //
            // Construct the new clauses: congruent triangles and CPCTC
            //
            GeometricCongruentTriangles gcts = new GeometricCongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo));

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri1);
            antecedent.Add(tri2);
            antecedent.Add(css1);
            antecedent.Add(css2);
            antecedent.Add(cas);

            newGrounded.Add(new EdgeAggregator(antecedent, gcts, annotation));

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(gcts, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}