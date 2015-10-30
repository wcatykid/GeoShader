using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class EquilateralTriangle : IsoscelesTriangle
    {
        /// <summary>
        /// Create a new equilateral triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public EquilateralTriangle(Segment a, Segment b, Segment c) : base(a, b, c)
        {
            provenIsosceles = true;
            provenEquilateral = true;
        }
        public EquilateralTriangle(Triangle t) : base(t.SegmentA, t.SegmentB, t.SegmentC)
        {
            if (!Utilities.CompareValues(t.SegmentA.Length, t.SegmentB.Length))
            {
                throw new ArgumentException("Equilateral Triangle constructed with non-congruent segments " + t.SegmentA.ToString() + " " + t.SegmentB.ToString());
            }

            if (!Utilities.CompareValues(t.SegmentA.Length, t.SegmentC.Length))
            {
                throw new ArgumentException("Equilateral Triangle constructed with non-congruent segments " + t.SegmentA.ToString() + " " + t.SegmentC.ToString());
            }

            if (!Utilities.CompareValues(t.SegmentB.Length, t.SegmentC.Length))
            {
                throw new ArgumentException("Equilateral Triangle constructed with non-congruent segments " + t.SegmentB.ToString() + " " + t.SegmentC.ToString());
            }

            provenIsosceles = true;
            provenEquilateral = true;

            // Need to capture all owners as well; if a triangle is strengthened.
            this.AddAtomicRegions(t.atoms);
            this.GetFigureAsAtomicRegion().AddOwners(t.GetFigureAsAtomicRegion().owners);
        }
        public EquilateralTriangle(List<Segment> segs) : this(segs[0], segs[1], segs[2])
        {
            if (segs.Count != 3) throw new ArgumentException("Equilateral Triangle constructed with " + segs.Count + " segments.");
        }

        public override bool IsStrongerThan(Polygon that)
        {
            if (that is EquilateralTriangle) return false;
            if (that is IsoscelesTriangle) return true;
            if (that is RightTriangle) return false;
            if (that is Triangle)
            {
                Triangle tri = that as Triangle;

                if (tri.provenIsosceles) return true;
                if (tri.provenEquilateral) return false;
                if (tri.provenRight) return false;

                return true;
            }

            return false;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            EquilateralTriangle triangle = obj as EquilateralTriangle;
            if (triangle == null) return false;
            return base.Equals(obj);
        }
    }
}
