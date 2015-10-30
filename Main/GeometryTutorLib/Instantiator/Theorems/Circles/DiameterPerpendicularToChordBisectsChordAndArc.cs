using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiameterPerpendicularToChordBisectsChordAndArc : Theorem
    {
        private readonly static string NAME = "A Diameter perpendicular to a chord bisects the Chord and Arc";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS);

        public static void Clear()
        {
            candidateIntersections.Clear();
            candidateCircleIntersections.Clear();
        }

        private static List<Intersection> candidateIntersections = new List<Intersection>();
        private static List<CircleSegmentIntersection> candidateCircleIntersections = new List<CircleSegmentIntersection>();

        //               A
        //              |)
        //              | )
        //              |  )
        // Q------- O---X---) D
        //              |  )   
        //              | ) 
        //              |) 
        //               C
        //
        // Perpendicular(Segment(Q, D), Segment(A, C)) -> Congruent(Arc(A, D), Arc(D, C)) -> Congruent(Segment(AX), Segment(XC))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Intersection)
            {
                candidateIntersections.Add(clause as Intersection);
            }
            else if (clause is CircleSegmentIntersection)
            {
                candidateCircleIntersections.Add(clause as CircleSegmentIntersection);
            }
            else if (clause is Perpendicular)
            {
                foreach (Intersection oldInter in candidateIntersections)
                {
                    foreach (CircleSegmentIntersection oldArcInter in candidateCircleIntersections)
                    {
                        newGrounded.AddRange(InstantiateTheorem(oldInter, oldArcInter, clause as Perpendicular, clause));
                    }
                }
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Perpendicular)) return newGrounded;
                
                foreach (Intersection oldInter in candidateIntersections)
                {
                    foreach (CircleSegmentIntersection oldArcInter in candidateCircleIntersections)
                    {
                        newGrounded.AddRange(InstantiateTheorem(oldInter, oldArcInter, streng.strengthened as Perpendicular, clause));
                    }
                }

            }

            return newGrounded;
        }

        //               A
        //              |)
        //              | )
        //              |  )
        // Q------- O---X---) D
        //              |  )   
        //              | ) 
        //              |) 
        //               C
        //
        // Perpendicular(Segment(Q, D), Segment(A, C)) -> Congruent(Arc(A, D), Arc(D, C)) -> Congruent(Segment(AX), Segment(XC))
        //
        private static List<EdgeAggregator> InstantiateTheorem(Intersection inter, CircleSegmentIntersection arcInter, Perpendicular perp, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Does this intersection apply to this perpendicular?
            //
            if (!inter.intersect.StructurallyEquals(perp.intersect)) return newGrounded;

            // Is this too restrictive?
            if (!((inter.HasSegment(perp.lhs) && inter.HasSegment(perp.rhs)) && (perp.HasSegment(inter.lhs) && perp.HasSegment(inter.rhs)))) return newGrounded;

            //
            // Does this perpendicular intersection apply to a given circle?
            //
            // Acquire the circles for which the segments are secants.
            List<Circle> secantCircles1 = Circle.GetSecantCircles(perp.lhs);
            List<Circle> secantCircles2 = Circle.GetSecantCircles(perp.rhs);

            List<Circle> intersection = Utilities.Intersection<Circle>(secantCircles1, secantCircles2);

            if (!intersection.Any()) return newGrounded;

            //
            // Find the single, unique circle that has as chords the components of the perpendicular intersection
            //
            Circle theCircle = null;
            Segment chord1 = null;
            Segment chord2 = null;
            foreach (Circle circle in intersection)
            {
                chord1 = circle.GetChord(perp.lhs);
                chord2 = circle.GetChord(perp.rhs);
                if (chord1 != null && chord2 != null)
                {
                    theCircle = circle;
                    break;
                }
            }

            Segment diameter = chord1.Length > chord2.Length ? chord1 : chord2;
            Segment chord = chord1.Length < chord2.Length ? chord1 : chord2;

            //
            // Does the arc intersection apply?
            //
            if (!arcInter.HasSegment(diameter)) return newGrounded;
            if (!theCircle.StructurallyEquals(arcInter.theCircle)) return newGrounded;

            //
            // Create the bisector
            //
            Strengthened sb = new Strengthened(inter, new SegmentBisector(inter, diameter));
            Strengthened ab = new Strengthened(arcInter, new ArcSegmentBisector(arcInter));

            // For hypergraph
            List<GroundedClause> antecedentArc = new List<GroundedClause>();
            antecedentArc.Add(arcInter);
            antecedentArc.Add(original);
            antecedentArc.Add(theCircle);

            newGrounded.Add(new EdgeAggregator(antecedentArc, ab, annotation));

            List<GroundedClause> antecedentSegment = new List<GroundedClause>();
            antecedentSegment.Add(inter);
            antecedentSegment.Add(original);
            antecedentSegment.Add(theCircle);

            newGrounded.Add(new EdgeAggregator(antecedentSegment, sb, annotation));

            return newGrounded;
        }
    }
}