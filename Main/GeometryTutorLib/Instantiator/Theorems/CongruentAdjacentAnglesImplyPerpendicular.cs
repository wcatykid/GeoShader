using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentAdjacentAnglesImplyPerpendicular : Theorem
    {
        private readonly static string NAME = "Congruent Adjacent Angles Imply Perpendicular Segments";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR);

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
        // Congruent(Angle(C, M, B), Angle(D, M, B)) -> Perpendicular(Segment(A, B), Segment(C, D))
        //
        //                                            B
        //                                           /
        //                              C-----------/-----------D
        //                                         / M
        //                                        /
        //                                       A
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (c is CongruentAngles)
            {
                CongruentAngles conAngles = c as CongruentAngles;

                // We are interested in adjacent angles, not reflexive
                if (conAngles.IsReflexive()) return newGrounded;

                // Any candidates congruent angles need to be adjacent to each other.
                if (conAngles.AreAdjacent() == null) return newGrounded;

                // Find two candidate lines cut by the same transversal
                foreach (Intersection inter in candIntersection)
                {
                    newGrounded.AddRange(CheckAndGenerateCongruentAdjacentImplyPerpendicular(inter, conAngles));
                }

                candAngles.Add(conAngles);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                if (!newIntersection.IsStraightAngleIntersection()) return newGrounded;

                foreach (CongruentAngles cas in candAngles)
                {
                    newGrounded.AddRange(CheckAndGenerateCongruentAdjacentImplyPerpendicular(newIntersection, cas));
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateCongruentAdjacentImplyPerpendicular(Intersection intersection, CongruentAngles conAngles)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The given angles must belong to the intersection. That is, the vertex must align and all rays must overlay the intersection.
            if (!intersection.InducesBothAngles(conAngles)) return newGrounded;

            //
            // Now we have perpendicular scenario
            //
            Strengthened streng = new Strengthened(intersection, new Perpendicular(intersection));

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(intersection);
            antecedent.Add(conAngles);

            newGrounded.Add(new EdgeAggregator(antecedent, streng, annotation));

            return newGrounded;
        }
    }
}