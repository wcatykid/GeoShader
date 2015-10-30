using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiagonalsOfRhombusBisectRhombusAngles : Theorem
    {
        private readonly static string NAME = "Diagonals Of Rhombi Bisect Rhombus Angles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS);

        //  A __________  B
        //   |          |
        //   |          |
        //   |          |
        // D |__________| C
        //
        // Rhombus(A, B, C, D) -> AngleBisector(Angle(A, B, C), Segment(B, D))
        //                        AngleBisector(Angle(A, D, C), Segment(B, D))
        //                        AngleBisector(Angle(B, A, D), Segment(A, C))
        //                        AngleBisector(Angle(B, C, D), Segment(A, C))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rhombus)
            {
                Rhombus rhombus = clause as Rhombus;

                newGrounded.AddRange(InstantiateToTheorem(rhombus, rhombus));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Rhombus)) return newGrounded;

                newGrounded.AddRange(InstantiateToTheorem(streng.strengthened as Rhombus, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Rhombus rhombus, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            // Instantiate this rhombus diagonals ONLY if the original figure has the diagonals drawn.
            if (rhombus.TopLeftDiagonalIsValid())
            {
                AngleBisector ab1 = new AngleBisector(rhombus.topLeftAngle, rhombus.topLeftBottomRightDiagonal);
                AngleBisector ab2 = new AngleBisector(rhombus.bottomRightAngle, rhombus.topLeftBottomRightDiagonal);
                newGrounded.Add(new EdgeAggregator(antecedent, ab1, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, ab2, annotation));
            }

            if (rhombus.BottomRightDiagonalIsValid())
            {
                AngleBisector ab1 = new AngleBisector(rhombus.topRightAngle, rhombus.bottomLeftTopRightDiagonal);
                AngleBisector ab2 = new AngleBisector(rhombus.bottomLeftAngle, rhombus.bottomLeftTopRightDiagonal);
                newGrounded.Add(new EdgeAggregator(antecedent, ab1, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, ab2, annotation));
            }

            return newGrounded;
        }
    }
}