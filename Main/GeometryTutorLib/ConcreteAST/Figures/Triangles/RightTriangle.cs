using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a right triangle, FOR PRECOMPTUATION purposes only now.
    /// </summary>
    public partial class RightTriangle : Triangle
    {
        public RightTriangle(Segment a, Segment b, Segment c) : base(a, b, c)
        {
            provenRight = true;

            rightAngle = Utilities.CompareValues(AngleA.measure, 90) ? AngleA : rightAngle;
            rightAngle = Utilities.CompareValues(AngleB.measure, 90) ? AngleB : rightAngle;
            rightAngle = Utilities.CompareValues(AngleC.measure, 90) ? AngleC : rightAngle;
        }

        public RightTriangle(Triangle t) : base(t.SegmentA, t.SegmentB, t.SegmentC)
        {
            provenRight = true;

            rightAngle = Utilities.CompareValues(AngleA.measure, 90) ? AngleA : rightAngle;
            rightAngle = Utilities.CompareValues(AngleB.measure, 90) ? AngleB : rightAngle;
            rightAngle = Utilities.CompareValues(AngleC.measure, 90) ? AngleC : rightAngle;
        }

        public override bool IsStrongerThan(Polygon that)
        {
            if (that is EquilateralTriangle) return false;
            if (that is IsoscelesTriangle) return false;
            if (that is RightTriangle) return false;
            if (that is Triangle) return true;

            return false;
        }

        //
        // Is this triangle encased in the given, larger triangle.
        // That is, two sides are defined by the larger triangle and one side by the altitude
        //
        public bool IsDefinedBy(RightTriangle rt, Altitude altitude)
        {
            // The opposite side of the smaller triangle has to be a side of the larger triangle.
            if (!rt.HasSegment(this.GetOppositeSide(this.rightAngle))) return false;

            // The smaller triangle's right angle must have a ray collinear with the altitude segment
            Segment altSegment = null;
            if (this.rightAngle.ray1.IsCollinearWith(altitude.segment))
            {
                altSegment = this.rightAngle.ray1;
            }
            else if (this.rightAngle.ray2.IsCollinearWith(altitude.segment))
            {
                altSegment = this.rightAngle.ray2;
            }
            if (altSegment == null) return false;

            // The last segment needs to be collinear with a side of the larger triangle
            return rt.LiesOn(this.rightAngle.OtherRayEquates(altSegment));
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            RightTriangle triangle = obj as RightTriangle;
            if (triangle == null) return false;
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "RightTriangle(" + Point1.ToString() + ", " + Point2.ToString() + ", " + Point3.ToString() + ")";
        }
    }
}
