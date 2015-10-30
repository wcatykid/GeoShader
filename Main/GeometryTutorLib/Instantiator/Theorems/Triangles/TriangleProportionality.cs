using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{


    public class TriangleProportionality : Theorem
    {
        private readonly static string NAME = "A Line Parallel to One Side of a Triangle and Intersects the Other Two Sides, then it Divides the Sides Proportionally";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRIANGLE_PROPORTIONALITY);

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Triangle> candTriangle = new List<Triangle>();
        private static List<Parallel> candParallel = new List<Parallel>();

        // Resets all saved data.
        public static void Clear()
        {
            candIntersection.Clear();
            candTriangle.Clear();
            candParallel.Clear();
        }

        //
        // Triangle(A, B, C),
        // Intersection(D, Segment(A,B), Segment(D, E)),
        // Intersection(E, Segment(A,C), Segment(D, E)),
        // Parallel(Segment(D, E), Segment(B, C)) -> Proportional(Segment(A, D), Segment(A, B)),
        //                                           Proportional(Segment(A, E), Segment(A, C))
        //            A
        //           /\
        //          /  \
        //         /    \
        //      D /------\ E
        //       /        \
        //    B /__________\ C
        //
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TRIANGLE_PROPORTIONALITY;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Parallel) && !(c is Intersection) && !(c is Triangle)) return newGrounded;

            if (c is Parallel)
            {
                Parallel parallel = c as Parallel;

                foreach (Triangle tri in candTriangle)
                {
                    for (int i = 0; i < candIntersection.Count; i++)
                    {
                        for (int j = i + 1; j < candIntersection.Count; j++)
                        {
                            newGrounded.AddRange(CheckAndGenerateProportionality(tri, candIntersection[i], candIntersection[j], parallel));
                        }
                    }
                }

                candParallel.Add(parallel);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                foreach (Triangle tri in candTriangle)
                {
                    foreach (Intersection inter in candIntersection)
                    {
                        foreach (Parallel parallel in candParallel)
                        {
                            newGrounded.AddRange(CheckAndGenerateProportionality(tri, newIntersection, inter, parallel));
                        }
                    }
                }

                candIntersection.Add(newIntersection);
            }
            else if (c is Triangle)
            {
                Triangle newTriangle = c as Triangle;

                for (int i = 0; i < candIntersection.Count; i++)
                {
                    for (int j = i + 1; j < candIntersection.Count; j++)
                    {
                        foreach (Parallel parallel in candParallel)
                        {
                            newGrounded.AddRange(CheckAndGenerateProportionality(newTriangle, candIntersection[i], candIntersection[j], parallel));
                        }
                    }
                }

                candTriangle.Add(newTriangle);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateProportionality(Triangle tri, Intersection inter1,
                                                                            Intersection inter2, Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The two intersections should not be at the same vertex
            if (inter1.intersect.Equals(inter2.intersect)) return newGrounded;

            //
            // Do these intersections share a segment? That is, do they share the transversal?
            //
            Segment transversal = inter1.AcquireTransversal(inter2);
            if (transversal == null) return newGrounded;

            //
            // Is the transversal a side of the triangle? It should not be.
            //
            if (tri.LiesOn(transversal)) return newGrounded;

            //
            // Determine if one parallel segment is a side of the triangle (which must occur)
            //
            Segment coinciding = tri.DoesParallelCoincideWith(parallel);
            if (coinciding == null) return newGrounded;

            // The transversal and common segment must be distinct
            if (coinciding.IsCollinearWith(transversal)) return newGrounded;

            //
            // Determine if the simplified transversal is within the parallel relationship.
            //
            Segment parallelTransversal = parallel.OtherSegment(coinciding);
            Segment simpleParallelTransversal = new Segment(inter1.intersect, inter2.intersect);

            if (!parallelTransversal.IsCollinearWith(simpleParallelTransversal)) return newGrounded;

            //            A
            //           /\
            //          /  \
            //         /    \
            //  off1  /------\ off2
            //       /        \
            //    B /__________\ C

            //
            // Both intersections should create a T-shape.
            //
            Point off1 = inter1.CreatesTShape();
            Point off2 = inter2.CreatesTShape();
            if (off1 == null || off2 == null) return newGrounded;

            // Get the intersection segments which should coincide with the triangle sides
            KeyValuePair<Segment, Segment> otherSides = tri.OtherSides(coinciding);

            // The intersections may be outside this triangle
            if (otherSides.Key == null || otherSides.Value == null) return newGrounded;

            Segment side1 = inter1.OtherSegment(transversal);
            Segment side2 = inter2.OtherSegment(transversal);

            // Get the actual sides of the triangle
            Segment triangleSide1 = null;
            Segment triangleSide2 = null;
            if (side1.IsCollinearWith(otherSides.Key) && side2.IsCollinearWith(otherSides.Value))
            {
                triangleSide1 = otherSides.Key;
                triangleSide2 = otherSides.Value;
            }
            else if (side1.IsCollinearWith(otherSides.Value) && side2.IsCollinearWith(otherSides.Key))
            {
                triangleSide1 = otherSides.Value;
                triangleSide2 = otherSides.Key;
            }
            else return newGrounded;

            // Verify the opposing parts of the T are on the opposite sides of the triangle
            if (!triangleSide1.PointLiesOnAndExactlyBetweenEndpoints(off2)) return newGrounded;
            if (!triangleSide2.PointLiesOnAndExactlyBetweenEndpoints(off1)) return newGrounded;

            //
            // Construct the new proprtional relationship and resultant equation
            //
            Point sharedVertex = triangleSide1.SharedVertex(triangleSide2);
            SegmentRatio newProp1 = new SegmentRatio(new Segment(sharedVertex, off2), triangleSide1);
            SegmentRatio newProp2 = new SegmentRatio(new Segment(sharedVertex, off1), triangleSide2);

            GeometricSegmentRatioEquation newEq = new GeometricSegmentRatioEquation(newProp1, newProp2);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(inter1);
            antecedent.Add(inter2);
            antecedent.Add(parallel);

            newGrounded.Add(new EdgeAggregator(antecedent, newEq, annotation));

            return newGrounded;
        }
    }
}