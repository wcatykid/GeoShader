using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AnglesOfEqualMeasureAreCongruent : Axiom
    {
        private readonly static string NAME = "Angles of Equal Measure Are Congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT);

        private static List<AngleEquation> candiateAngleEquations = new List<AngleEquation>();

        // Resets all saved data.
        public static void Clear()
        {
            candiateAngleEquations.Clear();
        }

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            AngleEquation newAngEq = clause as AngleEquation;
            if (newAngEq == null) return newGrounded;

            // One side must be atomic
            int atomicity = newAngEq.GetAtomicity();
            if (atomicity != Equation.BOTH_ATOMIC) return newGrounded;

            // Split the information into the angle and its measure
            KeyValuePair<Angle, double> newAngleAndMeasure = ExtractFromEquation(newAngEq);

            // If splitting failed, we are not interested in the equation
            if (newAngleAndMeasure.Key == null) return newGrounded;

            // Can we create any new congruence relationships comparing numeric (deduced angle measure) values?
            foreach (AngleEquation oldEq in candiateAngleEquations)
            {
                KeyValuePair<Angle, double> oldEqAngle = ExtractFromEquation(oldEq);

                // Avoid generating equivalent angles
                if (!newAngleAndMeasure.Key.Equates(oldEqAngle.Key))
                {
                    // Do the angles have the same measure
                    if (Utilities.CompareValues(newAngleAndMeasure.Value, oldEqAngle.Value))
                    {
                        AlgebraicCongruentAngles acas = new AlgebraicCongruentAngles(newAngleAndMeasure.Key, oldEqAngle.Key);

                        // For hypergraph construction
                        List<GroundedClause> antecedent = new List<GroundedClause>();
                        antecedent.Add(newAngEq);
                        antecedent.Add(oldEq);

                        newGrounded.Add(new EdgeAggregator(antecedent, acas, annotation));
                    }
                }
            }

            // Add to the list for future reference
            candiateAngleEquations.Add(newAngEq);

            return newGrounded;
        }

        // Acquire the angle and its measure from the equation
        private static KeyValuePair<Angle, double> ExtractFromEquation(AngleEquation eq)
        {
            NumericValue numeral = null;
            Angle angle = null;

            // Split the sides
            if (eq.lhs is NumericValue)
            {
                numeral = eq.lhs as NumericValue;
                angle = eq.rhs as Angle;
            }
            else if (eq.rhs is NumericValue)
            {
                numeral = eq.rhs as NumericValue;
                angle = eq.lhs as Angle;
            }

            if (numeral == null || angle == null) return new KeyValuePair<Angle, double>(null, 0);

            // The multiplier must be one for: 2mABC = 45, not acceptable; something weird happened anyway
            if (angle.multiplier != 1)
            {
                return new KeyValuePair<Angle, double>(angle, numeral.DoubleValue * (1.0 / angle.multiplier));
            }

            return new KeyValuePair<Angle,double>(angle, numeral.DoubleValue);
        }
    }
}