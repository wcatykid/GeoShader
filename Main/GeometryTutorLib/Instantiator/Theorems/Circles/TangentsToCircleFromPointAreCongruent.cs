using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TangentsToCircleFromPointAreCongruent : Theorem
    {
        private readonly static string NAME = "Tangents to a circle from a point are congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT);

        public static void Clear()
        {
            candidateIntersections.Clear();
            candidateTangents.Clear();
            candidateStrengthened.Clear();
        }

        private static List<Intersection> candidateIntersections = new List<Intersection>();
        private static List<Tangent> candidateTangents = new List<Tangent>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //                    \ A
        //                     \
        //                      \
        //   center: O          / P
        //                     /
        //                    / B
        //
        // Tangent(Circle(O), Segment(B, P)),
        // Tangent(Circle(O), Segment(A, P)),
        // Intersection(AP, BP) -> Congruent(Segment(A, P), Segment(P, B))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Tangent)
            {
                Tangent newTangent = clause as Tangent;

                if (!(newTangent.intersection is CircleSegmentIntersection)) return newGrounded;

                foreach (Intersection oldInter in candidateIntersections)
                {
                    foreach (Tangent oldTangent in candidateTangents)
                    {
                        newGrounded.AddRange(InstantiateTheorem(newTangent, oldTangent, oldInter, newTangent, oldTangent));
                    }

                    foreach (Tangent oldTangent in candidateTangents)
                    {
                        newGrounded.AddRange(InstantiateTheorem(newTangent, oldTangent, oldInter, newTangent, oldTangent));
                    }
                }
 
                candidateTangents.Add(newTangent);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is Tangent)) return newGrounded;

                if (!((newStreng.strengthened as Tangent).intersection is CircleSegmentIntersection)) return newGrounded;

                foreach (Intersection oldInter in candidateIntersections)
                {
                    foreach (Tangent oldTangent in candidateTangents)
                    {
                        newGrounded.AddRange(InstantiateTheorem(newStreng.strengthened as Tangent, oldTangent, oldInter, newStreng, oldTangent));
                    }

                    foreach (Strengthened oldStreng in candidateStrengthened)
                    {
                        newGrounded.AddRange(InstantiateTheorem(newStreng.strengthened as Tangent, oldStreng.strengthened as Tangent, oldInter, newStreng, oldStreng));
                    }
                }

                candidateStrengthened.Add(newStreng);
            }
            else if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;

                // Just collect the intersections since they will arrive first.

                candidateIntersections.Add(newInter);
            }

            return newGrounded;
        }

        //                    \ A
        //                     \
        //                      \
        //   center: O          / P
        //                     /
        //                    / B
        //
        // Tangent(Circle(O), Segment(B, P)),
        // Tangent(Circle(O), Segment(A, P)),
        // Intersection(AP, BP) -> Congruent(Segment(A, P), Segment(P, B))
        //
        private static List<EdgeAggregator> InstantiateTheorem(Tangent tangent1, Tangent tangent2, Intersection inter, GroundedClause original1, GroundedClause original2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do the tangents apply to the same circle?
            if (!tangent1.intersection.theCircle.StructurallyEquals(tangent2.intersection.theCircle)) return newGrounded;

            // Do the tangents have components the are part of the third intersection
            if (!inter.HasSegment((tangent1.intersection as CircleSegmentIntersection).segment)) return newGrounded;
            if (!inter.HasSegment((tangent2.intersection as CircleSegmentIntersection).segment)) return newGrounded;

            Segment segment1 = Segment.GetFigureSegment(inter.intersect, tangent1.intersection.intersect);
            Segment segment2 = Segment.GetFigureSegment(inter.intersect, tangent2.intersection.intersect);

            GeometricCongruentSegments gcs = new GeometricCongruentSegments(segment1, segment2);
            
            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original1);
            antecedent.Add(original2);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, gcs, annotation));

            return newGrounded;
        }
    }
}