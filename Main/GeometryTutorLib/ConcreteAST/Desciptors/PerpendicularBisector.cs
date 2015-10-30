using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class PerpendicularBisector : Perpendicular
    {
        public Segment bisector { get; private set; }

        //public PerpendicularBisector(Point i, Segment l, Segment bisector, string just) : base(i, l, bisector, just)
        //{
        //    this.bisector = bisector;
        //}
        public PerpendicularBisector(Intersection inter, Segment bisector) : base(inter)
        {
            this.bisector = bisector;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            PerpendicularBisector p = obj as PerpendicularBisector;
            if (p == null) return false;
            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            PerpendicularBisector p = obj as PerpendicularBisector;
            if (p == null) return false;

            return intersect.Equals(p.intersect) && lhs.Equals(p.lhs) && rhs.Equals(p.rhs);
        }

        public override string ToString()
        {
            return "PerpendicularBisector(" + bisector.ToString() + " Bisects(" + this.OtherSegment(bisector) + ") at " + this.intersect + ")";
        }
    }
}
