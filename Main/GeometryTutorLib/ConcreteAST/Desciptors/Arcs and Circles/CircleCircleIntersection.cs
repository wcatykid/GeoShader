using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class CircleCircleIntersection : CircleIntersection
    {
        public Circle otherCircle { get; protected set; }

        public CircleCircleIntersection(Point p, Circle c1, Circle c2) : base(p, c1)
        {
            otherCircle = c2;

            // Find the intersection points
            Point pt1, pt2;
            theCircle.FindIntersection(otherCircle, out pt1, out pt2);
            intersection1 = pt1;
            intersection2 = pt2;
        }

        //
        // If the arcs intersect at a single point.
        //
        public override bool IsTangent() { return intersection1 != null && intersection2 == null; }

        //
        // If the segment starts on this arc and extends outward.
        //
        public override bool StandsOn() { return false; }

        // If not tangent, circles pass through each other.
        public override bool Crossing() { return !IsTangent(); }

        public override bool StructurallyEquals(Object obj)
        {
            CircleCircleIntersection inter = obj as CircleCircleIntersection;
            if (inter == null) return false;
            return this.otherCircle.StructurallyEquals(inter.otherCircle) &&
                   this.intersect.StructurallyEquals(inter.intersect) &&
                   this.theCircle.StructurallyEquals(inter.theCircle);
        }

        public override bool Equals(Object obj)
        {
            CircleCircleIntersection inter = obj as CircleCircleIntersection;
            if (inter == null) return false;
            return this.otherCircle.Equals(inter.otherCircle) &&
                   this.intersect.Equals(inter.intersect) &&
                   this.theCircle.Equals(inter.theCircle);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "CircleCircleIntersection(" + intersect.ToString() + ", " + theCircle.ToString() + ", " + otherCircle.ToString() + ") " + justification;
        }
    }
}