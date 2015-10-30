using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicAngleArcEquation : AngleArcEquation
    {
        public AlgebraicAngleArcEquation() : base() { }

        public AlgebraicAngleArcEquation(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override GroundedClause DeepCopy()
        {
            return new AlgebraicAngleArcEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
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
