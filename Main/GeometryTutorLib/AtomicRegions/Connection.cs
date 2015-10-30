using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public enum ConnectionType { SEGMENT, ARC }

    //
    // Aggregation class for each segment of an atomic region.
    //
    public class Connection
    {
        public Point endpoint1;
        public Point endpoint2;
        public ConnectionType type;

        // The shape which has this connection. 
        public Figure segmentOrArc;

        public Connection(Point e1, Point e2, ConnectionType t, Figure so)
        {
            endpoint1 = e1;
            endpoint2 = e2;
            type = t;
            segmentOrArc = so;
        }

        public bool HasPoint(Point p) { return endpoint1.Equals(p) || endpoint2.Equals(p); }

        public Point OtherEndpoint(Point p)
        {
            if (endpoint1.StructurallyEquals(p)) return endpoint2;
            if (endpoint2.StructurallyEquals(p)) return endpoint1;
            return null;
        }

        public override string ToString()
        {
            return "< " + endpoint1.name + ", " + endpoint2.name + "(" + type + ") >";
        }

        public string CheapPrettyString()
        {
            return (type == ConnectionType.SEGMENT ? "Seg(" : "Arc(") + endpoint1.CheapPrettyString() + endpoint2.SimpleToString() + ")";
        }

        public bool StructurallyEquals(Connection that)
        {
            if (!this.HasPoint(that.endpoint1) || !this.HasPoint(that.endpoint2)) return false;

            if (type != that.type) return false;

            return segmentOrArc.StructurallyEquals(that.segmentOrArc);
        }

        //
        // Create an approximation of the arc by using a set number of arcs.
        //
        public List<Segment> Segmentize()
        {
            if (this.type == ConnectionType.SEGMENT) return Utilities.MakeList<Segment>(new Segment(this.endpoint1, this.endpoint2));

            return segmentOrArc.Segmentize();
        }

        public bool PointLiesOn(Point pt)
        {
            if (this.type == ConnectionType.SEGMENT)
            {
                return (this.segmentOrArc as Segment).PointLiesOnAndBetweenEndpoints(pt);
            }
            else if (this.type == ConnectionType.ARC)
            {
                return (segmentOrArc as Arc).PointLiesOn(pt);
            }

            return false;
        }

        public bool PointLiesStrictlyOn(Point pt)
        {
            if (this.HasPoint(pt)) return false;

            return PointLiesOn(pt);
        }

        public Point Midpoint()
        {
            if (this.type == ConnectionType.SEGMENT)
            {
                return (this.segmentOrArc as Segment).Midpoint();
            }
            else if (this.type == ConnectionType.ARC)
            {
                return (segmentOrArc as Arc).Midpoint();
            }

            return null;
        }

        //
        // Find the intersection points between this conenction and that; 2 points may result. (2 with arc / segment)
        //
        public void FindIntersection(List<Point> figurePoints, Connection that, out Point pt1, out Point pt2)
        {
            if (that.type == ConnectionType.ARC)
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Arc, out pt1, out pt2);
            }
            else
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Segment, out pt1, out pt2);
            }

            Segment thatSeg = that.segmentOrArc as Segment;
            Arc thatArc = that.segmentOrArc as Arc;

            Segment thisSeg = this.segmentOrArc as Segment;
            Arc thisArc = this.segmentOrArc as Arc;

            //
            // Normalize the points to the points in the drawing.
            //
            if (thisSeg != null && thatSeg != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thisSeg, thatSeg);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thisSeg, thatSeg);
            }
            else if (thisSeg != null && thatArc != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thisSeg, thatArc);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thisSeg, thatArc);
            }
            else if (thisArc != null && thatSeg != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thatSeg, thisArc);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thatSeg, thisArc);
            }
            else if (thisArc != null && thatArc != null)
            {
                pt1 = Utilities.AcquireRestrictedPoint(figurePoints, pt1, thisArc, thatArc);
                pt2 = Utilities.AcquireRestrictedPoint(figurePoints, pt2, thisArc, thatArc);
            }
        }
        public void FindIntersection(Connection that, out Point pt1, out Point pt2)
        {
            if (that.type == ConnectionType.ARC)
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Arc, out pt1, out pt2);
            }
            else
            {
                this.segmentOrArc.FindIntersection(that.segmentOrArc as Segment, out pt1, out pt2);
            }
        }

        //
        // If that is a segment which is smaller and is a subsegment of this.
        // If that is an arc which is smaller and is a subsrc of this.
        //
        public bool IsSubConnection(Connection that)
        {
            if (that.type != this.type) return false;

            if (this.type == ConnectionType.ARC)
            {
                return (this.segmentOrArc as Arc).HasSubArc(that.segmentOrArc as Arc);
            }
            else if (this.type == ConnectionType.SEGMENT)
            {
                return (this.segmentOrArc as Segment).HasSubSegment(that.segmentOrArc as Segment);
            }

            return false;
        }

        //
        // If one of the endpoints of that is inside of this; and vice versa.
        //
        public bool Overlap(Connection that)
        {
            if (that.type != this.type) return false;

            if (this.type == ConnectionType.ARC)
            {
                if (!(this.segmentOrArc as Arc).StructurallyEquals(this.segmentOrArc as Arc)) return false;

                // If the arcs just touch, it's not overlap.
                if (this.segmentOrArc is MinorArc)
                {
                    MinorArc minor = this.segmentOrArc as MinorArc;
                    if (minor.PointLiesStrictlyOn(that.endpoint1) || minor.PointLiesStrictlyOn(that.endpoint2)) return true;
                }
                else if (this.segmentOrArc is Semicircle)
                {
                    Semicircle semi = this.segmentOrArc as Semicircle;
                    if (semi.PointLiesStrictlyOn(that.endpoint1) || semi.PointLiesStrictlyOn(that.endpoint2)) return true;
                }
                else if (this.segmentOrArc is MajorArc)
                {
                    MajorArc major = this.segmentOrArc as MajorArc;
                    if (major.PointLiesStrictlyOn(that.endpoint1) || major.PointLiesStrictlyOn(that.endpoint2)) return true;
                }
                return false;
            }
            else if (this.type == ConnectionType.SEGMENT)
            {
                Segment thisSegment = this.segmentOrArc as Segment;
                Segment thatSegment = that.segmentOrArc as Segment;

                if (!thisSegment.IsCollinearWith(thatSegment)) return false;

                return thisSegment.CoincidingWithOverlap(thatSegment);
            }

            return false;
        }

        //
        // Does the segment or arc stand on the segment or arg? That is, the intersection point lies on the end of this or that?
        //
        public bool StandsOnNotEndpoint(Connection that)
        {
            if (StandsOnEndpoint(that)) return false;

            Point pt1 = null;
            Point pt2 = null;

            this.FindIntersection(that, out pt1, out pt2);

            if (pt2 != null) return false;

            if (this.HasPoint(pt1) && that.PointLiesOn(pt1)) return true;
            if (that.HasPoint(pt1) && this.PointLiesOn(pt1)) return true;

            return false;
        }

        public bool StandsOnEndpoint(Connection that)
        {
            Point pt1 = null;
            Point pt2 = null;

            this.FindIntersection(that, out pt1, out pt2);

            if (pt2 != null) return false;

            return this.HasPoint(pt1) && that.HasPoint(pt1);
        }

        //
        // Intersects at a single point (not at the endpoint of either connection).
        //
        public bool Crosses(Connection that)
        {
            if (this.type == that.type && this.type == ConnectionType.SEGMENT) return SegmentSegmentCrossing(that);
            else if (this.type == that.type && this.type == ConnectionType.ARC) return ArcArcCrossing(that);
            else if (this.type != that.type) return MixedCrossing(that);

            return false;
        }

        public bool SegmentSegmentCrossing(Connection that)
        {
            return (this.segmentOrArc as Segment).LooseCrosses(that.segmentOrArc as Segment);
        }

        public bool ArcArcCrossing(Connection that)
        {
            return (this.segmentOrArc as Arc).Crosses(that.segmentOrArc as Arc);
        }

        public bool MixedCrossing(Connection that)
        {
            Point pt1 = null;
            Point pt2 = null;
            this.FindIntersection(that, out pt1, out pt2);

            // Must intersect.
            if (pt1 == null && pt2 == null) return false;

            // If the endpoints align, this is not a crossing.
            if (this.HasPoint(that.endpoint1) && this.HasPoint(that.endpoint2)) return false;

            // A segment cuts through an arc in two points.
            if (this.type != that.type && pt1 != null && pt2 != null) return true;

            // Catch-all since we have the true cases for 2 intersections.
            if (pt2 != null) return false;

            // We only have one intersection point now: Point1
            // Check for the StandsOn relationship.
            return this.PointLiesStrictlyOn(pt1) && that.PointLiesStrictlyOn(pt1);
        }

        public bool DefinesArcSegmentRegion(Connection that)
        {
            if (this.type == that.type) return false;

            // Endpoints align.
            return this.HasPoint(that.endpoint1) && this.HasPoint(that.endpoint2);
        }
    }
}
