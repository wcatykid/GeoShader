using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Sector : Figure
    {
        public Arc theArc { get; protected set; }
        public Segment radius1 { get; protected set; }
        public Segment radius2 { get; protected set; }

        public Sector(Arc a)
        {
            theArc = a;
            radius1 = new Segment(theArc.theCircle.center, theArc.endpoint1);
            radius2 = new Segment(theArc.theCircle.center, theArc.endpoint2);

            thisAtomicRegion = new ShapeAtomicRegion(this);
        }

//        public override List<Point> GetApproximatingPoints() { return theArc.GetApproximatingPoints(); }

        public override Polygon GetPolygonalized()
        {
            if (polygonalized != null) return polygonalized;

            List<Segment> sides = new List<Segment>(theArc.GetApproximatingSegments());

            sides.Add(radius1);
            sides.Add(radius2);

            // Make a polygon out of the radii and the sector
            polygonalized = Polygon.MakePolygon(sides);

            return polygonalized;
        }

        /// <summary>
        /// Make a set of connections for atomic region analysis.
        /// </summary>
        /// <returns></returns>
        public override List<Connection> MakeAtomicConnections()
        {
            List<Connection> connections = new List<Connection>();

            connections.Add(new Connection(theArc.theCircle.center, theArc.endpoint1, ConnectionType.SEGMENT, radius1));
            connections.Add(new Connection(theArc.theCircle.center, theArc.endpoint2, ConnectionType.SEGMENT, radius2));

            connections.Add(new Connection(theArc.endpoint1, theArc.endpoint2, ConnectionType.ARC, this.theArc));

            return connections;
        }

        public override List<Segment> Segmentize()
        {
            List<Segment> segments = new List<Segment>();

            // Add radii
            segments.Add(radius1);
            segments.Add(radius2);

            // Segmentize the arc
            segments.AddRange(theArc.Segmentize());

            return segments;
        }

        //
        // Point must be in the given circle and then, specifically in the specified angle
        //
        public override bool PointLiesInside(Point pt)
        {
            // Is the point in the sector's circle?
            if (!theArc.theCircle.PointLiesInside(pt)) return false;

            // Radii
            if (radius1.PointLiesOnAndBetweenEndpoints(pt)) return false;
            if (radius2.PointLiesOnAndBetweenEndpoints(pt)) return false;

            //
            // For the Minor Arc, create two angles.
            // The sum must equal the measure of the angle created by the endpoints.
            //
            double originalMinorMeasure = theArc.minorMeasure;
            double centralAngle1 = new Angle(theArc.endpoint1, theArc.theCircle.center, pt).measure;
            double centralAngle2 = new Angle(theArc.endpoint2, theArc.theCircle.center, pt).measure;

            bool isInMinorArc = Utilities.CompareValues(theArc.minorMeasure, centralAngle1 + centralAngle2);

            if (theArc is MinorArc) return isInMinorArc;

            if (theArc is MajorArc) return !isInMinorArc;

            if (theArc is Semicircle)
            {
                Semicircle semi = theArc as Semicircle;

                // The point in question must lie on the same side of the diameter as the middle point
                Segment candSeg = new Segment(pt, semi.middlePoint);

                Point intersection = semi.diameter.FindIntersection(candSeg);

                return !candSeg.PointLiesOnAndBetweenEndpoints(intersection);
            }

            return false;
        }

        //
        // Point is on the perimeter?
        //
        public override bool PointLiesOn(Point pt)
        {
            if (pt == null) return false;

            // Radii
            KeyValuePair<Segment, Segment> radii = theArc.GetRadii();
            if (radii.Key.PointLiesOnAndBetweenEndpoints(pt) || radii.Value.PointLiesOnAndBetweenEndpoints(pt)) return true;

            // This point must lie on the circle in question, minimally.
            if (!theArc.theCircle.PointLiesOn(pt)) return false;

            // Arc
            if (theArc is MajorArc) return Arc.BetweenMajor(pt, theArc as MajorArc);
            else if (theArc is MinorArc) return Arc.BetweenMinor(pt, theArc as MinorArc);
            else if (theArc is Semicircle)
            {
                Semicircle semi = theArc as Semicircle;

                // The point in question must lie on the same side of the diameter as the middle point
                Segment candSeg = new Segment(pt, semi.middlePoint);

                Point intersection = semi.diameter.FindIntersection(candSeg);

                return !candSeg.PointLiesOnAndBetweenEndpoints(intersection);
            }

            return false;
        }

        public override bool PointLiesInOrOn(Point pt)
        {
            if (pt == null) return false;

            return PointLiesOn(pt) || PointLiesInside(pt);
        }

        //
        // The center lies inside the polygon and there are no intersection points with the sides.
        //
        private bool ContainsCircle(Circle that)
        {
            // Center lies (strictly) inside of this sector
            if (!this.PointLiesInside(that.center)) return false;
            
            // As a simple heuristic, the radii lengths must support inclusion.
            if (Point.calcDistance(this.theArc.theCircle.center, that.center) + that.radius > this.theArc.theCircle.radius) return false;

            //
            // Any intersections between the sides of the sector and the circle must be tangent.
            //
            Point pt1 = null;
            Point pt2 = null;
            that.FindIntersection(radius1, out pt1, out pt2);
            if (pt2 != null) return false;

            that.FindIntersection(radius2, out pt1, out pt2);
            if (pt2 != null) return false;

            that.FindIntersection(this.theArc, out pt1, out pt2);
            if (pt2 != null) return false;

            return true;
        }

        //
        // All points of the polygon are on or in the sector.
        // No need to check that any sides of the polygon pass
        // through the sector since that implies a vertex exterior to the sector.
        //
        private bool ContainsPolygon(Polygon that)
        {
            foreach (Point thatPt in that.points)
            {
                if (!this.PointLiesInOrOn(thatPt)) return false;
            }

            foreach (Segment side in that.orderedSides)
            {
                if (!this.PointLiesInOrOn(side.Midpoint())) return false;
            }

            return true;
        }

        //
        // that Sector lies within this sector
        //
        private bool ContainsSector(Sector that)
        {
            if (this.StructurallyEquals(that)) return true;

            //
            // Is this sector from the same circle as that sector?
            //
            if (this.theArc.theCircle.StructurallyEquals(that.theArc.theCircle))
            {
                foreach (Point pt in that.theArc.GetApproximatingPoints())
                {
                    if (!this.PointLiesInOrOn(pt)) return false;
                }

                return true;
            }

            // this radius must be longer than that.
            if (Utilities.GreaterThan(that.theArc.theCircle.radius, this.theArc.theCircle.radius)) return false;

            //
            // Check containment of the points of that sector.
            //
            if (!this.PointLiesInOrOn(that.theArc.endpoint1)) return false;
            if (!this.PointLiesInOrOn(that.theArc.endpoint2)) return false;

            if (!this.PointLiesInOrOn(that.theArc.theCircle.center)) return false;

            // Check midpoint is also within the sector.
            if (!this.PointLiesInOrOn(that.theArc.Midpoint())) return false;

            return true;
        }

        //
        // A shape within this shape?
        //
        public override bool Contains(Figure that)
        {
            if (that is Circle) return ContainsCircle(that as Circle);
            if (that is Polygon) return ContainsPolygon(that as Polygon);
            if (that is Sector) return ContainsSector(that as Sector);

            return false;
        }

        //
        // Area-Related Computations
        //
        protected double Area(double radAngleMeasure, double radius)
        {
            return 0.5 * radius * radius * radAngleMeasure;
        }
        protected double RationalArea(double radAngleMeasure, double radius)
        {
            return Area(radAngleMeasure, radius) / Math.PI;
        }
        public override bool IsComputableArea() { return true; }
        public override bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Central Angle
            if (known.GetAngleMeasure(this.theArc.GetCentralAngle()) < 0) return false;

            // Radius / Circle 
            return theArc.theCircle.CanAreaBeComputed(known);
        }
        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            if (theArc is Semicircle) return (theArc as Semicircle).GetArea(known);

            // Central Angle; this is minor arc measure by default
            double angleMeasure = Angle.toRadians(known.GetAngleMeasure(this.theArc.GetCentralAngle()));

            if (angleMeasure <= 0) return -1;

            // Make a major arc measure, if needed.
            if (theArc is MajorArc) angleMeasure = 2 * Math.PI - angleMeasure;

            // Radius / Circle
            double circArea = theArc.theCircle.GetArea(known);

            if (circArea <= 0) return -1;

            // The area is a proportion of the circle defined by the angle.
            return (angleMeasure / (2 * Math.PI)) * circArea;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Sector sector = obj as Sector;
            if (sector == null) return false;

            return theArc.StructurallyEquals(sector.theArc);
        }

        public override bool Equals(object obj)
        {
            Sector sector = obj as Sector;
            if (sector == null) return false;

            return theArc.Equals(sector.theArc);
        }

        public override string ToString() { return "Sector(" + theArc + ")"; }

        public override string CheapPrettyString()
        {
            string prefix = "";

            if (theArc is MajorArc) prefix = "Major";
            if (theArc is MinorArc) prefix = "Minor";
            if (theArc is Semicircle) return theArc.CheapPrettyString();

            return prefix + "(" +
                   theArc.endpoint1.SimpleToString() + theArc.theCircle.center.CheapPrettyString() + theArc.endpoint2.SimpleToString() + ")";
        }

        //
        // Does this particular segment intersect one of the sides.
        //
        public bool Covers(Segment that)
        {
            if (radius1.Covers(that)) return true;

            if (radius2.Covers(that)) return true;

            return theArc.Covers(that);
        }

        //
        // An arc is covered if one side of the polygon defines the endpoints of the arc.
        //
        public bool Covers(Arc that)
        {
            if (radius1.Covers(that)) return true;

            if (radius2.Covers(that)) return true;

            return theArc.Covers(that);
        }

        //
        // Does the atom have a connection which intersects the sides of the polygon.
        //
        public override bool Covers(AtomicRegion atom)
        {
            foreach (Connection conn in atom.connections)
            {
                if (conn.type == ConnectionType.SEGMENT)
                {
                    if (this.Covers(conn.segmentOrArc as Segment)) return true;
                }
                else if (conn.type == ConnectionType.ARC)
                {
                    if (this.Covers(conn.segmentOrArc as Arc)) return true;
                }
            }

            return false;
        }
    }
}