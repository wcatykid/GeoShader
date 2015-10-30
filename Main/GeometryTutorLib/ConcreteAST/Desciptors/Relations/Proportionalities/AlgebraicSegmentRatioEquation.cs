using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicSegmentRatioEquation : SegmentRatioEquation
    {
        public AlgebraicSegmentRatioEquation(SegmentRatio r1, SegmentRatio r2) : base(r1, r2) { }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override bool Equals(Object obj)
        {
            AlgebraicSegmentRatioEquation asr = obj as AlgebraicSegmentRatioEquation;
            if (asr == null) return false;

            return base.Equals(asr);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "AlgebraicProportionalEquation(" + lhs.ToString() + " = " + rhs.ToString() + ") ";
        }
    }
}
