using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AngleBisectorTheorem : Theorem
    {
        private readonly static string NAME = "Angle Bisector Theorem";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_THEOREM);

        //
        // AngleBisector(SegmentA, D), Angle(C, A, B)) -> 2 m\angle CAD = m \angle CAB,
        //                                                2 m\angle BAD = m \angle CAB
        //
        //   A ________________________B
        //    |\
        //    | \ 
        //    |  \
        //    |   \
        //    |    \
        //    C     D
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_THEOREM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is AngleBisector)) return newGrounded;

            AngleBisector angleBisector = c as AngleBisector;
            KeyValuePair<Angle, Angle> adjacentAngles = angleBisector.GetBisectedAngles();

            // Construct 2 m\angle CAD
            Multiplication product1 = new Multiplication(new NumericValue(2), adjacentAngles.Key);
            // Construct 2 m\angle BAD
            Multiplication product2 = new Multiplication(new NumericValue(2), adjacentAngles.Value);

            // 2X = AB
            GeometricAngleEquation newEq1 = new GeometricAngleEquation(product1, angleBisector.angle);
            GeometricAngleEquation newEq2 = new GeometricAngleEquation(product2, angleBisector.angle);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(angleBisector);

            newGrounded.Add(new EdgeAggregator(antecedent, newEq1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, newEq2, annotation));

            return newGrounded;
        }
    }
}