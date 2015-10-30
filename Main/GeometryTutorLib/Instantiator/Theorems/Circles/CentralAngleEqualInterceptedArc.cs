using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CentralAngleEqualInterceptedArc : Theorem
    {
        private readonly static string NAME = "The measure of a central angle is equal to the measure of its intercepted arc.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC);

        public static void Clear()
        {
            candidateCircles.Clear();
            candidateAngles.Clear();
        }

        private static List<Circle> candidateCircles = new List<Circle>();
        private static List<Angle> candidateAngles = new List<Angle>();

        //              A
        //            / |)
        //           /  | )
        //          /   |  )
        // Q------- O---X---) D
        //          \   |  )   
        //           \  | ) 
        //            \ |) 
        //              C
        //
        // Central Angle(AQC) -> Angle(AQC) = Arc(AC)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC;

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

        //              A
        //            / |)
        //           /  | )
        //          /   |  )
        // Q------- O---X---) D
        //          \   |  )   
        //           \  | ) 
        //            \ |) 
        //              C
        //
        // Central Angle(AQC) -> Angle(AQC) = Arc(AC)
        //
        private static List<EdgeAggregator> InstantiateTheorem(Circle circle, Angle angle)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Acquire all circles in which the angle is inscribed
            List<Circle> circles = Circle.IsCentralAngle(angle);

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
            GeometricAngleArcEquation gaeq = new GeometricAngleArcEquation(angle, intercepted);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(circle);
            antecedent.Add(angle);
            antecedent.Add(intercepted);
 
            newGrounded.Add(new EdgeAggregator(antecedent, gaeq, annotation));

            return newGrounded;
        }
    }
}