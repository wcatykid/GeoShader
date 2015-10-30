using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ASA : CongruentTriangleAxiom
    {
        private readonly static string NAME = "ASA";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ASA);

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

        //       A
        //      /\ 
        //     /  \
        //    /    \
        //   /______\
        //  B        C      
        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Congruent(Segment(B, C), Segment(E, F)),
        //    Congruent(Angle(A, C, B), Angle(D, F, E)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                                 Congruent(Segment(A, B), Angle(D, E)),
        //                                                 Congruent(Segment(A, C), Angle(D, F)),
        //                                                 Congruent(Angle(B, A, C), Angle(E, D, F)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ASA;

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
                        for (int m = 0; m < candidateAngles.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidateAngles.Count; n++)
                            {
                                newGrounded.AddRange(InstantiateASA(candidateTriangles[i], candidateTriangles[j], candidateAngles[m], candidateAngles[n], newCss));
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

                // Except for reflexive congruent triangle (a triangle and itself), reflexive angles cannot lead to congruency
                if (newCas.IsReflexive()) return newGrounded;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentSegments css in candidateSegments)
                        {
                            foreach (CongruentAngles oldCas in candidateAngles)
                            {
                                newGrounded.AddRange(InstantiateASA(candidateTriangles[i], candidateTriangles[j], oldCas, newCas, css));
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
                foreach(Triangle oldTri in candidateTriangles)
                {
                    for (int m = 0; m < candidateAngles.Count - 1; m++)
                    {
                        for (int n = m + 1; n < candidateAngles.Count; n++)
                        {
                            foreach (CongruentSegments css in candidateSegments)
                            {
                                newGrounded.AddRange(InstantiateASA(newTriangle, oldTri, candidateAngles[m], candidateAngles[n], css));
                            }
                        }
                    }
                }

                candidateTriangles.Add(newTriangle);

            }

            return newGrounded;
        }

        //
        // Checks for ASA given the 5 values
        //
        private static List<EdgeAggregator> InstantiateASA(Triangle tri1, Triangle tri2,
                                                                                               CongruentAngles cas1, CongruentAngles cas2, CongruentSegments css)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // All congruence pairs must minimally relate the triangles
            //
            if (!cas1.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!cas2.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!css.LinksTriangles(tri1, tri2)) return newGrounded;

            // Is this angle an 'extension' of the actual triangle angle? If so, acquire the normalized version of
            // the angle, using only the triangle vertices to represent the angle
            Angle angle1Tri1 = tri1.NormalizeAngle(tri1.AngleBelongs(cas1));
            Angle angle1Tri2 = tri2.NormalizeAngle(tri2.AngleBelongs(cas1));

            Angle angle2Tri1 = tri1.NormalizeAngle(tri1.AngleBelongs(cas2));
            Angle angle2Tri2 = tri2.NormalizeAngle(tri2.AngleBelongs(cas2));

            // The angles for each triangle must be distinct
            if (angle1Tri1.Equals(angle2Tri1) || angle1Tri2.Equals(angle2Tri2)) return newGrounded;

            Segment segTri1 = tri1.GetSegment(css);
            Segment segTri2 = tri2.GetSegment(css);

            if (!segTri1.IsIncludedSegment(angle1Tri1, angle2Tri1) || !segTri2.IsIncludedSegment(angle1Tri2, angle2Tri2)) return newGrounded;

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
            triangleOne.Add(tri1.OtherPoint(segTri1));
            triangleTwo.Add(tri2.OtherPoint(segTri2));

            //
            // Construct the new clauses: congruent triangles and CPCTC
            //
            GeometricCongruentTriangles gcts = new GeometricCongruentTriangles(new Triangle(triangleOne), new Triangle(triangleTwo));

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri1);
            antecedent.Add(tri2);
            antecedent.Add(cas1);
            antecedent.Add(cas2);
            antecedent.Add(css);

            newGrounded.Add(new EdgeAggregator(antecedent, gcts, annotation));

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(gcts, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}