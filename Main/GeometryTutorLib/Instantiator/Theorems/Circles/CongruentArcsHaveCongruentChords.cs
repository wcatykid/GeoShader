using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentArcsHaveCongruentChords : Theorem
    {
        private readonly static string FORWARD_NAME = "Congruent Chords have Congruent Arcs";
        private static Hypergraph.EdgeAnnotation forwardAnnotation = new Hypergraph.EdgeAnnotation(FORWARD_NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS);

        private readonly static string CONVERSE_NAME = "Congruent Arcs have Congruent Chords";
        private static Hypergraph.EdgeAnnotation converseAnnotation = new Hypergraph.EdgeAnnotation(CONVERSE_NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS);

        public static void Clear()
        {
            candidateCongruentArcs.Clear();
            candidateCongruentSegments.Clear();
        }

        private static List<CongruentArcs> candidateCongruentArcs = new List<CongruentArcs>();
        private static List<CongruentSegments> candidateCongruentSegments = new List<CongruentSegments>();

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
        // Congruent(Arc(A, C), Arc(D, F)) -> Congruent(Segment(AC), Segment(DF))
        //
        // Congruent(Segment(AC), Segment(DF)) -> Congruent(Arc(A, C), Arc(D, F))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            forwardAnnotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS;
            converseAnnotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentSegments)
            {
                newGrounded.AddRange(InstantiateForwardPartOfTheorem(clause as CongruentSegments));
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
        // Congruent(Segment(AC), Segment(DF)) -> Congruent(Arc(A, C), Arc(D, F))
        //
        private static List<EdgeAggregator> InstantiateForwardPartOfTheorem(CongruentSegments cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Acquire the circles for which the segments are chords.
            //
            List<Circle> circles1 = Circle.GetChordCircles(cas.cs1);
            List<Circle> circles2 = Circle.GetChordCircles(cas.cs2);

            //
            // Make all possible combinations of arcs congruent
            //
            foreach (Circle circle1 in circles1)
            {
                // Create the appropriate type of arcs from the chord and the circle
                List<Semicircle> c1semi = null;
                MinorArc c1minor = null;
                MajorArc c1major = null;

                if (circle1.DefinesDiameter(cas.cs1))
                {
                    c1semi = CreateSemiCircles(circle1, cas.cs1);
                }
                else
                {
                    c1minor = new MinorArc(circle1, cas.cs1.Point1, cas.cs1.Point2);
                    c1major = new MajorArc(circle1, cas.cs1.Point1, cas.cs1.Point2);
                }

                foreach (Circle circle2 in circles2)
                {
                    //The two circles must be the same or congruent
                    if (circle1.radius == circle2.radius)
                    {
                        List<Semicircle> c2semi = null;
                        MinorArc c2minor = null;
                        MajorArc c2major = null;

                        List<GeometricCongruentArcs> congruencies = new List<GeometricCongruentArcs>();
                        if (circle2.DefinesDiameter(cas.cs2))
                        {
                            c2semi = CreateSemiCircles(circle2, cas.cs2);
                            congruencies.AddRange(EquateSemiCircles(c1semi, c2semi));
                        }
                        else
                        {
                            c2minor = new MinorArc(circle2, cas.cs2.Point1, cas.cs2.Point2);
                            c2major = new MajorArc(circle2, cas.cs2.Point1, cas.cs2.Point2);
                            congruencies.Add(new GeometricCongruentArcs(c1minor, c2minor));
                            congruencies.Add(new GeometricCongruentArcs(c1major, c2major));
                        }

                        // For hypergraph
                        List<GroundedClause> antecedent = new List<GroundedClause>();
                        antecedent.Add(cas.cs1);
                        antecedent.Add(cas.cs2);
                        antecedent.Add(cas);

                        foreach (GeometricCongruentArcs gcas in congruencies)
                        {
                            newGrounded.Add(new EdgeAggregator(antecedent, gcas, forwardAnnotation));
                        }
                    }
                }
            }

            return newGrounded;
        }

        private static List<Semicircle> CreateSemiCircles(Circle circle1, Segment diameter)
        {
            int e1 = circle1.pointsOnCircle.IndexOf(diameter.Point1);
            int e2 = circle1.pointsOnCircle.IndexOf(diameter.Point2);

            List<Semicircle> semis = new List<Semicircle>();

            for (int i = 0; i < circle1.pointsOnCircle.Count; ++i)
            {
                if (i != e1 && i != e2) semis.Add(new Semicircle(circle1, diameter.Point1, diameter.Point2, circle1.pointsOnCircle[i], diameter)); 
            }

            return semis;
        }

        private static List<GeometricCongruentArcs> EquateSemiCircles(List<Semicircle> s1, List<Semicircle> s2)
        {
            List<GeometricCongruentArcs> newCongruencies = new List<GeometricCongruentArcs>();

            foreach (Semicircle semi1 in s1)
            {
                foreach (Semicircle semi2 in s2)
                {
                    if (!semi1.SameSideSemicircle(semi2)) newCongruencies.Add(new GeometricCongruentArcs(semi1, semi2));
                }
            }

            return newCongruencies;
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
        // Congruent(Arc(A, C), Arc(D, F)) -> Congruent(Segment(AC), Segment(DF))
        //
        private static List<EdgeAggregator> InstantiateConversePartOfTheorem(CongruentArcs cas)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Acquire the chords for this specific pair of arcs (endpoints of arc and segment are the same).
            //
            Segment chord1 = Segment.GetFigureSegment(cas.ca1.endpoint1, cas.ca1.endpoint2);
            Segment chord2 = Segment.GetFigureSegment(cas.ca2.endpoint1, cas.ca2.endpoint2);

            if (chord1 == null || chord2 == null) return newGrounded;

            // Construct the congruence
            GeometricCongruentSegments gcss = new GeometricCongruentSegments(chord1, chord2);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(chord1);
            antecedent.Add(chord2);
            antecedent.Add(cas);

            newGrounded.Add(new EdgeAggregator(antecedent, gcss, forwardAnnotation));

            return newGrounded;
        }
    }
}