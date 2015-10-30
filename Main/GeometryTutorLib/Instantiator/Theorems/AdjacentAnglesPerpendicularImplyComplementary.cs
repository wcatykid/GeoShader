using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AdjacentAnglesPerpendicularImplyComplementary : Theorem
    {
        private readonly static string NAME = "If the exterior sides of two adjacent angles are perpendicular, then the angles are complementary.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY);

        public static void Clear()
        {
            candAngles.Clear();
            candPerpendicular.Clear();
        }

        private static List<Perpendicular> candPerpendicular = new List<Perpendicular>();
        private static List<Angle> candAngles = new List<Angle>();

        //
        // Perpendicular(M, Segment(A,B), Segment(C, D)),
        //                Angle(A, B, D), Angle(C, B, D) -> Complementary(Angle(A, B, D), Angle(C, B, D))
        //
        //       A     D
        //       |    /
        //       |   /
        //       |  /
        //       | /
        //       |/_____________________ C
        //       B
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Angle) && !(c is Perpendicular)) return newGrounded;

            if (c is Angle)
            {
                Angle newAngle = c as Angle;

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

            if (!Utilities.CompareValues(angle1.measure + angle2.measure, 90)) return newGrounded;

            // The perpendicular intersection must occur at the same vertex of both angles (we only need check one).
            if (!(angle1.GetVertex().Equals(perp.intersect) && angle2.GetVertex().Equals(perp.intersect))) return newGrounded;

            // Are the angles adjacent?
            Segment sharedRay = angle1.IsAdjacentTo(angle2);
            if (sharedRay == null) return newGrounded;

            // Do the non-shared rays for both angles align with the segments defined by the perpendicular intersection?
            if (!perp.DefinesBothRays(angle1.OtherRayEquates(sharedRay), angle2.OtherRayEquates(sharedRay))) return newGrounded;

            //
            // Now we have perpendicular -> complementary angles scenario
            //
            Complementary cas = new Complementary(angle1, angle2);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(perp);
            antecedent.Add(angle1);
            antecedent.Add(angle2);

            newGrounded.Add(new EdgeAggregator(antecedent, cas, annotation));

            return newGrounded;
        }
    }
}