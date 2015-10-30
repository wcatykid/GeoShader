using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SSS : CongruentTriangleAxiom
    {
        private readonly static string NAME = "SSS";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SSS);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentSegments> candidateSegments = new List<CongruentSegments>();

        // Resets all saved data.
        public static void Clear()
        {
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
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Segment(A, C), Angle(D, F)),
        //    Congruent(Segment(B, C), Segment(E, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Angle(A, B, C), Angle(D, E, F)),
        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SSS;

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
                        for (int m = 0; m < candidateSegments.Count - 1; m++)
                        {
                            for (int n = m + 1; n < candidateSegments.Count; n++)
                            {
                                newGrounded.AddRange(InstantiateSSS(candidateTriangles[i], candidateTriangles[j], candidateSegments[m], candidateSegments[n], newCss));
                            }
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateSegments.Add(newCss);
            }
            // If this is a new triangle, check for triangles which may be congruent to this new triangle
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                // Check all combinations of triangles to see if they are congruent
                // This congruence must include the new segment congruence
                foreach (Triangle oldTri in candidateTriangles)
                {
                    for (int m = 0; m < candidateSegments.Count - 1; m++)
                    {
                        for (int n = m + 1; n < candidateSegments.Count - 1; n++)
                        {
                            for (int p = n + 1; p < candidateSegments.Count - 2; p++)
                            {
                                newGrounded.AddRange(InstantiateSSS(newTriangle, oldTri, candidateSegments[m], candidateSegments[n], candidateSegments[p]));
                            }
                        }
                    }
                }

                candidateTriangles.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Checks for SSS given the 5 values
        //
        private static List<EdgeAggregator> InstantiateSSS(Triangle tri1, Triangle tri2, CongruentSegments css1, CongruentSegments css2, CongruentSegments css3)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // All congruence pairs must minimally relate the triangles
            //
            if (!css1.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!css2.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!css3.LinksTriangles(tri1, tri2)) return newGrounded;

            Segment seg1Tri1 = tri1.GetSegment(css1);
            Segment seg1Tri2 = tri2.GetSegment(css1);

            Segment seg2Tri1 = tri1.GetSegment(css2);
            Segment seg2Tri2 = tri2.GetSegment(css2);

            Segment seg3Tri1 = tri1.GetSegment(css3);
            Segment seg3Tri2 = tri2.GetSegment(css3);

            //
            // The vertices of both triangles must all be distinct and cover the triangle completely.
            //
            Point vertex1Tri1 = seg1Tri1.SharedVertex(seg2Tri1);
            Point vertex2Tri1 = seg2Tri1.SharedVertex(seg3Tri1);
            Point vertex3Tri1 = seg1Tri1.SharedVertex(seg3Tri1);

            if (vertex1Tri1 == null || vertex2Tri1 == null || vertex3Tri1 == null) return newGrounded;
            if (vertex1Tri1.StructurallyEquals(vertex2Tri1) ||
                vertex1Tri1.StructurallyEquals(vertex3Tri1) ||
                vertex2Tri1.StructurallyEquals(vertex3Tri1)) return newGrounded;

            Point vertex1Tri2 = seg1Tri2.SharedVertex(seg2Tri2);
            Point vertex2Tri2 = seg2Tri2.SharedVertex(seg3Tri2);
            Point vertex3Tri2 = seg1Tri2.SharedVertex(seg3Tri2);

            if (vertex1Tri2 == null || vertex2Tri2 == null || vertex3Tri2 == null) return newGrounded;
            if (vertex1Tri2.StructurallyEquals(vertex2Tri2) ||
                vertex1Tri2.StructurallyEquals(vertex3Tri2) ||
                vertex2Tri2.StructurallyEquals(vertex3Tri2)) return newGrounded;
            
            //
            // Construct the corresponding points between the triangles
            //
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();

            triangleOne.Add(vertex1Tri1);
            triangleTwo.Add(vertex1Tri2);

            triangleOne.Add(vertex2Tri1);
            triangleTwo.Add(vertex2Tri2);

            triangleOne.Add(vertex3Tri1);
            triangleTwo.Add(vertex3Tri2);

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
            antecedent.Add(css3);

            newGrounded.Add(new EdgeAggregator(antecedent, gcts, annotation));

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(gcts, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}