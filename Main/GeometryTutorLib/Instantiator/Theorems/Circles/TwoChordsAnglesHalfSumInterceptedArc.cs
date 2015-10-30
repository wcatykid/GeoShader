using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TwoChordsAnglesHalfSumInterceptedArc : Theorem
    {
        private readonly static string NAME = "The measure of an angle formed by two chords that intersect inside a circle is equal to half the sum of the measures of the interecepted arcs.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS);

        public static void Clear()
        {
            candidateCircle.Clear();
            candidateIntersection.Clear();
        }

        private static List<Circle> candidateCircle = new List<Circle>();
        private static List<Intersection> candidateIntersection = new List<Intersection>();

        //
        //    A \      / B
        //       \    /
        //        \  /
        //  O      \/ X
        //         /\
        //        /  \
        //     C /    \ D
        //
        // Intersection(Segment(AD), Segment(BC)) -> 2 * Angle(CXA) = Arc(A, C) + Arc(B, D),
        //                                           2 * Angle(BXD) = Arc(A, C) + Arc(B, D),
        //                                           2 * Angle(AXB) = Arc(A, B) + Arc(C, D),
        //                                           2 * Angle(CXD) = Arc(A, B) + Arc(C, D)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;

                foreach (Circle circle in candidateCircle)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, circle));
                }

                candidateIntersection.Add(newInter);
            }
            else if (clause is Circle)
            {
                Circle circle = clause as Circle;

                foreach (Intersection oldInter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, circle));
                }

                candidateCircle.Add(circle);
            }

            return newGrounded;
        }

        //
        //    A \      / B
        //       \    /
        //        \  /
        //  O      \/ X
        //         /\
        //        /  \
        //     C /    \ D
        //
        // Intersection(Segment(AD), Segment(BC)) -> 2 * Angle(CXA) = Arc(A, C) + Arc(B, D),
        //                                           2 * Angle(BXD) = Arc(A, C) + Arc(B, D),
        //                                           2 * Angle(AXB) = Arc(A, B) + Arc(C, D),
        //                                           2 * Angle(CXD) = Arc(A, B) + Arc(C, D)
        //
        public static List<EdgeAggregator> InstantiateTheorem(Intersection inter, Circle circle)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Is this intersection explicitly inside the circle? That is, the point of intersection interior?
            if (!circle.PointIsInterior(inter.intersect)) return newGrounded;

            //
            // Get the chords
            //
            Segment chord1 = circle.GetChord(inter.lhs);
            Segment chord2 = circle.GetChord(inter.rhs);

            if (chord1 == null || chord2 == null) return newGrounded;

            //
            // Group 1
            //
            Arc arc1 = Arc.GetFigureMinorArc(circle, chord1.Point1, chord2.Point1);
            Angle angle1 = Angle.AcquireFigureAngle(new Angle(chord1.Point1, inter.intersect, chord2.Point1));

            Arc oppArc1 = Arc.GetFigureMinorArc(circle, chord1.Point2, chord2.Point2);
            Angle oppAngle1 = Angle.AcquireFigureAngle(new Angle(chord1.Point2, inter.intersect, chord2.Point2));

            //
            // Group 2
            //
            Arc arc2 = Arc.GetFigureMinorArc(circle, chord1.Point1, chord2.Point2);
            Angle angle2 = Angle.AcquireFigureAngle(new Angle(chord1.Point1, inter.intersect, chord2.Point2));

            Arc oppArc2 = Arc.GetFigureMinorArc(circle, chord1.Point2, chord2.Point1);
            Angle oppAngle2 = Angle.AcquireFigureAngle(new Angle(chord1.Point2, inter.intersect, chord2.Point1));
            
            //
            // Construct each of the 4 equations.
            //
            NumericValue two = new NumericValue(2);

            GeometricAngleArcEquation gaeq1 = new GeometricAngleArcEquation(new Multiplication(two, angle1), new Addition(arc1, oppArc1));
            GeometricAngleArcEquation gaeq2 = new GeometricAngleArcEquation(new Multiplication(two, oppAngle1), new Addition(arc1, oppArc1));

            GeometricAngleArcEquation gaeq3 = new GeometricAngleArcEquation(new Multiplication(two, angle2), new Addition(arc2, oppArc2));
            GeometricAngleArcEquation gaeq4 = new GeometricAngleArcEquation(new Multiplication(two, oppAngle2), new Addition(arc2, oppArc2));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter);
            antecedent.Add(circle);

            newGrounded.Add(new EdgeAggregator(antecedent, gaeq1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, gaeq2, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, gaeq3, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, gaeq4, annotation));

            return newGrounded;
        }
    }
}