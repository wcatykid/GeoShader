using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularImplyCongruentAdjacentAngles : Theorem
    {
        private readonly static string NAME = "Perpendicular Segments Imply Congruent Adjacent Angles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES);

        private static List<Perpendicular> candPerpendicular = new List<Perpendicular>();
        private static List<Angle> candAngles = new List<Angle>();

        // Resets all saved data.
        public static void Clear()
        {
            candPerpendicular.Clear();
            candAngles.Clear();
        }

        //
        // Perpendicular(Segment(A, B), Segment(C, D)), Angle(A, M, D), Angle(D, M, B) -> Congruent(Angle(A, M, D), Angle(D, M, B)) 
        //
        //                                            B
        //                                           /
        //                              C-----------/-----------D
        //                                         / M
        //                                        /
        //                                       A
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Angle) && !(c is Perpendicular)) return newGrounded;

            if (c is Angle)
            {
                Angle newAngle = c as Angle;

                // Find two candidate lines cut by the same transversal
                foreach (Perpendicular perp in candPerpendicular)
                {
                    foreach (Angle oldAngle in candAngles)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicularImplyCongruentAdjacent(perp, oldAngle, newAngle));
                    }
                }

                candAngles.Add(newAngle);
            }
            else if (c is Perpendicular)
            {
                Perpendicular newPerpendicular = c as Perpendicular;

                // Avoid generating if the situation is:
                //
                //   |
                //   |
                //   |_
                //   |_|_________
                //
                if (newPerpendicular.StandsOnEndpoint()) return newGrounded;

                for (int i = 0; i < candAngles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candAngles.Count; j++)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicularImplyCongruentAdjacent(newPerpendicular, candAngles[i], candAngles[j]));
                    }
                }

                candPerpendicular.Add(newPerpendicular);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGeneratePerpendicularImplyCongruentAdjacent(Perpendicular perp, Angle angle1, Angle angle2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!Utilities.CompareValues(angle1.measure, angle2.measure)) return newGrounded;

            // The given angles must belong to the intersection. That is, the vertex must align and all rays must overlay the intersection.
            if (!(perp.InducesNonStraightAngle(angle1) && perp.InducesNonStraightAngle(angle1))) return newGrounded;

            //
            // Now we have perpendicular -> congruent angles scenario
            //
            GeometricCongruentAngles gcas = new GeometricCongruentAngles(angle1, angle2);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(perp);
            antecedent.Add(angle1);
            antecedent.Add(angle2);

            newGrounded.Add(new EdgeAggregator(antecedent, gcas, annotation));

            return newGrounded;
        }
    }
}