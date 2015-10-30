using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricArcEquation : ArcEquation
    {
        public GeometricArcEquation() : base() { }

        public GeometricArcEquation(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override GroundedClause DeepCopy()
        {
            return new GeometricArcEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "GeometricEquation(" + lhs + " = " + rhs + "): " + justification;
        }
    }
}
