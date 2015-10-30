using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SumAnglesInTriangle : Theorem
    {
        private readonly static string NAME = "Sum of Measure of Three Angles in Triangle is 180";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SUM_ANGLES_IN_TRIANGLE_180);

        // 
        // Triangle(A, B, C) -> m\angle ABC + m\angle CAB + m\angle BCA = 180^o
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SUM_ANGLES_IN_TRIANGLE_180;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Triangle tri = c as Triangle;
            if (tri == null) return newGrounded;

            // Generate, by definition the sum of the three angles equal 180^o
            Angle a1 = new Angle(tri.Point1, tri.Point2, tri.Point3);
            Angle a2 = new Angle(tri.Point3, tri.Point1, tri.Point2);
            Angle a3 = new Angle(tri.Point2, tri.Point3, tri.Point1);

            Addition add = new Addition(a1, a2);
            Addition overallAdd = new Addition(add, a3);
            NumericValue value = new NumericValue(180); // Sum is 180^o
            GeometricAngleEquation eq = new GeometricAngleEquation(overallAdd, value);

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(tri);
            newGrounded.Add(new EdgeAggregator(antecedent, eq, annotation));

            return newGrounded;
        }
    }
}