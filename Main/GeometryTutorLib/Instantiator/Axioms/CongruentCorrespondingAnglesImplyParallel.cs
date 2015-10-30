using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentCorrespondingAnglesImplyParallel : Axiom
    {
        private readonly static string NAME = "Corresponding Congruent Angles Imply Parallel Lines";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL);

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
        // Congruent(Angle(A, N, F), Angle(A, M, D)) -> Parallel(Segment(C, D), Segment(E, F))
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
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL;

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
                        newGrounded.AddRange(CheckAndGenerateCorrespondingAnglesImplyParallel(candIntersection[i], candIntersection[j], conAngles));
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
                        newGrounded.AddRange(CheckAndGenerateCorrespondingAnglesImplyParallel(newIntersection, inter, cas));
                    }
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateCorrespondingAnglesImplyParallel(Intersection inter1, Intersection inter2, CongruentAngles conAngles)
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
            // Check to see if they are, in fact, Corresponding Angles
            //
            Segment parallelCand1 = inter1.OtherSegment(transversal);
            Segment parallelCand2 = inter2.OtherSegment(transversal);

            // The resultant candidate parallel segments shouldn't share any vertices
            if (parallelCand1.SharedVertex(parallelCand2) != null) return newGrounded;

            // Numeric hack to discount any non-parallel segments
            if (!parallelCand1.IsParallelWith(parallelCand2)) return newGrounded;

            // A sanity check that the '4th' point does not lie on the other intersection (thus creating an obvious triangle and thus not parallel lines)
            Point fourthPoint1 = parallelCand1.OtherPoint(inter1.intersect);
            Point fourthPoint2 = parallelCand2.OtherPoint(inter2.intersect);
            if (fourthPoint1 != null && fourthPoint2 != null)
            {
                if (parallelCand1.PointLiesOn(fourthPoint2) || parallelCand2.PointLiesOn(fourthPoint1)) return newGrounded;
            }

            // Both angles should NOT be in the interioir OR the exterioir; a combination is needed.
            bool ang1Interior = angleI.OnInteriorOf(inter1, inter2);
            bool ang2Interior = angleJ.OnInteriorOf(inter1, inter2);
            if (ang1Interior && ang2Interior) return newGrounded;
            if (!ang1Interior && !ang2Interior) return newGrounded;

            //
            // Are these angles on the same side of the transversal?
            //
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
            // Will this crossing segment actually intersect the real transversal in the middle of the two segments It should NOT.
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
            antecedent.Add(conAngles);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallel, annotation));

            return newGrounded;
        }
    }
}