using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class SegmentEquation : Equation
    {
        public SegmentEquation() : base() { }
        public SegmentEquation(GroundedClause l, GroundedClause r) : base(l, r)
        {
            double sumL = SumSide(l.CollectTerms());
            double sumR = SumSide(r.CollectTerms());

            //if (!Utilities.CompareValues(sumL, sumR))
            //{
            //    throw new ArgumentException("Segment equation is inaccurate; sums differ: " + l + " = " + r);
            //}
            if (Utilities.CompareValues(sumL, 0) && Utilities.CompareValues(sumR, 0))
            {
                throw new ArgumentException("Should not have an equation that is 0 = 0: " + this.ToString());
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
                else if (clause is Segment)
                {
                    sum += clause.multiplier * (clause as Segment).Length;
                }
            }
            return sum;
        }

        //public SegmentEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just)
        //{
        //    double sumL = SumSide(l.CollectTerms());
        //    double sumR = SumSide(r.CollectTerms());

        //    if (!Utilities.CompareValues(sumL, sumR))
        //    {
        //        throw new ArgumentException("Segment equation is inaccurate; sums differ: " + l + " " + r);
        //    }
        //    if (sumL == 0 && sumR == 0)
        //    {
        //        throw new ArgumentException("Should not have an equation that is 0 = 0: " + this.ToString());
        //    }
        //}

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            SegmentEquation eq = obj as SegmentEquation;
            if (eq == null) return false;
            return base.Equals(obj);
        }
    }
}