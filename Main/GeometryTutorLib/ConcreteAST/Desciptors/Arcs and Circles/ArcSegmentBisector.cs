using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class ArcSegmentBisector : Bisector
    {
        public CircleSegmentIntersection bisected { get; private set; }

        public ArcSegmentBisector(CircleSegmentIntersection b) : base()
        {
            bisected = b;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            ArcSegmentBisector ab = obj as ArcSegmentBisector;
            if (ab == null) return false;
            return bisected.StructurallyEquals(ab.bisected);
        }

        public override bool Equals(Object obj)
        {
            ArcSegmentBisector ab = obj as ArcSegmentBisector;
            if (ab == null) return false;
            return bisected.Equals(ab.bisected);
        }

        public override string ToString()
        {
            return "ArcSegmentBisector(" + bisected.ToString() + ") at " + bisected.intersect + ")";
        }
    }
}
