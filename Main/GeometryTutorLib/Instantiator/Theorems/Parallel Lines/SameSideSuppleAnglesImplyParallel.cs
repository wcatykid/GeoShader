using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{
    public class SameSideSuppleAnglesImplyParallel : Theorem
    {
        private readonly static string SAME_SIDE_INT_SUPPLE_NAME = "Same-Side Interior Angles Formed by a Transversal Are Supplementary Imply Parallel Lines";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(SAME_SIDE_INT_SUPPLE_NAME, EngineUIBridge.JustificationSwitch.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL);

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Supplementary> candSupps = new List<Supplementary>();

        // Resets all saved data.
        public static void Clear()
        {
            candIntersection.Clear();
            candSupps.Clear();
        }

        //
        // Intersection(M, Segment(A,B), Segment(C, D)),
        // Intersection(N, Segment(A,B), Segment(E, F)),
        // Supplmentary(Angle(F, N, M), Angle(D, M, N)) -> Parallel(Segment(C, D), Segment(E, F))
        //
        //                                            B
        //                                           /
        //                              C-----------/-----------D
        //                                         / M
        //                                        /
        //                             E---------/-----------F
        //                                      / N
        //                                     A
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Supplementary) && !(c is Intersection)) return newGrounded;

            if (c is Supplementary)
            {
                Supplementary supp = c as Supplementary;

                // Find two candidate lines cut by the same transversal
                for (int i = 0; i < candIntersection.Count; i++)
                {
                    for (int j = i + 1; j < candIntersection.Count; j++)
                    {
                        newGrounded.AddRange(CheckAndGenerateSameSideInteriorImplyParallel(candIntersection[i], candIntersection[j], supp));
                    }
                }

                candSupps.Add(supp);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                // Find two candidate lines cut by the same transversal
                foreach (Intersection inter in candIntersection)
                {
                    foreach (Supplementary supp in candSupps)
                    {
                        newGrounded.AddRange(CheckAndGenerateSameSideInteriorImplyParallel(newIntersection, inter, supp));
                    }
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateSameSideInteriorImplyParallel(Intersection inter1, Intersection inter2, Supplementary supp)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Get the transversal (shared segment), if it exists
            Segment transversal = inter1.AcquireTransversal(inter2);
            if (transversal == null) return newGrounded;

            Angle angleI = inter1.GetInducedNonStraightAngle(supp);
            Angle angleJ = inter2.GetInducedNonStraightAngle(supp);

            //
            // Do we have valid intersections and congruent angle pairs
            //
            if (angleI == null || angleJ == null) return newGrounded;

            //
            // Check to see if they are, in fact, alternate interior angles respectively
            //
            // Are the angles within the interior 
            Segment parallelCand1 = inter1.OtherSegment(transversal);
            Segment parallelCand2 = inter2.OtherSegment(transversal);

            if (!angleI.OnInteriorOf(inter1, inter2) || !angleJ.OnInteriorOf(inter1, inter2)) return newGrounded;

            //
            // Are these angles on the opposite side of the transversal?
            //
            // Make a simple transversal from the two intersection points
            Segment simpleTransversal = new Segment(inter1.intersect, inter2.intersect);

            // Find the rays the lie on the transversal
            Segment rayNotOnTransversalI = angleI.OtherRayEquates(simpleTransversal);
            Segment rayNotOnTransversalJ = angleJ.OtherRayEquates(simpleTransversal);

            Point pointNotOnTransversalNorVertexI = rayNotOnTransversalI.OtherPoint(angleI.GetVertex());
            Point pointNotOnTransversalNorVertexJ = rayNotOnTransversalJ.OtherPoint(angleJ.GetVertex());

            // Create a segment from these two points so we can compare distances
            Segment crossing = new Segment(pointNotOnTransversalNorVertexI, pointNotOnTransversalNorVertexJ);

            //
            // Will this crossing segment intersect the real transversal in the middle of the two segments? If it DOES NOT, it is same side
            //
            Point intersection = transversal.FindIntersection(crossing);

            if (Segment.Between(intersection, inter1.intersect, inter2.intersect)) return newGrounded;

            //
            // Now we have an alternate interior scenario
            //
            GeometricParallel newParallel = new GeometricParallel(parallelCand1, parallelCand2);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter1);
            antecedent.Add(inter2);
            antecedent.Add(supp);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallel, annotation));

            return newGrounded;
        }
    }
}