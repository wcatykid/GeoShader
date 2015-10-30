using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RightAngleDefinition : Definition
    {
        private readonly static string DEF_NAME = "Definition of Right Angle";
        private readonly static string TRANS_NAME = "Transitivity of Congruent Angles With a Right Angle";

        private static Hypergraph.EdgeAnnotation defAnnotation = new Hypergraph.EdgeAnnotation(DEF_NAME, EngineUIBridge.JustificationSwitch.RIGHT_ANGLE_DEFINITION);
        private static Hypergraph.EdgeAnnotation transAnnotation = new Hypergraph.EdgeAnnotation(TRANS_NAME, EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE);

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            defAnnotation.active = EngineUIBridge.JustificationSwitch.RIGHT_ANGLE_DEFINITION;
            transAnnotation.active = EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Strengthened streng = clause as Strengthened;

            // FROM or TO RightAngle as needed
            if (clause is RightAngle || (streng != null && streng.strengthened is RightAngle))
            {
                newGrounded.AddRange(InstantiateFromRightAngle(clause));
                newGrounded.AddRange(InstantiateToRightAngle(clause));
            }

            // TO RightAngle
            if (clause is CongruentAngles || clause is AngleEquation)
            {
                return InstantiateToRightAngle(clause);
            }

            return newGrounded;
        }

        public static List<EdgeAggregator> InstantiateFromRightAngle(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            RightAngle ra = null;

            if (clause is Strengthened) ra = ((clause as Strengthened).strengthened) as RightAngle;
            else if (clause is RightAngle) ra = clause as RightAngle;
            else return newGrounded;

            // Strengthening may be something else
            if (ra == null) return newGrounded;

            GeometricAngleEquation angEq = new GeometricAngleEquation(ra, new NumericValue(90));

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(clause);

            newGrounded.Add(new EdgeAggregator(antecedent, angEq, defAnnotation));

            return newGrounded;
        }

        public static void Clear()
        {
            candidateCongruentAngles.Clear();
            candidateRightAngles.Clear();
            candidateStrengthened.Clear();
        }

        private static List<CongruentAngles> candidateCongruentAngles = new List<CongruentAngles>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<RightAngle> candidateRightAngles = new List<RightAngle>();
        public static List<EdgeAggregator> InstantiateToRightAngle(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is AngleEquation)
            {
                AngleEquation eq = clause as AngleEquation;
                //Filter for acceptable equations - both sides atomic
                int atomicity = eq.GetAtomicity();
                if (atomicity != Equation.BOTH_ATOMIC) return newGrounded;

                //Check that the terms equate an angle to a measure
                List<GroundedClause> lhs = eq.lhs.CollectTerms();
                List<GroundedClause> rhs = eq.rhs.CollectTerms();

                Angle angle = null;
                NumericValue value = null;
                if (lhs[0] is Angle && rhs[0] is NumericValue)
                {
                    angle = lhs[0] as Angle;
                    value = rhs[0] as NumericValue;
                }
                else if (rhs[0] is Angle && lhs[0] is NumericValue)
                {
                    angle = rhs[0] as Angle;
                    value = lhs[0] as NumericValue;
                }
                else
                    return newGrounded;

                //Verify that the angle is a right angle
                if (!Utilities.CompareValues(value.DoubleValue, 90.0)) return newGrounded;

                Strengthened newRightAngle = new Strengthened(angle, new RightAngle(angle));

                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(clause);

                newGrounded.Add(new EdgeAggregator(antecedent, newRightAngle, defAnnotation));

                return newGrounded;

            }
            else if (clause is CongruentAngles)
            {
                CongruentAngles cas = clause as CongruentAngles;

                // Not interested in reflexive relationships in this case
                if (cas.IsReflexive()) return newGrounded;

                foreach (RightAngle ra in candidateRightAngles)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(ra, cas, ra));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(streng.strengthened as RightAngle, cas, streng));
                }

                candidateCongruentAngles.Add(clause as CongruentAngles);
            }
            else if (clause is RightAngle)
            {
                RightAngle ra = clause as RightAngle;

                foreach (CongruentAngles oldCas in candidateCongruentAngles)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(ra, oldCas, ra));
                }

                candidateRightAngles.Add(ra);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                // Only intrerested in right angles
                if (!(streng.strengthened is RightAngle)) return newGrounded;

                foreach (CongruentAngles oldCas in candidateCongruentAngles)
                {
                    newGrounded.AddRange(InstantiateToRightAngle(streng.strengthened as RightAngle, oldCas, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // Implements 'transitivity' with right angles; that is, we may know two angles are congruent and if one is a right angle, the other is well
        //
        // Congruent(Angle(A, B, C), Angle(D, E, F), RightAngle(A, B, C) -> RightAngle(D, E, F)
        //
        public static List<EdgeAggregator> InstantiateToRightAngle(RightAngle ra, CongruentAngles cas, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The congruent must have the given angle in order to generate
            if (!cas.HasAngle(ra)) return newGrounded;

            Angle toBeRight = cas.OtherAngle(ra);
            Strengthened newRightAngle = new Strengthened(toBeRight, new RightAngle(toBeRight));

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);
            antecedent.Add(cas);

            newGrounded.Add(new EdgeAggregator(antecedent, newRightAngle, transAnnotation));

            return newGrounded;
        }
    }
}