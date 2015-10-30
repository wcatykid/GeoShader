using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TwoInterceptedArcsHaveCongruentAngles : Theorem
    {
        private readonly static string NAME = "If two inscribed angles intercept the same arc, the angles are congruent.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES);

        public static void Clear()
        {
            candidateAngles.Clear();
        }

        private static List<Angle> candidateAngles = new List<Angle>();

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Angle)
            {
                Angle angle = clause as Angle;

                foreach (Angle candCongruentAngle in candidateAngles)
                {
                    newGrounded.AddRange(InstantiateTheorem(angle, candCongruentAngle));
                }

                candidateAngles.Add(angle);
            }

            else if (clause is Circle)
            {
                Circle circle = clause as Circle;

                for (int i = 0; i < candidateAngles.Count; ++i)
                {
                    for (int j = i + 1; j < candidateAngles.Count; ++j)
                    {
                        newGrounded.AddRange(InstantiateTheorem(candidateAngles[i], candidateAngles[j]));
                    }
                }

            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateTheorem(Angle a1, Angle a2)
        {
             List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Acquire all circles in which the angles are inscribed
            List<Circle> circles1 = Circle.IsInscribedAngle(a1);
            List<Circle> circles2 = Circle.IsInscribedAngle(a2);

            //Acquire the common circles in which both angles are inscribed
            List<Circle> circles = (circles1.Intersect(circles2)).ToList();

            //For each common circle, check for equivalent itercepted arcs
            foreach (Circle c in circles)
            {
                Arc i1 = Arc.GetInterceptedArc(c, a1);
                Arc i2 = Arc.GetInterceptedArc(c, a2);

                if (i1.StructurallyEquals(i2))
                {
                    GeometricCongruentAngles gcas = new GeometricCongruentAngles(a1, a2);
                    
                    //For hypergraph
                    List<GroundedClause> antecedent = new List<GroundedClause>();
                    antecedent.Add(c);
                    antecedent.Add(a1);
                    antecedent.Add(a2);
                    antecedent.Add(i1);

                    newGrounded.Add(new EdgeAggregator(antecedent, gcas, annotation));
                }
            }

            return newGrounded;
        }
    }
}