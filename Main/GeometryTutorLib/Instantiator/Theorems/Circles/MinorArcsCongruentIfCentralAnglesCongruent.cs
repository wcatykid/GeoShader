using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MinorArcsCongruentIfCentralAnglesCongruent : Theorem
    {
        private readonly static string FORWARD_NAME = "If Central Angles are Congruent then Minor Arcs are Congruent";
        private static Hypergraph.EdgeAnnotation forwardAnnotation = new Hypergraph.EdgeAnnotation(FORWARD_NAME, EngineUIBridge.JustificationSwitch.MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT);

        private readonly static string CONVERSE_NAME = "If Minor Arcs are Congruent then Central Angles are Congruent";
        private static Hypergraph.EdgeAnnotation converseAnnotation = new Hypergraph.EdgeAnnotation(CONVERSE_NAME, EngineUIBridge.JustificationSwitch.CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT);

        public static void Clear()
        {
            candidateCongruentArcs.Clear();
            candidateCongruentAngles.Clear();
        }

        private static List<CongruentArcs> candidateCongruentArcs = new List<CongruentArcs>();
        private static List<CongruentAngles> candidateCongruentAngles = new List<CongruentAngles>();

        //               A
        //              /)
        //             /  )
        //            /    )
        // center:   O      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               C
        //
        //               D
        //              /)
        //             /  )
        //            /    )
        // center:   Q      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               F
        //
        // Congruent(Arc(A, C), Arc(D, F)) -> Congruent(Angle(AOC), Angle(DQF))
        //
        // Congruent(Angle(AOC), Angle(DQF)) -> Congruent(Arc(A, C), Arc(D, F))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            forwardAnnotation.active = EngineUIBridge.JustificationSwitch.MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT;
            converseAnnotation.active = EngineUIBridge.JustificationSwitch.CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentAngles)
            {
                newGrounded.AddRange(InstantiateForwardPartOfTheorem(clause as CongruentAngles));
            }

            if (clause is CongruentArcs)
            {
                newGrounded.AddRange(InstantiateConversePartOfTheorem(clause as CongruentArcs));
            }

            return newGrounded;
        }

        //               A
        //              /)
        //             /  )
        //            /    )
        // center:   O      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               C
        //
        //               D
        //              /)
        //             /  )
        //            /    )
        // center:   Q      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               F
        //
        // Congruent(Angle(AOC), Angle(DQF)) -> Congruent(Arc(A, C), Arc(D, F))
        //
        private static List<EdgeAggregator> InstantiateForwardPartOfTheorem(CongruentAngles cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine if the angles are central angles.
            //
            List<Circle> circles1 = Circle.IsCentralAngle(cas.ca1);
            //Circle circle1 = null;  Circle.IsCentralAngle(cas.ca1);
            if (!circles1.Any()) return newGrounded;

            List<Circle> circles2 = Circle.IsCentralAngle(cas.ca2);
            //Circle circle2 = null;  Circle.IsCentralAngle(cas.ca2);
            if (!circles2.Any()) return newGrounded;
            
            //Construct arc congruences between congruent circles
            foreach (Circle c1 in circles1)
            {
                foreach (Circle c2 in circles2)
                {
                    if (c1.radius == c2.radius)
                    {
                        Arc a1 = Arc.GetInterceptedArc(c1, cas.ca1);
                        Arc a2 = Arc.GetInterceptedArc(c2, cas.ca2);
                        if (a1 != null & a2 != null)
                        {
                            GeometricCongruentArcs gcas = new GeometricCongruentArcs(a1, a2);

                            // For hypergraph
                            List<GroundedClause> antecedent = new List<GroundedClause>();
                            antecedent.Add(a1);
                            antecedent.Add(a2);
                            antecedent.Add(cas);

                            newGrounded.Add(new EdgeAggregator(antecedent, gcas, forwardAnnotation));
                        }
                    }
                }
            }
            //
            // Construct the arc congruence by acquiring the arc endpoints.
            //
            //Point endpt11, endpt12, garbage;
            //circle1.FindIntersection(cas.ca1.ray1, out endpt11, out garbage);
            //if (endpt11 == null) return newGrounded;

            //circle1.FindIntersection(cas.ca1.ray2, out endpt12, out garbage);
            //if (endpt12 == null) return newGrounded;

            //Point endpt21, endpt22;
            //circle1.FindIntersection(cas.ca2.ray1, out endpt21, out garbage);
            //if (endpt21 == null) return newGrounded;

            //circle1.FindIntersection(cas.ca2.ray2, out endpt22, out garbage);
            //if (endpt22 == null) return newGrounded;

            //Arc arc1 = Arc.GetFigureMinorArc(circle1, endpt11, endpt12);
            //Arc arc2 = Arc.GetFigureMinorArc(circle2, endpt21, endpt22);
            //if (arc1 == null || arc2 == null) return newGrounded;


            return newGrounded;
        }

        //               A
        //              /)
        //             /  )
        //            /    )
        // center:   O      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               C
        //
        //               D
        //              /)
        //             /  )
        //            /    )
        // center:   Q      )
        //            \    )   
        //             \  ) 
        //              \) 
        //               F
        //
        // Congruent(Arc(A, C), Arc(D, F)) -> Congruent(Angle(AOC), Angle(DQF))
        //
        private static List<EdgeAggregator> InstantiateConversePartOfTheorem(CongruentArcs cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Get the radii (and determine if the exist in the figure)
            //
            Segment radius11;
            Segment radius12;
            cas.ca1.GetRadii(out radius11, out radius12);

            if (radius11 == null || radius12 == null) return newGrounded;

            Segment radius21;
            Segment radius22;
            cas.ca2.GetRadii(out radius21, out radius22);

            if (radius21 == null || radius22 == null) return newGrounded;

            //
            // Acquire the central angles from the respoitory
            //
            Angle central1 = Angle.AcquireFigureAngle(new Angle(radius11, radius12));
            Angle central2 = Angle.AcquireFigureAngle(new Angle(radius21, radius22));

            if (central1 == null || central2 == null) return newGrounded;

            GeometricCongruentAngles gcas = new GeometricCongruentAngles(central1, central2);
    
            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(central1);
            antecedent.Add(central2);
            antecedent.Add(cas);

            newGrounded.Add(new EdgeAggregator(antecedent, gcas, converseAnnotation));

            return newGrounded;
        }
    }
}