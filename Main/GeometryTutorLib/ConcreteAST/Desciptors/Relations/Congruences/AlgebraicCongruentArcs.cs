using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicCongruentArcs : CongruentArcs
    {
        public AlgebraicCongruentArcs(Arc c1, Arc c2) : base(c1, c2) { }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "AlgebraicCongruent(" + ca1.ToString() + ", " + ca2.ToString() + ") " + justification; }
    }
}
