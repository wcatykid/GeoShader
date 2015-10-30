using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class MajorArc : Arc
    {
        public MajorArc(Circle circle, Point e1, Point e2) : this(circle, e1, e2, new List<Point>(), new List<Point>()) { }

        public MajorArc(Circle circle, Point e1, Point e2, List<Point> minorPts, List<Point> majorPts) : base(circle, e1, e2, minorPts, majorPts)
        {
            if (circle.DefinesDiameter(new Segment(e1, e2)))
            {
                System.Diagnostics.Debug.WriteLine("Major Arc should not be constructed when a semicircle is appropriate.");
            }
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            MajorArc arc = obj as MajorArc;
            if (arc == null) return false;

            return this.theCircle.StructurallyEquals(arc.theCircle) && ((this.endpoint1.StructurallyEquals(arc.endpoint1)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint2))
                                                                    || (this.endpoint1.StructurallyEquals(arc.endpoint2)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint1)));
        }

        public override Point Midpoint()
        {
            return theCircle.OppositePoint(theCircle.Midpoint(endpoint1, endpoint2));
        }

        public override bool CoordinateCongruent(Figure that)
        {
            MajorArc thatArc = that as MajorArc;
            if (thatArc == null) return false;

            if (!theCircle.CoordinateCongruent(thatArc.theCircle)) return false;

            return Utilities.CompareValues(this.GetMajorArcMeasureDegrees(), thatArc.GetMajorArcMeasureDegrees());
        }

        private void GetStartEndPoints(double angle1, double angle2, out Point start, out Point end, out double angle)
        {
            start = null;
            end = null;
            angle = -1;

            if (angle2 - angle1 > 0 && angle2 - angle1 >= Angle.toRadians(180))
            {
                start = endpoint1;
                end = endpoint2;
                angle = angle1;
            }
            else if (angle1 - angle2 > 0 && angle1 - angle2 >= Angle.toRadians(180))
            {
                start = endpoint2;
                end = endpoint1;
                angle = angle2;
            }
            else if (angle2 - angle1 > 0 && angle2 - angle1 < Angle.toRadians(180))
            {
                start = endpoint2;
                end = endpoint1;
                angle = angle2;
            }
            else if (angle1 - angle2 > 0 && angle1 - angle2 < Angle.toRadians(180))
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
            double angleIncrement = this.GetMajorArcMeasureRadians() / Figure.NUM_SEGS_TO_APPROX_ARC;

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
            return Arc.BetweenMajor(pt, this);
        }

        public override bool PointLiesStrictlyOn(Point pt)
        {
            return Arc.StrictlyBetweenMajor(pt, this);
        }

        public override bool HasSubArc(Arc that)
        {
            if (!this.theCircle.StructurallyEquals(that.theCircle)) return false;

            if (that is MajorArc) return this.HasMajorSubArc(that);
            if (that is Semicircle)
            {
                Semicircle semi = that as Semicircle;
                return this.HasMinorSubArc(new MinorArc(semi.theCircle, semi.endpoint1, semi.middlePoint)) &&
                       this.HasMinorSubArc(new MinorArc(semi.theCircle, semi.endpoint2, semi.middlePoint));
            }

            return this.HasMinorSubArc(that);
        }

        public override bool Equals(Object obj)
        {
            MajorArc arc = obj as MajorArc;
            if (arc == null) return false;

            // Check equality of MajorArc Major / major points?

            return base.Equals(obj);
        }

        public override string ToString() { return "MajorArc(" + theCircle + "(" + endpoint1.ToString() + ", " + endpoint2.ToString() + "))"; }
    }
}