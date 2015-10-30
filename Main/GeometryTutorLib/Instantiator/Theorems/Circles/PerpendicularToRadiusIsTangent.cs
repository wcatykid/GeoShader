using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularToRadiusIsTangent : Theorem
    {
        private readonly static string NAME = "Segment perpendicular to a radii is a tangent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PERPENDICULAR_TO_RADIUS_IS_TANGENT);

        public static void Clear()
        {
            candidateIntersections.Clear();
            candidatePerpendicular.Clear();
            candidateStrengthened.Clear();
        }

        private static List<CircleSegmentIntersection> candidateIntersections = new List<CircleSegmentIntersection>();
        private static List<Perpendicular> candidatePerpendicular = new List<Perpendicular>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //        )  | B
        //         ) |
        // O        )| S
        //         ) |
        //        )  |
        //       )   | A
        // Tangent(Circle(O, R), Segment(A, B)), Intersection(OS, AB) -> Perpendicular(Segment(A,B), Segment(O, S))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.PERPENDICULAR_TO_RADIUS_IS_TANGENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CircleSegmentIntersection)
            {
                CircleSegmentIntersection newInter = clause as CircleSegmentIntersection;

                foreach (Perpendicular oldPerp in candidatePerpendicular)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, oldPerp, oldPerp));
                }

                foreach (Strengthened oldStreng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, oldStreng.strengthened as Perpendicular, oldStreng));
                }

                candidateIntersections.Add(newInter);
            }
            else if (clause is Perpendicular)
            {
                Perpendicular newPerp = clause as Perpendicular;

                foreach (CircleSegmentIntersection oldInter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, newPerp, newPerp));
                }

                candidatePerpendicular.Add(newPerp);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is Perpendicular)) return newGrounded;

                foreach (CircleSegmentIntersection oldInter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, newStreng.strengthened as Perpendicular, newStreng));
                }

                candidateStrengthened.Add(newStreng);
            }

            return newGrounded;
        }

        //        )  | B
        //         ) |
        // O        )| S
        //         ) |
        //        )  |
        //       )   | A
        // Tangent(Circle(O, R), Segment(A, B)), Intersection(OS, AB) -> Perpendicular(Segment(A,B), Segment(O, S))
        //
        private static List<EdgeAggregator> InstantiateTheorem(CircleSegmentIntersection inter, Perpendicular perp, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The intersection points must be the same.
            if (!inter.intersect.StructurallyEquals(perp.intersect)) return newGrounded;

            // Get the radius - if it exists
            Segment radius = null;
            Segment garbage = null;
            inter.GetRadii(out radius, out garbage);

            if (radius == null) return newGrounded;

            // Two intersections, not a tangent situation.
            if (garbage != null) return newGrounded;

            // The radius can't be the same as the Circ-Inter segment.
            if (inter.segment.HasSubSegment(radius)) return newGrounded;

            // Does this perpendicular apply to this Arc intersection?
            if (!perp.HasSegment(radius) || !perp.HasSegment(inter.segment)) return newGrounded;

            Strengthened newTangent = new Strengthened(inter, new Tangent(inter));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, newTangent, annotation));

            return newGrounded;
        }
    }
}