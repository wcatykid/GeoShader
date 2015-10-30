using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ChordTangentAngleHalfInterceptedArc : Theorem
    {
        private readonly static string NAME = "The measure of an angle formed by a chord and a tangent is equal to half the measure of the intercepted arc.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC);

        public static void Clear()
        {
            candidateIntersection.Clear();
            candidateTangent.Clear();
            candidateStrengthened.Clear();
        }

        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Tangent> candidateTangent = new List<Tangent>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //
        //          C
        //         /)
        //        /  )
        //       / )
        //      / )
        //   A /)_________ B
        //
        // Tangent(Circle(O), Segment(AB)), Intersection(Segment(AC), Segment(AB)) -> 2 * Angle(CAB) = Arc(C, B)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;

                foreach (Tangent tangent in candidateTangent)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, tangent, tangent));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateTheorem(newInter, streng.strengthened as Tangent, streng));
                }

                candidateIntersection.Add(newInter);
            }
            else if (clause is Tangent)
            {
                Tangent tangent = clause as Tangent;

                if (!(tangent.intersection is CircleSegmentIntersection)) return newGrounded;

                foreach (Intersection oldInter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, tangent, tangent));
                }

                candidateTangent.Add(tangent);
            }

            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Tangent)) return newGrounded;

                foreach (Intersection oldInter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateTheorem(oldInter, streng.strengthened as Tangent, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        //          C
        //         /)
        //        /  )
        //       / )
        //      / )
        //   A /)_________ B
        //
        // Tangent(Circle(O), Segment(AB)), Intersection(Segment(AC), Segment(AB)) -> 2 * Angle(CAB) = Arc(C, B)
        //
        public static List<EdgeAggregator> InstantiateTheorem(Intersection inter, Tangent tangent, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            CircleSegmentIntersection tan = tangent.intersection as CircleSegmentIntersection;

            //
            // Does this tangent apply to this intersection?
            //
            if (!inter.intersect.StructurallyEquals(tangent.intersection.intersect)) return newGrounded;

            Segment secant = null;
            Segment tanSegment = null;
            if (tan.HasSegment(inter.lhs))
            {
                secant = inter.rhs;
                tanSegment = inter.lhs;
            }
            else if (tan.HasSegment(inter.rhs))
            {
                secant = inter.lhs;
                tanSegment = inter.rhs;
            }
            else return newGrounded;

            //
            // Acquire the angle and intercepted arc.
            //
            Segment chord = tan.theCircle.GetChord(secant);
            if (chord == null) return newGrounded;
            //Segment chord = tan.theCircle.ContainsChord(secant);

            // Arc
            // We want the MINOR ARC only!
            if (tan.theCircle.DefinesDiameter(chord))
            {
                Arc theArc = null;
                Point midpt = PointFactory.GeneratePoint(tan.theCircle.Midpoint(chord.Point1, chord.Point2));
                Point opp = PointFactory.GeneratePoint(tan.theCircle.OppositePoint(midpt));

                Point tanPoint = tanSegment.OtherPoint(inter.intersect);

                if (tanPoint != null)
                {
                    // Angle; the smaller angle is always the chosen angle
                    Angle theAngle = new Angle(chord.OtherPoint(inter.intersect), inter.intersect, tanPoint);

                    theArc = new Semicircle(tan.theCircle, chord.Point1, chord.Point2, midpt, chord);
                    newGrounded.Add(CreateClause(inter, original, theAngle, theArc));

                    theArc = new Semicircle(tan.theCircle, chord.Point1, chord.Point2, opp, chord);
                    newGrounded.Add(CreateClause(inter, original, theAngle, theArc));
                }
                else
                {
                    // Angle; the smaller angle is always the chosen angle
                    Angle theAngle = new Angle(chord.OtherPoint(inter.intersect), inter.intersect, tanSegment.Point1);

                    theArc = new Semicircle(tan.theCircle, chord.Point1, chord.Point2, midpt, chord);
                    newGrounded.Add(CreateClause(inter, original, theAngle, theArc));

                    theArc = new Semicircle(tan.theCircle, chord.Point1, chord.Point2, opp, chord);
                    newGrounded.Add(CreateClause(inter, original, theAngle, theArc));

                    // Angle; the smaller angle is always the chosen angle
                    theAngle = new Angle(chord.OtherPoint(inter.intersect), inter.intersect, tanSegment.Point2);

                    theArc = new Semicircle(tan.theCircle, chord.Point1, chord.Point2, midpt, chord);
                    newGrounded.Add(CreateClause(inter, original, theAngle, theArc));

                    theArc = new Semicircle(tan.theCircle, chord.Point1, chord.Point2, opp, chord);
                    newGrounded.Add(CreateClause(inter, original, theAngle, theArc));

                }
            }
            else
            {
                Arc theArc = new MinorArc(tan.theCircle, chord.Point1, chord.Point2);

                // Angle; the smaller angle is always the chosen angle
                Point endPnt = (inter.intersect.StructurallyEquals(tanSegment.Point1)) ? tanSegment.Point2 : tanSegment.Point1;
                Angle theAngle = new Angle(chord.OtherPoint(inter.intersect), inter.intersect, endPnt);
                
                if (theAngle.measure > 90)
                {
                    //If the angle endpoint was already set to Point2, or if the intersect equals Point2, then the smaller angle does not exist
                    //In this case, should we create a major arc or return nothing?
                    if (endPnt.StructurallyEquals(tanSegment.Point2) || inter.intersect.StructurallyEquals(tanSegment.Point2)) return newGrounded;
                    theAngle = new Angle(chord.OtherPoint(inter.intersect), inter.intersect, tanSegment.Point2);
                }

                Multiplication product = new Multiplication(new NumericValue(2), theAngle);
                GeometricAngleArcEquation angArcEq = new GeometricAngleArcEquation(product, theArc);

                // For hypergraph
                List<GroundedClause> antecedent = new List<GroundedClause>();
                antecedent.Add(original);
                antecedent.Add(inter);
                antecedent.Add(theArc);
                antecedent.Add(theAngle);

                newGrounded.Add(new EdgeAggregator(antecedent, angArcEq, annotation));
            }


            


            return newGrounded;
        }

        private static EdgeAggregator CreateClause(Intersection inter, GroundedClause original, Angle theAngle, Arc theArc)
        {
            Multiplication product = new Multiplication(new NumericValue(2), theAngle);
            GeometricAngleArcEquation angArcEq = new GeometricAngleArcEquation(product, theArc);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(inter);

            return new EdgeAggregator(antecedent, angArcEq, annotation);
        }
    }
}