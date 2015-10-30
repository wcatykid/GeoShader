using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicParallel : Parallel
    {
        public AlgebraicParallel(Segment segment1, Segment segment2) : base(segment1, segment2) { }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override bool Equals(Object obj)
        {
            AlgebraicParallel gp = obj as AlgebraicParallel;
            if (gp == null) return false;
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "AlgebraicParallel(" + segment1.ToString() + ", " + segment2.ToString() + ") " + justification;
        }
    }
}
