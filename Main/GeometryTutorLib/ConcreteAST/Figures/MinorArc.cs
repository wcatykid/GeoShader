using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class MinorArc : Arc
    {
        public MinorArc(Circle circle, Point e1, Point e2) : this(circle, e1, e2, new List<Point>(), new List<Point>()) { }

        public MinorArc(Circle circle, Point e1, Point e2, List<Point> minorPts, List<Point> majorPts) : base(circle, e1, e2, minorPts, majorPts) { }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            MinorArc arc = obj as MinorArc;
            if (arc == null) return false;

            return this.theCircle.StructurallyEquals(arc.theCircle) && ((this.endpoint1.StructurallyEquals(arc.endpoint1)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint2))
                                                                    || (this.endpoint1.StructurallyEquals(arc.endpoint2)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint1)));
        }

        public override Point Midpoint() { return theCircle.Midpoint(endpoint1, endpoint2); }

        public override bool CoordinateCongruent(Figure that)
        {
            MinorArc thatArc = that as MinorArc;
            if (thatArc == null) return false;

            if (!theCircle.CoordinateCongruent(thatArc.theCircle)) return false;

            return Utilities.CompareValues(this.GetMinorArcMeasureDegrees(), thatArc.GetMinorArcMeasureDegrees());
        }

        private void GetStartEndPoints(double angle1, double angle2, out Point start, out Point end, out double angle)
        {
            start = null;
            end = null;
            angle = -1;

            if (angle2 - angle1 > 0 && angle2 - angle1 < Angle.toRadians(180))
            {
                start = endpoint1;
                end = endpoint2;
                angle = angle1;
            }
            else if (angle1 - angle2 > 0 && angle1 - angle2 < Angle.toRadians(180))
            {
                start = endpoint2;
                end = endpoint1;
                angle = angle2;
            }
            else if (angle2 - angle1 > 0 && angle2 - angle1 >= Angle.toRadians(180))
            {
                start = endpoint2;
                end = endpoint1;
                angle = angle2;
            }
            else if (angle1 - angle2 > 0 && angle1 - angle2 >= Angle.toRadians(180))
            {
                start = endpoint1;
                end = endpoint2;
                angle = angle1;
            }
        }

        public override List<Segment> Segmentize()
        {
            if (approxSegments.Any()) return approxSegments;

            // How much we will change the angle measure as we create segments.
            double angleIncrement = Angle.toRadians(this.minorMeasure / Figure.NUM_SEGS_TO_APPROX_ARC);

            // Find the first point so we sweep in a counter-clockwise manner.
            double angle1 = Point.GetRadianStandardAngleWithCenter(theCircle.center, endpoint1);
            double angle2 = Point.GetRadianStandardAngleWithCenter(theCircle.center, endpoint2);

            Point firstPoint = null;
            Point secondPoint = null;
            double angle = -1;

            GetStartEndPoints(angle1, angle2, out firstPoint, out secondPoint, out angle);

            for (int i = 1; i <= Figure.NUM_SEGS_TO_APPROX_ARC; i++)
            {
                // Save this as an approximating point.
                approxPoints.Add(firstPoint);

                // Get the next point.
                angle += angleIncrement;
                secondPoint = Point.GetPointFromAngle(theCircle.center, theCircle.radius, angle);

                // Make the segment.
                approxSegments.Add(new Segment(firstPoint, secondPoint));

                // Rotate points.
                firstPoint = secondPoint;
            }

            // Save this as an approximating point.
            approxPoints.Add(secondPoint);

            return approxSegments;
        }

        public override bool PointLiesOn(Point pt)
        {
            return Arc.BetweenMinor(pt, this);
        }

        public override bool PointLiesStrictlyOn(Point pt)
        {
            return Arc.StrictlyBetweenMinor(pt, this);
        }

        public override bool HasSubArc(Arc that)
        {
            if (!this.theCircle.StructurallyEquals(that.theCircle)) return false;

            if (that is MajorArc || that is Semicircle) return false;

            return this.HasMinorSubArc(that);
        }

        public override bool Equals(Object obj)
        {
            MinorArc arc = obj as MinorArc;
            if (arc == null) return false;

            // Check equality of arc minor / major points?

            return base.Equals(obj);
        }

        public override string ToString() { return "MinorArc(" + theCircle + "(" + endpoint1.ToString() + ", " + endpoint2.ToString() + "))"; }
    }
}