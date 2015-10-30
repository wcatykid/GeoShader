using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class ArcEquation : Equation
    {
        public ArcEquation() : base() { }
        public ArcEquation(GroundedClause l, GroundedClause r) : base(l, r)
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
                else if (clause is Arc)
                {
                    sum += clause.multiplier * (clause as Arc).length;
                }
            }
            return sum;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            ArcEquation eq = obj as ArcEquation;
            if (eq == null) return false;
            return base.Equals(obj);
        }
    }
}
