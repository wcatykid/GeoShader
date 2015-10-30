using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RectangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Rectangle";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.RECTANGLE_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateParallelogram.Clear();
            candidateRightAngle.Clear();
            candidateStrengParallelogram.Clear();
            candidateStrengRightAngle.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.RECTANGLE_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallelogram || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateToRectangle(clause));
            }

            if (clause is Rectangle || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromRectangle(clause));
            }

            return newGrounded;
        }

        //  A ________________  B
        //   |                |
        //   |                |
        //   |                |
        // D |________________| C
        //
        // Rectangle(A, B, C, D) -> 4 Right Angles
        //
        private static List<EdgeAggregator> InstantiateFromRectangle(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rectangle)
            {
                Rectangle rectangle  = clause as Rectangle;

                newGrounded.AddRange(InstantiateFromRectangle(rectangle, rectangle));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Rectangle)) return newGrounded;

                newGrounded.AddRange(InstantiateFromRectangle(streng.strengthened as Rectangle, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromRectangle(Rectangle rectangle, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine the parallel opposing sides and output that.
            //
            List<Strengthened> newRightAngles = new List<Strengthened>();
            foreach (Angle rectAngle in rectangle.angles)
            {
                Angle figureAngle = Angle.AcquireFigureAngle(rectAngle);
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

        //  A ________________  B
        //   |                |
        //   |                |
        //   |                |
        // D |________________| C
        //
        // RightAngle(B, A, D), Parallelogram(A, B, C, D) -> Rectangle(A, B, C, D)
        //
        private static List<Parallelogram> candidateParallelogram = new List<Parallelogram>();
        private static List<Strengthened> candidateStrengParallelogram = new List<Strengthened>();
        private static List<RightAngle> candidateRightAngle = new List<RightAngle>();
        private static List<Strengthened> candidateStrengRightAngle = new List<Strengthened>();
        private static List<EdgeAggregator> InstantiateToRectangle(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallelogram)
            {
                Parallelogram newPara = clause as Parallelogram;

                // Don't strengthen a Rectangle to a Rectangle
                if (newPara is Rectangle) return newGrounded;

                foreach (RightAngle rightAngle in candidateRightAngle)
                {
                    newGrounded.AddRange(InstantiateToRectangle(newPara, rightAngle, newPara, rightAngle));
                }

                foreach(Strengthened rightStreng in candidateStrengRightAngle)
                {
                    newGrounded.AddRange(InstantiateToRectangle(newPara, rightStreng.strengthened as RightAngle, newPara, rightStreng));
                }

                candidateParallelogram.Add(newPara);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (streng.strengthened is Parallelogram)
                {
                    // Don't strengthen a Rectangle to a Rectangle
                    if (streng.strengthened is Rectangle || streng.strengthened is Square) return newGrounded;

                    foreach (RightAngle rightAngle in candidateRightAngle)
                    {
                        newGrounded.AddRange(InstantiateToRectangle(streng.strengthened as Parallelogram, rightAngle, streng, rightAngle));
                    }

                    foreach (Strengthened rightStreng in candidateStrengRightAngle)
                    {
                        newGrounded.AddRange(InstantiateToRectangle(streng.strengthened as Parallelogram, rightStreng.strengthened as RightAngle, streng, rightStreng));
                    }

                    candidateStrengParallelogram.Add(streng);
                }
                else if (streng.strengthened is RightAngle)
                {
                    foreach (Parallelogram oldPara in candidateParallelogram)
                    {
                        newGrounded.AddRange(InstantiateToRectangle(oldPara, streng.strengthened as RightAngle, oldPara, streng));
                    }

                    foreach (Strengthened strengPara in candidateStrengParallelogram)
                    {
                        newGrounded.AddRange(InstantiateToRectangle(strengPara.strengthened as Parallelogram, streng.strengthened as RightAngle, strengPara, streng));
                    }

                    candidateStrengRightAngle.Add(streng);
                }
                else return newGrounded;
            }
            else if (clause is RightAngle)
            {
                RightAngle rightAngle = clause as RightAngle;

                foreach (Parallelogram oldPara in candidateParallelogram)
                {
                    newGrounded.AddRange(InstantiateToRectangle(oldPara, rightAngle, oldPara, rightAngle));
                }

                foreach (Strengthened strengPara in candidateStrengParallelogram)
                {
                    newGrounded.AddRange(InstantiateToRectangle(strengPara.strengthened as Parallelogram, rightAngle, strengPara, rightAngle));
                }

                candidateRightAngle.Add(rightAngle);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToRectangle(Parallelogram parallelogram, RightAngle ra, GroundedClause originalPara, GroundedClause originalRightAngle)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this right angle apply to this quadrilateral?
            if (!parallelogram.HasAngle(ra)) return newGrounded;

            //
            // Create the new Rectangle object
            //
            Strengthened newRectangle = new Strengthened(parallelogram, new Rectangle(parallelogram));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(originalPara);
            antecedent.Add(originalRightAngle);

            newGrounded.Add(new EdgeAggregator(antecedent, newRectangle, annotation));

            return newGrounded;
        }
    }
}