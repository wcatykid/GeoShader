using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ExteriorAngleHalfDifferenceInterceptedArcs : Theorem
    {
        private readonly static string NAME = "The measure of an angle formed by two secants, two tangents, or a secant and a tangent drawm from a point outside a circle is equal to half the difference of the measure of the intercepted arcs.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS);

        public static void Clear()
        {
            candidateTangent.Clear();
            candidateCircle.Clear();
            candidateIntersection.Clear();
            candidateStrengthened.Clear();
        }

        private static List<Tangent> candidateTangent = new List<Tangent>();
        private static List<Circle> candidateCircle = new List<Circle>();
        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //
        //    A \   
        //       \    B
        //        \  /
        //  O      \/ X
        //         /\
        //        /  \
        //     C /    D
        //
        // Two tangents:
        // Intersection(X, AD, BC), Tangent(Circle(O), BC), Tangent(Circle(O), AD) -> 2 * Angle(AXC) = MajorArc(AC) - MinorArc(AC)
        //
        // One Secant, One Tangent
        // Intersection(X, AD, BC), Tangent(Circle(O), BC) -> 2 * Angle(AXC) = MajorArc(AC) - MinorArc(AC)
        //
        // Two Secants
        // Intersection(X, AD, BC) -> 2 * Angle(AXC) = MajorArc(AC) - MinorArc(AC)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;

                foreach (Circle circle in candidateCircle)
                {
                    newGrounded.AddRange(InstantiateTwoSecantsTheorem(newInter, circle));
                }

                candidateIntersection.Add(newInter);
            }
            else if (clause is Circle)
            {
                Circle circle = clause as Circle;

                foreach (Intersection oldInter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateTwoSecantsTheorem(oldInter, circle));
                }

                candidateCircle.Add(circle);
            }
            else if (clause is Tangent)
            {
                Tangent newTangent = clause as Tangent;

                if (!(newTangent.intersection is CircleSegmentIntersection)) return newGrounded;

                // 1 secant / 1 tangent
                foreach (Intersection oldInter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateOneSecantOneTangentTheorem(oldInter, newTangent, newTangent));
                }

                // 2 tangents
                foreach (Intersection oldInter in candidateIntersection)
                {
                    foreach (Tangent oldTangent in candidateTangent)
                    {
                        newGrounded.AddRange(InstantiateTwoTangentsTheorem(newTangent, oldTangent, oldInter, newTangent, oldTangent));
                    }
                    foreach (Strengthened streng in candidateStrengthened)
                    {
                        newGrounded.AddRange(InstantiateTwoTangentsTheorem(newTangent, streng.strengthened as Tangent, oldInter, newTangent, streng));
                    }
                }

                candidateTangent.Add(newTangent);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is Tangent)) return newGrounded;
                if (!((newStreng.strengthened as Tangent).intersection is CircleSegmentIntersection)) return newGrounded;

                // 1 secant / 1 tangent
                foreach (Intersection oldInter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateOneSecantOneTangentTheorem(oldInter, newStreng.strengthened as Tangent, newStreng));
                }

                // 2 tangents
                foreach (Intersection oldInter in candidateIntersection)
                {
                    foreach (Tangent oldTangent in candidateTangent)
                    {
                        newGrounded.AddRange(InstantiateTwoTangentsTheorem(newStreng.strengthened as Tangent, oldTangent, oldInter, newStreng, oldTangent));
                    }
                    foreach (Strengthened streng in candidateStrengthened)
                    {
                        newGrounded.AddRange(InstantiateTwoTangentsTheorem(newStreng.strengthened as Tangent,
                                                                           streng.strengthened as Tangent, oldInter, newStreng, streng));
                    }
                }

                candidateStrengthened.Add(newStreng);
            }

            return newGrounded;
        }

        //
        //    A \   
        //       \    B
        //        \  /
        //  O      \/ X
        //         /\
        //        /  \
        //     C /    D
        //
        // Two Secants
        // Intersection(X, AD, BC) -> 2 * Angle(AXC) = MajorArc(AC) - MinorArc(AC)
        //
        public static List<EdgeAggregator> InstantiateTwoSecantsTheorem(Intersection inter, Circle circle)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Is this intersection explicitly outside the circle? That is, the point of intersection exterior?
            if (!circle.PointIsExterior(inter.intersect)) return newGrounded;

            //
            // Get the chords
            //
            Segment chord1 = circle.ContainsChord(inter.lhs);
            Segment chord2 = circle.ContainsChord(inter.rhs);

            if (chord1 == null || chord2 == null) return newGrounded;

            Point closeChord1Pt = null;
            Point closeChord2Pt = null;
            Point farChord1Pt = null;
            Point farChord2Pt = null;
            if (Segment.Between(chord1.Point1, chord1.Point2, inter.intersect))
            {
                closeChord1Pt = chord1.Point1;
                farChord1Pt = chord1.Point2;
            }
            else
            {
                closeChord1Pt = chord1.Point2;
                farChord1Pt = chord1.Point1;
            }

            if (Segment.Between(chord2.Point1, chord2.Point2, inter.intersect))
            {
                closeChord2Pt = chord2.Point1;
                farChord2Pt = chord2.Point2;
            }
            else
            {
                closeChord2Pt = chord2.Point2;
                farChord2Pt = chord2.Point1;
            }

            //
            // Get the arcs
            //
            Arc closeArc = Arc.GetFigureMinorArc(circle, closeChord1Pt, closeChord2Pt);
            Arc farArc = Arc.GetFigureMinorArc(circle, farChord1Pt, farChord2Pt);

            Angle theAngle = Angle.AcquireFigureAngle(new Angle(closeChord1Pt, inter.intersect, closeChord2Pt));

            //
            // Construct the new relationship
            //
            NumericValue two = new NumericValue(2);

            //GeometricAngleEquation gaeq = new GeometricAngleEquation(new Multiplication(two, theAngle), new Subtraction(farArc, closeArc));
            GeometricAngleArcEquation gaaeq = new GeometricAngleArcEquation(new Multiplication(two, theAngle), new Subtraction(farArc, closeArc));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter);
            antecedent.Add(circle);
            antecedent.Add(closeArc);
            antecedent.Add(farArc);

            newGrounded.Add(new EdgeAggregator(antecedent, gaaeq, annotation));

            return newGrounded;
        }

                //
        //    A \   
        //       \    B
        //        \  /
        //  O      \/ X
        //         /\
        //        /  \
        //     C /    D
        //
        // One Secant, One Tangent
        // Intersection(X, AD, BC), Tangent(Circle(O), BC) -> 2 * Angle(AXC) = MajorArc(AC) - MinorArc(AC)
        //
        public static List<EdgeAggregator> InstantiateOneSecantOneTangentTheorem(Intersection inter, Tangent tangent, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            CircleSegmentIntersection tan = tangent.intersection as CircleSegmentIntersection;

            // Is the tangent segment part of the intersection?
            if (!inter.HasSegment(tan.segment)) return newGrounded;

            // Acquire the chord that the intersection creates.
            Segment secant = inter.OtherSegment(tan.segment);

            Circle circle = tan.theCircle;
            Segment chord = circle.ContainsChord(secant);

            // Check if this segment never intersects the circle or doesn't create a chord.
            if (chord == null) return newGrounded;

            //
            // Get the near / far points out of the chord
            //
            Point closeChordPt = null;
            Point farChordPt = null;
            if (Segment.Between(chord.Point1, chord.Point2, inter.intersect))
            {
                closeChordPt = chord.Point1;
                farChordPt = chord.Point2;
            }
            else
            {
                closeChordPt = chord.Point2;
                farChordPt = chord.Point1;
            }

            //
            // Acquire the arcs
            //
            // Get the close arc first which we know exactly how it is constructed AND that it's a minor arc.
            Arc closeArc = Arc.GetFigureMinorArc(circle, closeChordPt, tan.intersect);

            // The far arc MAY be a major arc; if it is, the first candidate arc will contain the close arc.
            Arc farArc = Arc.GetFigureMinorArc(circle, farChordPt, tan.intersect);

            if (farArc.HasMinorSubArc(closeArc))
            {
                farArc = Arc.GetFigureMajorArc(circle, farChordPt, tan.intersect);
            }

            Angle theAngle = Angle.AcquireFigureAngle(new Angle(closeChordPt, inter.intersect, tan.intersect));

            //
            // Construct the new relationship
            //
            NumericValue two = new NumericValue(2);

            GeometricAngleArcEquation gaaeq = new GeometricAngleArcEquation(new Multiplication(two, theAngle), new Subtraction(farArc, closeArc));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(inter);
            antecedent.Add(closeArc);
            antecedent.Add(farArc);

            newGrounded.Add(new EdgeAggregator(antecedent, gaaeq, annotation));

            return newGrounded;
        }

        //
        //    A \   
        //       \    B
        //        \  /
        //  O      \/ X
        //         /\
        //        /  \
        //     C /    D
        //
        // Two tangents:
        // Intersection(X, AD, BC), Tangent(Circle(O), BC), Tangent(Circle(O), AD) -> 2 * Angle(AXC) = MajorArc(AC) - MinorArc(AC)
        //
        public static List<EdgeAggregator> InstantiateTwoTangentsTheorem(Tangent tangent1, Tangent tangent2, Intersection inter, GroundedClause original1, GroundedClause original2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            CircleSegmentIntersection tan1 = tangent1.intersection as CircleSegmentIntersection;
            CircleSegmentIntersection tan2 = tangent2.intersection as CircleSegmentIntersection;

            if (tan1.StructurallyEquals(tan2)) return newGrounded;

            // Do the tangents apply to the same circle?
            if (!tan1.theCircle.StructurallyEquals(tan2.theCircle)) return newGrounded;

            Circle circle = tan1.theCircle;

            // Do these tangents work with this intersection?
            if (!inter.HasSegment(tan1.segment) || !inter.HasSegment(tan2.segment)) return newGrounded;

            // Overkill? Do the tangents intersect at the same point as the intersection's intersect point?
            if (!tan1.segment.FindIntersection(tan2.segment).StructurallyEquals(inter.intersect)) return newGrounded;

            //
            // Get the arcs
            //
            Arc minorArc = new MinorArc(circle, tan1.intersect, tan2.intersect);
            Arc majorArc = new MajorArc(circle, tan1.intersect, tan2.intersect);

            Angle theAngle = new Angle(tan1.intersect, inter.intersect, tan2.intersect);

            //
            // Construct the new relationship
            //
            NumericValue two = new NumericValue(2);

            GeometricAngleArcEquation gaaeq = new GeometricAngleArcEquation(new Multiplication(two, theAngle), new Subtraction(majorArc, minorArc));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original1);
            antecedent.Add(original2);
            antecedent.Add(inter);
            antecedent.Add(majorArc);
            antecedent.Add(minorArc);

            newGrounded.Add(new EdgeAggregator(antecedent, gaaeq, annotation));

            return newGrounded;
        }
    }
}