using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A calculator for aggregator of all known measurements for a particular problem (angles, segment lengths, etc.)
    /// </summary>
    public static class KnownValueAcquisition
    {
        public static KnownMeasurementsAggregator AcquireAllKnownValues(KnownMeasurementsAggregator known, List<GroundedClause> congsEqs, List<GroundedClause> triangles)
        {
            //
            // Fixed-point acquisition of values using congruences and equations.
            //
            bool change = true;
            while(change)
            {
                change = AcquireCongruences(known, congsEqs);

                // Pythagorean Theorem
                change = AcquireViaTriangles(known, triangles) || change;

                change = AcquireViaEquations(known, congsEqs) || change;
            }

            return known;
        }

        public static bool AcquireCongruences(KnownMeasurementsAggregator known, List<GroundedClause> clauses)
        {
            bool addedKnown = false;

            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in clauses)
            {
                GeometryTutorLib.ConcreteAST.CongruentSegments cs = clause as GeometryTutorLib.ConcreteAST.CongruentSegments;
                if (cs != null && !cs.IsReflexive())
                {
                    double length1 = known.GetSegmentLength(cs.cs1);
                    double length2 = known.GetSegmentLength(cs.cs2);

                    if (length1 >= 0 && length2 < 0)
                    {
                        if (known.AddSegmentLength(cs.cs2, length1)) addedKnown = true;
                    }
                    if (length1 <= 0 && length2 > 0)
                    {
                        if (known.AddSegmentLength(cs.cs1, length2)) addedKnown = true;
                    }
                    // else: both known
                }

                GeometryTutorLib.ConcreteAST.CongruentAngles cas = clause as GeometryTutorLib.ConcreteAST.CongruentAngles;
                if (cas != null && !cas.IsReflexive())
                {
                    double measure1 = known.GetAngleMeasure(cas.ca1);
                    double measure2 = known.GetAngleMeasure(cas.ca2);

                    if (measure1 >= 0 && measure2 < 0)
                    {
                        if (known.AddAngleMeasureDegree(cas.ca2, measure1)) addedKnown = true;
                    }
                    if (measure1 <= 0 && measure2 > 0)
                    {
                        if (known.AddAngleMeasureDegree(cas.ca1, measure2)) addedKnown = true;
                    }
                    // else: both known
                }
            }

            return addedKnown;
        }

        //
        // A right triangle means we can apply the pythagorean theorem to acquire an unknown.
        //
        public static bool HandleTriangle(KnownMeasurementsAggregator known, Triangle tri)
        {
            if (tri == null) return false; 

            KeyValuePair<Segment, double> pair = tri.PythagoreanTheoremApplies(known);

            if (pair.Value > 0)
            {
                // Do we know this already?
                if (known.GetSegmentLength(pair.Key) > 0) return false;

                // We don't know it, we add it.
                known.AddSegmentLength(pair.Key, pair.Value);
                return true;
            }
            else
            {
                if (AddKnowns(known, tri.IsoscelesRightApplies(known))) return true;
                if (AddKnowns(known, tri.CalculateBaseOfIsosceles(known))) return true;
                if (AddKnowns(known, tri.RightTriangleTrigApplies(known))) return true;
            }

            return false;
        }
        private static bool AddKnowns(KnownMeasurementsAggregator known, List<KeyValuePair<Segment, double>> pairs)
        {
            if (!pairs.Any()) return false;

            bool change = false;
            foreach (KeyValuePair<Segment, double> rightPair in pairs)
            {
                // Do we know this already?
                if (known.GetSegmentLength(rightPair.Key) < 0)
                {
                    change = true;
                    known.AddSegmentLength(rightPair.Key, rightPair.Value);
                }
            }
            return change;
        }

        private static bool AcquireViaTriangles(KnownMeasurementsAggregator known, List<GroundedClause> triangles)
        {
            bool addedKnown = false;

            foreach (GroundedClause clause in triangles)
            {
                if (clause is Triangle)
                {
                    addedKnown = HandleTriangle(known, clause as Triangle) || addedKnown;
                }
                else if (clause is Strengthened)
                {
                    Strengthened streng = clause as Strengthened;
                    if (streng.strengthened is Triangle)
                    {
                        addedKnown = HandleTriangle(known, streng.strengthened as Triangle) || addedKnown;
                    }
                }
            }

            return addedKnown;
        }

        //
        // Check all equations to see if we can substitute into any.
        //
        public static bool AcquireViaEquations(KnownMeasurementsAggregator known, List<GroundedClause> clauses)
        {
            bool addedKnown = false;

            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in clauses)
            {
                if (clause is GeometryTutorLib.ConcreteAST.Equation)
                {
                    if (HandleEquation(known, clauses, clause as Equation)) addedKnown = true;
                }
                else if (clause is GeometryTutorLib.ConcreteAST.SegmentRatioEquation)
                {
                    if (HandleRatioEquation(known, clause as SegmentRatioEquation)) addedKnown = true;
                }
            }

            return addedKnown;
        }


        //
        // (1) Make a copy
        // (2) Collect the equation terms.
        // (3) Are all but one known?
        // (4) Substitute
        // (5) Simplify
        // (6) Acquire the unknown and its value.
        // (7) Add to the list of knowns.
        //
        public static bool HandleEquation(KnownMeasurementsAggregator known, List<GroundedClause> clauses, Equation theEq)
        {
            if (theEq is AngleEquation) return HandleAngleEquation(known, clauses, theEq as AngleEquation);
            if (theEq is SegmentEquation) return HandleSegmentEquation(known, clauses, theEq as SegmentEquation);
            if (theEq is ArcEquation) return HandleArcEquation(known, clauses, theEq as ArcEquation);

            return false;
        }

        //
        // (1) Make a copy
        // (2) Collect the equation terms.
        // (3) Are all but one known?
        // (4) Substitute
        // (5) Simplify
        // (6) Acquire the unknown and its value.
        // (7) Add to the list of knowns.
        //
        private static bool HandleAngleEquation(KnownMeasurementsAggregator known, List<GroundedClause> clauses, AngleEquation theEq)
        {
            if (theEq.GetAtomicity() == Equation.BOTH_ATOMIC) return HandleSimpleAngleEquation(known, theEq);

            // CTA: Verify this calls the correct Equation deep copy mechanism.
            // (1) Make a copy
            AngleEquation copy = (AngleEquation)theEq.DeepCopy();

            // (2) Collect the equation terms.
            List<GroundedClause> left = copy.lhs.CollectTerms();
            double[] leftVal = new double[left.Count];
            List<GroundedClause> right = copy.rhs.CollectTerms();
            double[] rightVal = new double[right.Count];

            // (3) Are all but one term known?
            int unknownCount = 0;
            for (int ell = 0; ell < left.Count; ell++)
            {
                if (left[ell] is NumericValue) leftVal[ell] = (left[ell] as NumericValue).DoubleValue;
                else
                {
                    leftVal[ell] = known.GetAngleMeasure(left[ell] as Angle);
                    if (leftVal[ell] <= 0) unknownCount++;
                }
            }
            for (int r = 0; r < right.Count; r++)
            {
                if (right[r] is NumericValue) rightVal[r] = (right[r] as NumericValue).DoubleValue;
                else
                {
                    rightVal[r] = known.GetAngleMeasure(right[r] as Angle);
                    if (rightVal[r] <= 0) unknownCount++;
                }
            }

            // We can't solve for more or less than one unknown.
            if (unknownCount != 1) return false;

            //
            // (4) Substitute
            //
            for (int ell = 0; ell < left.Count; ell++)
            {
                if (leftVal[ell] > 0) copy.Substitute(left[ell], new NumericValue(leftVal[ell]));
            }
            for (int r = 0; r < right.Count; r++)
            {
                if (rightVal[r] > 0) copy.Substitute(right[r], new NumericValue(rightVal[r]));
            }

            //
            // (5) Simplify
            //
            AngleEquation simplified = (AngleEquation)GenericInstantiator.Simplification.Simplify(copy);

            return HandleSimpleAngleEquation(known, simplified);
        }

        private static bool HandleSimpleAngleEquation(KnownMeasurementsAggregator known, AngleEquation theEq)
        {
            if (theEq.GetAtomicity() != Equation.BOTH_ATOMIC) return false;

            Angle unknownAngle = null;
            double angleValue = -1;
            if (theEq.lhs is NumericValue)
            {
                unknownAngle = theEq.rhs as Angle;
                angleValue = (theEq.lhs as NumericValue).DoubleValue;
            }
            else if (theEq.rhs is NumericValue)
            {
                unknownAngle = theEq.lhs as Angle;
                angleValue = (theEq.rhs as NumericValue).DoubleValue;
            }
            else return false;

            //
            // (7) Add to the list of knowns
            //
            return known.AddAngleMeasureDegree(unknownAngle, angleValue);
        }

        //
        // (1) Make a copy
        // (2) Collect the equation terms.
        // (3) Are all but one known?
        // (4) Substitute
        // (5) Simplify
        // (6) Acquire the unknown and its value.
        // (7) Add to the list of knowns.
        //
        private static bool HandleArcEquation(KnownMeasurementsAggregator known, List<GroundedClause> clauses, ArcEquation theEq)
        {
            if (theEq.GetAtomicity() == Equation.BOTH_ATOMIC) return HandleSimpleArcEquation(known, theEq);

            // CTA: Verify this calls the correct Equation deep copy mechanism.
            // (1) Make a copy
            ArcEquation copy = (ArcEquation)theEq.DeepCopy();

            // (2) Collect the equation terms.
            List<GroundedClause> left = copy.lhs.CollectTerms();
            double[] leftVal = new double[left.Count];
            List<GroundedClause> right = copy.rhs.CollectTerms();
            double[] rightVal = new double[right.Count];

            // (3) Are all but one term known?
            int unknownCount = 0;
            for (int ell = 0; ell < left.Count; ell++)
            {
                if (left[ell] is NumericValue) leftVal[ell] = (left[ell] as NumericValue).DoubleValue;
                else
                {
                    if (left[ell] is Angle)
                    {
                        leftVal[ell] = known.GetAngleMeasure(left[ell] as Angle);
                        if (leftVal[ell] <= 0) unknownCount++;
                    }
                    else if (left[ell] is Arc)
                    {
                        leftVal[ell] = known.GetArcMeasure(left[ell] as Arc);
                        if (leftVal[ell] <= 0) unknownCount++;
                    }
                }
            }
            for (int r = 0; r < right.Count; r++)
            {
                if (right[r] is NumericValue) rightVal[r] = (right[r] as NumericValue).DoubleValue;
                else
                {
                    if (right[r] is Angle)
                    {
                        rightVal[r] = known.GetAngleMeasure(right[r] as Angle);
                        if (rightVal[r] <= 0) unknownCount++;
                    }
                    else if (right[r] is Arc)
                    {
                        rightVal[r] = known.GetArcMeasure(right[r] as Arc);
                        if (rightVal[r] <= 0) unknownCount++;
                    }
                }
            }

            // We can't solve for more or less than one unknown.
            if (unknownCount != 1) return false;

            //
            // (4) Substitute
            //
            for (int ell = 0; ell < left.Count; ell++)
            {
                if (leftVal[ell] > 0) copy.Substitute(left[ell], new NumericValue(leftVal[ell]));
            }
            for (int r = 0; r < right.Count; r++)
            {
                if (rightVal[r] > 0) copy.Substitute(right[r], new NumericValue(rightVal[r]));
            }

            //
            // (5) Simplify
            //
            ArcEquation simplified = (ArcEquation)GenericInstantiator.Simplification.Simplify(copy);

            return HandleSimpleArcEquation(known, simplified);
        }

        private static bool HandleSimpleArcEquation(KnownMeasurementsAggregator known, ArcEquation theEq)
        {
            if (theEq.GetAtomicity() != Equation.BOTH_ATOMIC) return false;

            Arc unknownArc = null;
            double measure = -1;
            if (theEq.lhs is NumericValue)
            {
                unknownArc = theEq.rhs as Arc;
                measure = (theEq.lhs as NumericValue).DoubleValue;
            }
            else if (theEq.rhs is NumericValue)
            {
                unknownArc = theEq.lhs as Arc;
                measure = (theEq.rhs as NumericValue).DoubleValue;
            }
            else return false;

            //
            // (7) Add to the list of knowns
            //
            return known.AddArcMeasureDegree(unknownArc, measure);
        }

        //
        // (1) Make a copy
        // (2) Collect the equation terms.
        // (3) Are all but one known?
        // (4) Substitute
        // (5) Simplify
        // (6) Acquire the unknown and its value.
        // (7) Add to the list of knowns.
        //
        private static bool HandleSegmentEquation(KnownMeasurementsAggregator known, List<GroundedClause> clauses, SegmentEquation theEq)
        {
            if (theEq.GetAtomicity() == Equation.BOTH_ATOMIC) return HandleSimpleSegmentEquation(known, theEq);

            // CTA: Verify this calls the correct Equation deep copy mechanism.
            // (1) Make a copy
            SegmentEquation copy = (SegmentEquation)theEq.DeepCopy();

            // (2) Collect the equation terms.
            List<GroundedClause> left = copy.lhs.CollectTerms();
            double[] leftVal = new double[left.Count];
            List<GroundedClause> right = copy.rhs.CollectTerms();
            double[] rightVal = new double[right.Count];

            // (3) Are all but one term known?
            int unknownCount = 0;
            for (int ell = 0; ell < left.Count; ell++)
            {
                if (left[ell] is NumericValue) leftVal[ell] = (left[ell] as NumericValue).DoubleValue;
                else
                {
                    leftVal[ell] = known.GetSegmentLength(left[ell] as Segment);
                    if (leftVal[ell] <= 0) unknownCount++;
                }
            }
            for (int r = 0; r < right.Count; r++)
            {
                if (right[r] is NumericValue) rightVal[r] = (right[r] as NumericValue).DoubleValue;
                else
                {
                    rightVal[r] = known.GetSegmentLength(right[r] as Segment);
                    if (rightVal[r] <= 0) unknownCount++;
                }
            }

            // We can't solve for more or less than one unknown.
            if (unknownCount != 1) return false;

            //
            // (4) Substitute
            //
            for (int ell = 0; ell < left.Count; ell++)
            {
                if (leftVal[ell] > 0) copy.Substitute(left[ell], new NumericValue(leftVal[ell]));
            }
            for (int r = 0; r < right.Count; r++)
            {
                if (rightVal[r] > 0) copy.Substitute(right[r], new NumericValue(rightVal[r]));
            }

            //
            // (5) Simplify
            //
            SegmentEquation simplified = (SegmentEquation)GenericInstantiator.Simplification.Simplify(copy);

            return HandleSimpleSegmentEquation(known, simplified);
        }

        private static bool HandleSimpleSegmentEquation(KnownMeasurementsAggregator known, SegmentEquation theEq)
        {
            if (theEq.GetAtomicity() != Equation.BOTH_ATOMIC) return false;

            Segment unknownSegment = null;
            double segmentValue = -1;
            if (theEq.lhs is NumericValue)
            {
                unknownSegment = theEq.rhs as Segment;
                segmentValue = (theEq.lhs as NumericValue).DoubleValue;
            }
            else if (theEq.rhs is NumericValue)
            {
                unknownSegment = theEq.lhs as Segment;
                segmentValue = (theEq.rhs as NumericValue).DoubleValue;
            }
            else return false;

            //
            // (7) Add to the list of knowns
            //
            return known.AddSegmentLength(unknownSegment, segmentValue);
        }

        private static bool HandleRatioEquation(KnownMeasurementsAggregator known, SegmentRatioEquation theEq)
        {
            double topLeft = known.GetSegmentLength(theEq.lhs.smallerSegment);
            double bottomLeft = known.GetSegmentLength(theEq.lhs.largerSegment);
            double topRight = known.GetSegmentLength(theEq.rhs.smallerSegment);
            double bottomRight = known.GetSegmentLength(theEq.rhs.largerSegment);

            int unknown = 0;
            if (topLeft <= 0) unknown++; 
            if (bottomLeft <= 0) unknown++;
            if (topRight <= 0) unknown++;
            if (bottomRight <= 0) unknown++;
            if (unknown != 1) return false;
            
            if (topLeft <= 0)
            {
                return known.AddSegmentLength(theEq.lhs.smallerSegment, (topRight / bottomRight) * bottomLeft);
            }
            else if (bottomLeft <= 0)
            {
                return known.AddSegmentLength(theEq.lhs.largerSegment, topLeft * (bottomRight / topRight));
            }
            else if (topRight <= 0)
            {
                return known.AddSegmentLength(theEq.rhs.smallerSegment, (topLeft / bottomLeft) * bottomRight);
            }
            else if (bottomRight <= 0)
            {
                return known.AddSegmentLength(theEq.rhs.largerSegment, topRight * (bottomLeft / topLeft));
            }
            else return false;
        }
    }
}
