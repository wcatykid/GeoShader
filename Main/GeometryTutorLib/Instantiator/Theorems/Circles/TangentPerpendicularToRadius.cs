using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TangentPerpendicularToRadius : Theorem
    {
        private readonly static string NAME = "Tangents are Perpendicular To Radii";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TANGENT_IS_PERPENDICULAR_TO_RADIUS);

        public static void Clear()
        {
            candidateIntersections.Clear();
            candidateTangents.Clear();
            candidateStrengthened.Clear();
        }

        private static List<Intersection> candidateIntersections = new List<Intersection>();
        private static List<Tangent> candidateTangents = new List<Tangent>();
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
            annotation.active = EngineUIBridge.JustificationSwitch.TANGENT_IS_PERPENDICULAR_TO_RADIUS;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Tangent)
            {
                Tangent newTangent = clause as Tangent;

                if (!(newTangent.intersection is CircleSegmentIntersection)) return newGrounded;

                foreach (Intersection inter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(newTangent, inter, newTangent));
                }

                candidateTangents.Add(newTangent);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is Tangent)) return newGrounded;

                if (!((newStreng.strengthened as Tangent).intersection is CircleSegmentIntersection)) return newGrounded;

                foreach (Intersection inter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateTheorem(newStreng.strengthened as Tangent, inter, newStreng));
                }

                candidateStrengthened.Add(newStreng);
            }
            else if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;

                foreach (Tangent oldTangent in candidateTangents)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldTangent, newInter, oldTangent));
                }

                foreach (Strengthened oldStreng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldStreng.strengthened as Tangent, newInter, oldStreng));
                }

                candidateIntersections.Add(newInter);
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
        private static List<EdgeAggregator> InstantiateTheorem(Tangent tangent, Intersection inter, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            CircleSegmentIntersection tanInter = tangent.intersection as CircleSegmentIntersection;

            // Does this tangent segment apply to this intersection?
            if (!inter.HasSegment(tanInter.segment)) return newGrounded;

            // Get the radius--if it exists
            Segment radius = null;
            Segment garbage = null;
            tanInter.GetRadii(out radius, out garbage);

            if (radius == null) return newGrounded;

            // Does this radius apply to this intersection?
            if (!inter.HasSubSegment(radius)) return newGrounded;

            Strengthened newPerp = new Strengthened(inter, new Perpendicular(inter));
            
            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, newPerp, annotation));

            return newGrounded;
        }
    }
}