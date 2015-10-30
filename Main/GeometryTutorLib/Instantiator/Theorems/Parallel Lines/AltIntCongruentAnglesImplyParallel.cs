using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{


    public class AltIntCongruentAnglesImplyParallel : Theorem
    {
        private readonly static string ALT_INT_NAME = "Alternate Interior Angles Formed by a Transversal Imply Parallel Lines";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(ALT_INT_NAME, EngineUIBridge.JustificationSwitch.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL);

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<CongruentAngles> candAngles = new List<CongruentAngles>();

        // Resets all saved data.
        public static void Clear()
        {
            candIntersection.Clear();
            candAngles.Clear();
        }

        //
        // Intersection(M, Segment(A,B), Segment(C, D)),
        // Intersection(N, Segment(A,B), Segment(E, F)),
        // Congruent(Angle(A, N, F), Angle(A, M, C)) -> Parallel(Segment(C, D), Segment(E, F))
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
            annotation.active = EngineUIBridge.JustificationSwitch.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is CongruentAngles) && !(c is Intersection)) return newGrounded;

            if (c is CongruentAngles)
            {
                CongruentAngles conAngles = c as CongruentAngles;

                if (conAngles.IsReflexive()) return newGrounded;

                // Find two candidate lines cut by the same transversal
                for (int i = 0; i < candIntersection.Count - 1; i++)
                {
                    for (int j = i + 1; j < candIntersection.Count; j++)
                    {
                        newGrounded.AddRange(CheckAndGenerateAlternateInteriorImplyParallel(candIntersection[i], candIntersection[j], conAngles));
                    }
                }

                candAngles.Add(conAngles);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                // Find two candidate lines cut by the same transversal
                foreach (Intersection inter in candIntersection)
                {
                    foreach (CongruentAngles cas in candAngles)
                    {
                        newGrounded.AddRange(CheckAndGenerateAlternateInteriorImplyParallel(newIntersection, inter, cas));
                    }
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateAlternateInteriorImplyParallel(Intersection inter1, Intersection inter2, CongruentAngles conAngles)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The two intersections should not be at the same vertex
            if (inter1.intersect.Equals(inter2.intersect)) return newGrounded;

            Segment transversal = inter1.CommonSegment(inter2);

            if (transversal == null) return newGrounded;

            Angle angleI = inter1.GetInducedNonStraightAngle(conAngles);
            Angle angleJ = inter2.GetInducedNonStraightAngle(conAngles);

            //
            // Do we have valid intersections and congruent angle pairs
            //
            if (angleI == null || angleJ == null) return newGrounded;

            //
            // Check to see if they are, in fact, alternate interior angles respectively
            //
            Segment parallelCand1 = inter1.OtherSegment(transversal);
            Segment parallelCand2 = inter2.OtherSegment(transversal);

            // Quick hack check to ensure we have parallel segmenets
            if (!parallelCand1.IsParallelWith(parallelCand2)) return newGrounded;

            // The resultant candidate parallel segments shouldn't share any vertices
            if (parallelCand1.SharedVertex(parallelCand2) != null) return newGrounded;

            // A sanity check that the '4th' point does not lie on the other intersection (thus creating an obvious triangle and thus not parallel lines)
            Point fourthPoint1 = parallelCand1.OtherPoint(inter1.intersect);
            Point fourthPoint2 = parallelCand2.OtherPoint(inter2.intersect);
            if (fourthPoint1 != null && fourthPoint2 != null)
            {
                if (parallelCand1.PointLiesOn(fourthPoint2) || parallelCand2.PointLiesOn(fourthPoint1)) return newGrounded;
            }

            // Are the angles within the interior 
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
            // Will this crossing segment actually intersect the real transversal in the middle of the two segments
            //
            Point intersection = transversal.FindIntersection(crossing);

            if (!Segment.Between(intersection, inter1.intersect, inter2.intersect)) return newGrounded;

            //
            // Now we have an alternate interior scenario
            //
            GeometricParallel newParallel = new GeometricParallel(parallelCand1, parallelCand2);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter1);
            antecedent.Add(inter2);
            antecedent.Add(conAngles);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallel, annotation));

            return newGrounded;
        }
    }
}