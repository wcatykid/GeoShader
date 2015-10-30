using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class EquilateralTriangleHasSixtyDegreeAngles : Theorem
    {
        private readonly static string NAME = "An equilateral triangle has three sixty degree angles.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES);

        public EquilateralTriangleHasSixtyDegreeAngles() { }

        //
        // EquilateralTriangle(A, B, C) -> Equation(m \angle ABC = 60),  
        //                                 Equation(m \angle BCA = 60),
        //                                 Equation(m \angle CAB = 60)
        //

        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is EquilateralTriangle) && !(c is Strengthened)) return newGrounded;

            EquilateralTriangle eqTri = c as EquilateralTriangle;

            if (c is Strengthened)
            {
                eqTri = (c as Strengthened).strengthened as EquilateralTriangle; 
            }

            if (eqTri == null) return newGrounded;

            // EquilateralTriangle(A, B, C) ->
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(c);

            //                              -> Equation(m \angle ABC = 60),  
            //                                 Equation(m \angle BCA = 60),
            //                                 Equation(m \angle CAB = 60)
            GeometricAngleEquation eq1 = new GeometricAngleEquation(eqTri.AngleA, new NumericValue(60));
            GeometricAngleEquation eq2 = new GeometricAngleEquation(eqTri.AngleB, new NumericValue(60));
            GeometricAngleEquation eq3 = new GeometricAngleEquation(eqTri.AngleC, new NumericValue(60));

            newGrounded.Add(new EdgeAggregator(antecedent, eq1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, eq2, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, eq3, annotation));

            return newGrounded;
        }
    }
}