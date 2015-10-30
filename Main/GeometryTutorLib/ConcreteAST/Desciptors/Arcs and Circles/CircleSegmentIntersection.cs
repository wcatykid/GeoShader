using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class CircleSegmentIntersection : CircleIntersection
    {
        public Segment segment { get; protected set; }
        private bool isTangent;

        public CircleSegmentIntersection(Point p, Circle circ, Segment thatSegment) : base(p, circ)
        {
            segment = thatSegment;
            isTangent = CalcTangency();

            // Find the intersection points
            Point pt1, pt2;
            theCircle.FindIntersection(segment, out pt1, out pt2);
            intersection1 = pt1;
            intersection2 = pt2;
        }

        //
        // If the segment intersects at a single point on the circle AND the given radius (perpendicular) is in the middle of this arc.
        //
        private bool CalcTangency()
        {
            //// Is the segment tangent to the circle?
            //Segment radius =
                
            return theCircle.IsTangent(segment) != null;

            //if (radius == null) return false;

            ////
            //// Is the radius inside the minor arc? That is, is the radius applicable to this arc or not?
            ////
            //Point ptOnCircle = radius.OtherPoint(theCircle.center);

            //return Arc.BetweenMinor(ptOnCircle, new MinorArc(theCircle, intersection1, intersection2));
        }

        //
        // If the segment intersects at a single point and does not pass through the arc.
        //
        public override bool IsTangent()
        {
            return isTangent;
        }

        //
        // Acquire the radii to the points of intersection
        //
        public void GetRadii(out Segment radius1, out Segment radius2)
        {
            radius1 = theCircle.GetRadius(new Segment(theCircle.center, intersection1));

            if (intersection2 == null) radius2 = null;
            else
            {
                radius2 = theCircle.GetRadius(new Segment(theCircle.center, intersection2));
            }
        }

        //
        // If the segment starts on this arc and extends outward.
        //
        public override bool StandsOn()
        {
            // Is one endpoint of the segment on the circle?
            return theCircle.PointLiesOn(segment.Point1) || theCircle.PointLiesOn(segment.Point2);
        }

        // Is Chord?
        public bool IsChord()
        {
            // Are both endpoints of the segment on the circle?
            return theCircle.PointLiesOn(segment.Point1) && theCircle.PointLiesOn(segment.Point2);
        }

        // If the segment / arc passes through this arc and extends outward.
        public override bool Crossing()
        {
            // Both endpoints are not on the arc.
            return (theCircle.PointIsExterior(segment.Point1) && theCircle.PointIsInterior(segment.Point2)) ||
                   (theCircle.PointIsExterior(segment.Point2) && theCircle.PointIsInterior(segment.Point1));
        }

        public bool HasSegment(Segment thatSegment)
        {
            return segment.HasSubSegment(thatSegment) && thatSegment.PointLiesOnAndBetweenEndpoints(intersect);
        }

        public override bool StructurallyEquals(Object obj)
        {
            CircleSegmentIntersection inter = obj as CircleSegmentIntersection;
            if (inter == null) return false;
            return this.segment.StructurallyEquals(inter.segment) &&
                   this.intersect.StructurallyEquals(inter.intersect) &&
                   this.theCircle.StructurallyEquals(inter.theCircle);
        }

        public override bool Equals(Object obj)
        {
            CircleSegmentIntersection inter = obj as CircleSegmentIntersection;
            if (inter == null) return false;
            return this.segment.Equals(inter.segment) &&
                   this.intersect.StructurallyEquals(inter.intersect) &&
                   this.theCircle.StructurallyEquals(inter.theCircle);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "CircleSegmentIntersection(" + intersect.ToString() + ", " + theCircle.ToString() + ", " + segment.ToString() + ") " + justification;
        }
    }
}