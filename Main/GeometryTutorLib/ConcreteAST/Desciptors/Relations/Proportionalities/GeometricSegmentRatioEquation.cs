using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricSegmentRatioEquation : SegmentRatioEquation
    {
        public GeometricSegmentRatioEquation(SegmentRatio r1, SegmentRatio r2) : base(r1, r2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override bool Equals(Object obj)
        {
            GeometricSegmentRatioEquation gsr = obj as GeometricSegmentRatioEquation;
            if (gsr == null) return false;

            return base.Equals(gsr);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "GeometricProportionalEquation(" + lhs.ToString() + " = " + rhs.ToString() + ") ";
        }
    }
}
