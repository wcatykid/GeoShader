using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleInscribedSemiCircleIsRight : Theorem
    {
        private readonly static string NAME = "An angle inscribed in a semicircle is a right angle.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT);

        public static void Clear()
        {
            candidateInscribed.Clear();
            candidateSemiCircles.Clear();
            candidateStrengthened.Clear();
        }

        private static List<Angle> candidateInscribed = new List<Angle>();
        private static List<Semicircle> candidateSemiCircles = new List<Semicircle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //
        //     C 
        //     |\
        //     | \
        //     |  \
        //     |   O
        //     |    \
        //     |_    \
        //   A |_|____\ B
        //
        // SemiCircle(O, BC), Angle(BAC) -> RightAngle(BAC)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Angle)
            {
                Angle angle = clause as Angle;

                foreach (Semicircle semi in candidateSemiCircles)
                {
                    newGrounded.AddRange(InstantiateTheorem(semi, angle, semi));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateTheorem(streng.strengthened as Semicircle, angle, streng));
                }

                candidateInscribed.Add(angle);
            }
            else if (clause is Semicircle)
            {
                Semicircle semi = clause as Semicircle;

                foreach (Angle angle in candidateInscribed)
                {
                    newGrounded.AddRange(InstantiateTheorem(semi, angle, semi));
                }

                candidateSemiCircles.Add(semi);
            }

            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Semicircle)) return newGrounded;

                foreach (Angle angle in candidateInscribed)
                {
                    newGrounded.AddRange(InstantiateTheorem(streng.strengthened as Semicircle, angle, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        //     C 
        //     |\
        //     | \
        //     |  \
        //     |   O
        //     |    \
        //     |_    \
        //   A |_|____\ B
        //
        // SemiCircle(O, BC), Angle(BAC) -> RightAngle(BAC)
        //
        public static List<EdgeAggregator> InstantiateTheorem(Semicircle semi, Angle angle, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The angle needs to be inscribed in the given semicircle

            // Note: Previously this was checked indirectly by verifying that the angle intercepts a semicircle, but since semicircles now 
            // require 3 points to be defined, it is safer to directly verify that the angle is inscribed in the semicircle.
            // (There may not have been any points defined on the other side of the diameter,
            // meaning there would not actually be any defined semicircles which the angle intercepts). 

            if (!semi.AngleIsInscribed(angle)) return newGrounded;

            Strengthened newRight = new Strengthened(angle, new RightAngle(angle));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(angle);

            newGrounded.Add(new EdgeAggregator(antecedent, newRight, annotation));

            return newGrounded;
        }
    }
}