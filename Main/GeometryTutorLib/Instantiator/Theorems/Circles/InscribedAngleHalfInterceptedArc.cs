using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class InscribedAngleHalfInterceptedArc : Theorem
    {
        private readonly static string NAME = "The measure of an inscribed angle is equal to half the measure of its intercepted arc.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC);

        public static void Clear()
        {
            candidateCircles.Clear();
            candidateAngles.Clear();
        }

        private static List<Circle> candidateCircles = new List<Circle>();
        private static List<Angle> candidateAngles = new List<Angle>();

        //     /        A
        //    /         |)
        //   /          | )
        //  /           |  )
        // Q------- O---X---) D
        //   \          |  )   
        //    \         | ) 
        //     \        |) 
        //      \        C
        //
        // Inscribed Angle(AQC) -> Angle(AQC) = 2 * Arc(AC)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Angle)
            {
                Angle angle = clause as Angle;

                foreach (Circle circle in candidateCircles)
                {
                    newGrounded.AddRange(InstantiateTheorem(circle, angle));
                }

                candidateAngles.Add(angle);
            }
            else if (clause is Circle)
            {
                Circle circle = clause as Circle;

                foreach (Angle angle in candidateAngles)
                {
                    newGrounded.AddRange(InstantiateTheorem(circle, angle));
                }

                candidateCircles.Add(circle);
            }

            return newGrounded;
        }

        //     /        A
        //    /         |)
        //   /          | )
        //  /           |  )
        // Q------- O---X---) D
        //   \          |  )   
        //    \         | ) 
        //     \        |) 
        //      \        C
        //
        // Inscribed Angle(AQC) -> Angle(AQC) = 2 * Arc(AC)
        //
        private static List<EdgeAggregator> InstantiateTheorem(Circle circle, Angle angle)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Acquire all circles in which the angle is inscribed
            List<Circle> circles = Circle.IsInscribedAngle(angle);

            //
            // Get this particular inscribed circle.
            //
            Circle inscribed = null;
            foreach (Circle c in circles)
            {
                if (circle.StructurallyEquals(c)) inscribed = c;
            }

            if (inscribed == null) return newGrounded;

            // Get the intercepted arc
            Arc intercepted = Arc.GetInterceptedArc(inscribed, angle);

            //
            // Create the equation
            //
            Multiplication product = new Multiplication(new NumericValue(2), angle);
            GeometricAngleArcEquation gaaeq = new GeometricAngleArcEquation(product, intercepted);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(circle);
            antecedent.Add(angle);
            antecedent.Add(intercepted);
 
            newGrounded.Add(new EdgeAggregator(antecedent, gaaeq, annotation));

            return newGrounded;
        }
    }
}