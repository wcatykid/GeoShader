using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricCongruentTriangles : CongruentTriangles
    {
        public GeometricCongruentTriangles(Triangle t1, Triangle t2) : base(t1, t2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "GeometricCongruent(" + ct1.ToString() + ", " + ct2.ToString() + ") " + justification; }

        public override string ToPrettyString() { return ct1.ToPrettyString() + "is congruent to " + ct2.ToPrettyString() + "."; }
    }
}
