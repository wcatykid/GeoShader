using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricCongruentArcs : CongruentArcs
    {
        public GeometricCongruentArcs(Arc c1, Arc c2) : base(c1, c2) { }

        public override bool IsGeometric() { return true; }
        public override bool IsAlgebraic() { return false; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "GeometricCongruent(" + ca1.ToString() + ", " + ca2.ToString() + ") " + justification; }
    }
}
