using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricAngleArcEquation : AngleArcEquation
    {
        public GeometricAngleArcEquation() : base() { }

        public GeometricAngleArcEquation(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override GroundedClause DeepCopy()
        {
            return new GeometricAngleArcEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "GeometricEquation(" + lhs + " = " + rhs + "): " + justification;
        }
    }
}
