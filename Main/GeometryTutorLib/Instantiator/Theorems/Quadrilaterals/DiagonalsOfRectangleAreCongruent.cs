using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiagonalsOfRectangleAreCongruent : Theorem
    {
        private readonly static string NAME = "Diagonals Of Rectangles are Congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT);

        //  A ________________  B
        //   |                |
        //   |                |
        //   |                |
        // D |________________| C
        //
        // Rectangle(A, B, C, D) -> Congruent(Segment(A, C), Segment(B, D))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rectangle)
            {
                Rectangle rectangle = clause as Rectangle;

                newGrounded.AddRange(InstantiateToTheorem(rectangle, rectangle));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Rectangle)) return newGrounded;

                newGrounded.AddRange(InstantiateToTheorem(streng.strengthened as Rectangle, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Rectangle rectangle, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //Instantiate this rectangle ONLY if the original figure has the rectangle diagonals drawn.
            if (rectangle.diagonalIntersection == null) return newGrounded;

            // Determine the CongruentSegments opposing sides and output that.
            GeometricCongruentSegments gcs = new GeometricCongruentSegments(rectangle.topLeftBottomRightDiagonal,
                                                                            rectangle.bottomLeftTopRightDiagonal);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gcs, annotation));

            return newGrounded;
        }
    }
}