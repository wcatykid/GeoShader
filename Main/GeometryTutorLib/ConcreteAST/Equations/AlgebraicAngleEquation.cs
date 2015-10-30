using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicAngleEquation : AngleEquation
    {
        public AlgebraicAngleEquation() : base() { }

        public AlgebraicAngleEquation(GroundedClause l, GroundedClause r) : base(l, r) { }
        //public AlgebraicAngleEquation(GroundedClause l, GroundedClause r, string just) : base(l, r, just) { }

        public override GroundedClause DeepCopy()
        {
            return new AlgebraicAngleEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
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