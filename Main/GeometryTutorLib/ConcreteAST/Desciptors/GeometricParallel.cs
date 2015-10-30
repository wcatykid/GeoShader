using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricParallel : Parallel
    {
        public GeometricParallel(Segment segment1, Segment segment2) : base(segment1, segment2) {}

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            GeometricParallel gp = obj as GeometricParallel;
            if (gp == null) return false;
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "GeometricParallel(" + segment1.ToString() + ", " + segment2.ToString() + ") " + justification;
        }

        public override string ToPrettyString()
        {
            return segment1.ToPrettyString() + " is parallel to " + segment2.ToPrettyString() + ".";
        }

    }
}
