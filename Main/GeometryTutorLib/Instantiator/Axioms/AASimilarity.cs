using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AASimilarity : Axiom
    {
        private readonly static string NAME = "AA Similarity";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.AA_SIMILARITY);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentAngles> candidateAngles = new List<CongruentAngles>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateAngles.Clear();
            candidateTriangles.Clear();
        }

        //       A
        //      /\ 
        //     /  \
        //    /    \
        //   /______\
        //  B        C      
        //
        // In order for two triangles to be Similar, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Angle(B, C, A), Angle(E, F, D))
        //    Congruent(Angle(A, B, C), Angle(D, E, F)) -> Similar(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                 Proportional(Segment(A, C), Angle(D, F)),
        //                                                 Proportional(Segment(A, B), Segment(D, E)),
        //                                                 Proportional(Segment(B, C), Segment(E, F))
        //                                                 Congruent(Angle(C, A, B), Angle(F, D, E)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.AA_SIMILARITY;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentAngles)
            {
                CongruentAngles newCas = clause as CongruentAngles;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentAngles oldCas in candidateAngles)
                        {
                            newGrounded.AddRange(InstantiateAASimilarity(candidateTriangles[i], candidateTriangles[j], oldCas, newCas));
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
                    for (int m = 0; m < candidateAngles.Count - 1; m++)
                    {
                        for (int n = m + 1; n < candidateAngles.Count; n++)
                        {
                            newGrounded.AddRange(InstantiateAASimilarity(newTriangle, oldTri, candidateAngles[m], candidateAngles[n]));
                        }
                    }
                }

                candidateTriangles.Add(newTriangle);

            }

            return newGrounded;
        }

        //
        // Checks for AA given the 4 values
        //
        private static List<EdgeAggregator> InstantiateAASimilarity(Triangle tri1, Triangle tri2, CongruentAngles cas1, CongruentAngles cas2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // All congruence pairs must minimally relate the triangles
            //
            if (!cas1.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!cas2.LinksTriangles(tri1, tri2)) return newGrounded;

            // Is this angle an 'extension' of the actual triangle angle? If so, acquire the normalized version of
            // the angle, using only the triangle vertices to represent the angle
            Angle angle1Tri1 = tri1.NormalizeAngle(tri1.AngleBelongs(cas1));
            Angle angle1Tri2 = tri2.NormalizeAngle(tri2.AngleBelongs(cas1));

            Angle angle2Tri1 = tri1.NormalizeAngle(tri1.AngleBelongs(cas2));
            Angle angle2Tri2 = tri2.NormalizeAngle(tri2.AngleBelongs(cas2));

            // The angles for each triangle must be distinct
            if (angle1Tri1.Equals(angle2Tri1) || angle1Tri2.Equals(angle2Tri2)) return newGrounded;

            //
            // Construct the corrsesponding points between the triangles
            //
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();

            triangleOne.Add(angle1Tri1.GetVertex());
            triangleTwo.Add(angle1Tri2.GetVertex());

            triangleOne.Add(angle2Tri1.GetVertex());
            triangleTwo.Add(angle2Tri2.GetVertex());

            // We know the segment endpoint mappings above, now acquire the opposite point
            triangleOne.Add(tri1.OtherPoint(new Segment(angle1Tri1.GetVertex(), angle2Tri1.GetVertex())));
            triangleTwo.Add(tri2.OtherPoint(new Segment(angle1Tri2.GetVertex(), angle2Tri2.GetVertex())));

            //
            // Construct the new clauses: similar triangles and resultant components
            //
            GeometricSimilarTriangles simTris = new GeometricSimilarTriangles(new Triangle(triangleOne), new Triangle(triangleTwo));

            // Hypergraph edge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri1);
            antecedent.Add(tri2);
            antecedent.Add(cas1);
            antecedent.Add(cas2);

            newGrounded.Add(new EdgeAggregator(antecedent, simTris, annotation));

            // Add all the corresponding parts as new Similar clauses
            newGrounded.AddRange(SimilarTriangles.GenerateComponents(simTris, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}