using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularDefinition : Definition
    {
        private readonly static string NAME = "Definition of Perpendicular";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PERPENDICULAR_DEFINITION);

        public static void Clear()
        {
            candidateIntersections.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.PERPENDICULAR_DEFINITION;

            // FROM Perpendicular
            if (clause is Perpendicular) return InstantiateFromPerpendicular(clause, clause as Perpendicular);

            // TO Perpendicular
            if (clause is RightAngle || clause is Intersection) return InstantiateToPerpendicular(clause);

            // Handle Strengthening; may be a Perpendicular (FROM) or Right Angle (TO)

            Strengthened streng = clause as Strengthened;
            if (streng != null)
            {
                if (streng.strengthened is Perpendicular && !(streng.strengthened is PerpendicularBisector))
                {
                    return InstantiateFromPerpendicular(clause, streng.strengthened as Perpendicular);
                }
                else if (streng.strengthened is RightAngle)
                {
                    return InstantiateToPerpendicular(clause);
                }
            }

            return new List<EdgeAggregator>();
        }

        // 
        // Perpendicular(B, Segment(A, B), Segment(B, C)) -> RightAngle(), RightAngle()
        //
        private static List<EdgeAggregator> InstantiateFromPerpendicular(GroundedClause original, Perpendicular perp)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            //          top
            //           |
            //           |_
            //           |_|____ right
            if (perp.StandsOnEndpoint())
            {
                Point top = perp.lhs.OtherPoint(perp.intersect);
                Point right = perp.rhs.OtherPoint(perp.intersect);

                Strengthened streng = new Strengthened(new Angle(top, perp.intersect, right), new RightAngle(top, perp.intersect, right));

                newGrounded.Add(new EdgeAggregator(antecedent, streng, annotation));
            }
            //          top
            //           |
            //           |_
            // left  ____|_|____ right
            else if (perp.StandsOn())
            {
                Point top = perp.lhs.Point1;
                Point center = perp.intersect;
                Point left = perp.rhs.Point1;
                Point right = perp.rhs.Point2;
                if (perp.lhs.PointLiesOnAndExactlyBetweenEndpoints(perp.intersect))
                {
                    left = perp.lhs.Point1;
                    right = perp.lhs.Point2;
                    top = perp.rhs.OtherPoint(perp.intersect);
                }
                else if (perp.rhs.PointLiesOnAndExactlyBetweenEndpoints(perp.intersect))
                {
                    left = perp.rhs.Point1;
                    right = perp.rhs.Point2;
                    top = perp.lhs.OtherPoint(perp.intersect);
                }
                else return newGrounded;

                Strengthened topRight = new Strengthened(new Angle(top, center, right), new RightAngle(top, center, right));
                Strengthened topLeft = new Strengthened(new Angle(top, center, left), new RightAngle(top, center, left));

                newGrounded.Add(new EdgeAggregator(antecedent, topRight, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, topLeft, annotation));
            }
            //          top
            //           |
            //           |_
            // left  ____|_|____ right
            //           |
            //           |
            //         bottom
            else if (perp.Crossing())
            {
                Point top = perp.lhs.Point1;
                Point bottom = perp.lhs.Point2;
                Point center = perp.intersect;
                Point left = perp.rhs.Point1;
                Point right = perp.rhs.Point2;

                Strengthened topRight = new Strengthened(new Angle(top, center, right), new RightAngle(top, center, right));
                Strengthened bottomRight = new Strengthened(new Angle(right, center, bottom), new RightAngle(right, center, bottom));
                Strengthened bottomLeft = new Strengthened(new Angle(left, center, bottom), new RightAngle(left, center, bottom));
                Strengthened topLeft = new Strengthened(new Angle(top, center, left), new RightAngle(top, center, left));

                newGrounded.Add(new EdgeAggregator(antecedent, topRight, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, bottomRight, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, bottomLeft, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, topLeft, annotation));
            }
            else return newGrounded;

            return newGrounded;
        }

        // 
        // RightAngle(A, B, C), Intersection(B, Segment(A, B), SubSegment(B, C)) -> Perpendicular(B, Segment(A, B), Segment(B, C))
        //
        private static List<Intersection> candidateIntersections = new List<Intersection>();
        public static List<EdgeAggregator> InstantiateToPerpendicular(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Intersection)
            {
                // Since we receive intersections right away in the instantiation process, just store the intersections
                candidateIntersections.Add(clause as Intersection);
            }
            else if (clause is RightAngle)
            {
                RightAngle ra = clause as RightAngle;

                foreach (Intersection inter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateToPerpendicular(inter, ra, clause));
                }
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                // Only intrerested in right angles
                if (!(streng.strengthened is RightAngle)) return newGrounded;

                foreach (Intersection inter in candidateIntersections)
                {
                    newGrounded.AddRange(InstantiateToPerpendicular(inter, streng.strengthened as RightAngle, clause));
                }
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToPerpendicular(Intersection inter, RightAngle ra, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // This angle must apply to this intersection (same vertex as well as the segments inducing this angle)
            if (!inter.InducesNonStraightAngle(ra)) return newGrounded;

            // We are strengthening an intersection to a perpendicular 'labeling'
            Strengthened streng = new Strengthened(inter, new Perpendicular(inter));

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, streng, annotation));

            return newGrounded;
        }
    }
}