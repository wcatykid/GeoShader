using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicSegmentEquation : SegmentEquation
    {
        public AlgebraicSegmentEquation() : base() { }

        public AlgebraicSegmentEquation(GroundedClause l, GroundedClause r) : base(l, r) { }
        // public AlgebraicSegmentEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just) { }

        public override GroundedClause DeepCopy()
        {
            return new AlgebraicSegmentEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "AlgebraicEquation(" + lhs + " = " + rhs + "): " + justification;
        }
    }
}