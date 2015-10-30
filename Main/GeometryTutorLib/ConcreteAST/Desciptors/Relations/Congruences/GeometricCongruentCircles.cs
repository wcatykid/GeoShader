using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricCongruentCircles : CongruentCircles
    {
        public GeometricCongruentCircles(Circle c1, Circle c2) : base(c1, c2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }


        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "GeometricCongruent(" + cc1.ToString() + ", " + cc2.ToString() + ") " + justification; }
    }
}
