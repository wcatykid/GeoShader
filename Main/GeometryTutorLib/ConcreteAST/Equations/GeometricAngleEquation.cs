using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricAngleEquation : AngleEquation
    {
        public GeometricAngleEquation() : base() { }

        public GeometricAngleEquation(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override GroundedClause DeepCopy()
        {
            return new GeometricAngleEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "GeometricEquation(" + lhs + " = " + rhs + "): " + justification;
        }
    }
}