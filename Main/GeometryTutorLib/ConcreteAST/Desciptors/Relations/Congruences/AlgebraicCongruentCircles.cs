using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicCongruentCircles : CongruentCircles
    {
        public AlgebraicCongruentCircles(Circle c1, Circle c2) : base(c1, c2) { }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "AlgebraicCongruent(" + cc1.ToString() + ", " + cc2.ToString() + ") " + justification; }
    }
}
