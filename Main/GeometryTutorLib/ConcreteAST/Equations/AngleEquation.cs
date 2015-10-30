using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class AngleEquation : Equation
    {
        public AngleEquation() : base() { }

        public AngleEquation(GroundedClause l, GroundedClause r) : base(l, r)
        {
            double sumL = SumSide(l.CollectTerms());
            double sumR = SumSide(r.CollectTerms());

            if (!Utilities.CompareValues(sumL, sumR))
            {
                throw new ArgumentException("Angle equation is inaccurate; sums differ: " + l + " " + r);
            }
        }

        private double SumSide(List<GroundedClause> side)
        {
            double sum = 0;
            foreach (GroundedClause clause in side)
            {
                if (clause is NumericValue)
                {
                    sum += (clause as NumericValue).DoubleValue;
                }
                else if (clause is Angle)
                {
                    sum += clause.multiplier * (clause as Angle).measure;
                }
                else if (clause is MinorArc)
                {
                    sum += clause.multiplier * (clause as MinorArc).GetMinorArcMeasureDegrees();
                }
                else if (clause is MajorArc)
                {
                    sum += clause.multiplier * (clause as MajorArc).GetMajorArcMeasureDegrees();
                }
            }
            return sum;
        }

        //public AngleEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just)
        //{
        //    double sumL = SumSide(l.CollectTerms());
        //    double sumR = SumSide(r.CollectTerms());

        //    if (!Utilities.CompareValues(sumL, sumR))
        //    {
        //        throw new ArgumentException("Angle equation is inaccurate; sums differ: " + l + " " + r);
        //    }

        //    //if (sumL == 0 && sumR == 0)
        //    //{
        //    //    throw new ArgumentException("Should not have an equation that is 0 = 0: " + this.ToString());
        //    //}
        //}

        public override int GetHashCode() { return base.GetHashCode(); }

        //
        // Equals checks that for both sides of this equation is the same as one entire side of the other equation
        //
        public override bool Equals(object target)
        {
            AngleEquation thatEquation = target as AngleEquation;

            if (thatEquation == null) return false;

            //
            // Collect all basic terms on the left and right hand sides of both equations.
            //
            List<GroundedClause> thisLHS = lhs.CollectTerms();
            List<GroundedClause> thisRHS = rhs.CollectTerms();

            List<GroundedClause> thatLHS = thatEquation.lhs.CollectTerms();
            List<GroundedClause> thatRHS = thatEquation.rhs.CollectTerms();

            // Check side length counts as a first step.
            if (!((thisLHS.Count == thatLHS.Count && thisRHS.Count == thatRHS.Count) ||
                (thisLHS.Count == thatRHS.Count && thisLHS.Count == thatLHS.Count))) return false;

            // Seek one side equal to one side and then the other equals the other.
            // Cannot do this easily with a union / set interection set theoretic approach since an equation may have multiple instances of a value
            // In theory, since we always deal with simplified equations, there should not be multiple instances of a particular value.
            // So, this should work.

            // Note operations like multiplication and substraction have been taken into account.
            List<GroundedClause> unionLHS = new List<GroundedClause>(thisLHS);
            Utilities.AddUniqueList(unionLHS, thatLHS);

            List<GroundedClause> unionRHS = new List<GroundedClause>(thisRHS);
            Utilities.AddUniqueList(unionRHS, thatRHS);

            // Exact same sides means the union is the same as each list itself
            if (unionLHS.Count == thisLHS.Count && unionRHS.Count == thisRHS.Count) return true;

            // Check the other combination of sides
            unionLHS = new List<GroundedClause>(thisLHS);
            Utilities.AddUniqueList(unionLHS, thatRHS);

            if (unionLHS.Count != thisLHS.Count) return false;

            unionRHS = new List<GroundedClause>(thisRHS);
            Utilities.AddUniqueList(unionRHS, thatLHS);

            // Exact same sides means the union is the same as each list itself
            return unionRHS.Count == thisRHS.Count;
        }
    }
}