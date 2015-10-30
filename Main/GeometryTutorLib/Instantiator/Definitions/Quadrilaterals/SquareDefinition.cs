using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SquareDefinition : Definition
    {
        private readonly static string NAME = "Definition of Square";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SQUARE_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateRhombus.Clear();
            candidateRightAngle.Clear();
            candidateStrengRhombus.Clear();
            candidateStrengRightAngle.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SQUARE_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rhombus || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateToSquare(clause));
            }

            if (clause is Square || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromSquare(clause));
            }

            return newGrounded;
        }

        //  A __________  B
        //   |          |
        //   |          |
        //   |_         |
        // D |_|________| C
        //
        // Square(A, B, C, D) -> 4 Right Angles
        //
        private static List<EdgeAggregator> InstantiateFromSquare(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Square)
            {
                Square square  = clause as Square;

                newGrounded.AddRange(InstantiateFromSquare(square, square));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Square)) return newGrounded;

                newGrounded.AddRange(InstantiateFromSquare(streng.strengthened as Square, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromSquare(Square square, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine the parallel opposing sides and output that.
            //
            List<Strengthened> newRightAngles = new List<Strengthened>();
            foreach (Angle angle in square.angles)
            {
                Angle figureAngle = Angle.AcquireFigureAngle(angle);
                newRightAngles.Add(new Strengthened(figureAngle, new RightAngle(figureAngle)));
            }

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            
            foreach (Strengthened rightAngle in newRightAngles)
            {
                newGrounded.Add(new EdgeAggregator(antecedent, rightAngle, annotation));
            }

            return newGrounded;
        }

        //  A __________  B
        //   |          |
        //   |          |
        //   |_         |
        // D |_|________| C
        //
        // Rhombus(A, B, C, D), RightAngle(A, D, C) -> Square(A, B, C, D)
        //
        private static List<Rhombus> candidateRhombus = new List<Rhombus>();
        private static List<Strengthened> candidateStrengRhombus = new List<Strengthened>();
        private static List<RightAngle> candidateRightAngle = new List<RightAngle>();
        private static List<Strengthened> candidateStrengRightAngle = new List<Strengthened>();
        private static List<EdgeAggregator> InstantiateToSquare(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rhombus)
            {
                Rhombus newRhom = clause as Rhombus;

                // We don't want to strengthen a Square to a square.
                if (clause is Square) return newGrounded;

                foreach (RightAngle rightAngle in candidateRightAngle)
                {
                    newGrounded.AddRange(InstantiateToSquare(newRhom, rightAngle, newRhom, rightAngle));
                }

                foreach(Strengthened rightStreng in candidateStrengRightAngle)
                {
                    newGrounded.AddRange(InstantiateToSquare(newRhom, rightStreng.strengthened as RightAngle, newRhom, rightStreng));
                }

                candidateRhombus.Add(newRhom);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (streng.strengthened is Rhombus)
                {
                    // We don't want to strengthen a Square to a square.
                    if (streng.strengthened is Square) return newGrounded;

                    foreach (RightAngle rightAngle in candidateRightAngle)
                    {
                        newGrounded.AddRange(InstantiateToSquare(streng.strengthened as Rhombus, rightAngle, streng, rightAngle));
                    }

                    foreach (Strengthened rightStreng in candidateStrengRightAngle)
                    {
                        newGrounded.AddRange(InstantiateToSquare(streng.strengthened as Rhombus, rightStreng.strengthened as RightAngle, streng, rightStreng));
                    }

                    candidateStrengRhombus.Add(streng);
                }
                else if (streng.strengthened is RightAngle)
                {
                    foreach (Rhombus oldRhom in candidateRhombus)
                    {
                        newGrounded.AddRange(InstantiateToSquare(oldRhom, streng.strengthened as RightAngle, oldRhom, streng));
                    }

                    foreach (Strengthened strengRhom in candidateStrengRhombus)
                    {
                        newGrounded.AddRange(InstantiateToSquare(strengRhom.strengthened as Rhombus, streng.strengthened as RightAngle, strengRhom, streng));
                    }

                    candidateStrengRightAngle.Add(streng);
                }
                else return newGrounded;
            }
            else if (clause is RightAngle)
            {
                RightAngle rightAngle = clause as RightAngle;

                foreach (Rhombus oldRhom in candidateRhombus)
                {
                    newGrounded.AddRange(InstantiateToSquare(oldRhom, rightAngle, oldRhom, rightAngle));
                }

                foreach (Strengthened strengRhom in candidateStrengRhombus)
                {
                    newGrounded.AddRange(InstantiateToSquare(strengRhom.strengthened as Rhombus, rightAngle, strengRhom, rightAngle));
                }

                candidateRightAngle.Add(rightAngle);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToSquare(Rhombus rhombus, RightAngle ra, GroundedClause originalRhom, GroundedClause originalRightAngle)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this right angle apply to this quadrilateral?
            if (!rhombus.HasAngle(ra)) return newGrounded;

            //
            // Create the new Square object
            //
            Strengthened newSquare = new Strengthened(rhombus, new Square(rhombus));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(originalRhom);
            antecedent.Add(originalRightAngle);

            newGrounded.Add(new EdgeAggregator(antecedent, newSquare, annotation));

            return newGrounded;
        }
    }
}