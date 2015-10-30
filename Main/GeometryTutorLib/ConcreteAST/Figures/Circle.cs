using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A circle is defined by a center and radius.
    /// </summary>
    public partial class Circle : Figure
    {
        public Point center { get; private set; }
        public double radius { get; private set; }

        // We define a secant to be a segment that passes through a circle (and contains a chord)
        // < Original secant segment, chord >
        public Dictionary<Segment, Segment> secants { get; private set; }

        // We define a chord to strictly be a segment that has BOTH endpoints on the circle (and does not extend).
        public List<Segment> chords { get; private set; }

        // Any radii defined by the figure.
        public List<Segment> radii { get; private set; }

        // A diameter is a special chord that passes through the center of the circle.
        public List<Segment> diameters { get; private set; }

        // Tangents intersect the circle at one point; the pair is <tangent, radius> where radius creates the 90^o angle.
        public Dictionary<Segment, Segment> tangents { get; private set; }

        // Polygons that are circumscribed about the circle. 
        public List<Polygon>[] circumPolys { get; private set; }

        // Polygons that are inscribed in the circle. 
        public List<Polygon>[] inscribedPolys { get; private set; }

        // The list of points from the UI which involve this circle.
        public List<Point> pointsOnCircle { get; private set; }

        // The minor Arcs of this circle (based on pointsOnCircle list)
        public List<MinorArc> minorArcs { get; private set; }
        public List<MajorArc> majorArcs { get; private set; }

        // The sectors of this circle (based on pointsOnCircle list)
        public List<Sector> minorSectors { get; private set; }
        public List<Sector> majorSectors { get; private set; }

        // Points that approximate the circle using straight-line segments.
        public List<Point> approxPoints { get; protected set; }
        public List<Segment> approxSegments { get; protected set; }

        /// <summary>
        /// Create a new ConcreteSegment. 
        /// </summary>
        /// <param name="p1">A point defining the segment.</param>
        /// <param name="p2">Another point defining the segment.</param>
        public Circle(Point center, double r) : base()
        {
            this.center = center;
            radius = r;

            secants = new Dictionary<Segment, Segment>();
            chords = new List<Segment>();
            radii = new List<Segment>();
            diameters = new List<Segment>();
            tangents = new Dictionary<Segment, Segment>();

            inscribedPolys = new List<Polygon>[Polygon.MAX_EXC_POLY_INDEX];
            circumPolys = new List<Polygon>[Polygon.MAX_EXC_POLY_INDEX];
            for (int n = Polygon.MIN_POLY_INDEX; n < Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                inscribedPolys[n] = new List<Polygon>();
                circumPolys[n] = new List<Polygon>();
            }

            pointsOnCircle = new List<Point>();

            minorArcs = new List<MinorArc>();
            majorArcs = new List<MajorArc>();
            minorSectors = new List<Sector>();
            majorSectors = new List<Sector>();

            approxPoints = new List<Point>();
            approxSegments = new List<Segment>();

            Utilities.AddUniqueStructurally(this.center.getSuperFigures(), this);

            thisAtomicRegion = new ShapeAtomicRegion(this);

            this.FigureSynthesizerConstructor();
        }

        public void AddMinorArc(MinorArc mArc) { minorArcs.Add(mArc); }
        public void AddMajorArc(MajorArc mArc) { majorArcs.Add(mArc); }
        public void AddMinorSector(Sector mSector) { minorSectors.Add(mSector); }
        public void AddMajorSector(Sector mSector) { majorSectors.Add(mSector); }

        public void SetPointsOnCircle(List<Point> pts) { OrderPoints(pts); }

        public bool DefinesRadius(Segment seg)
        {
            if (center.StructurallyEquals(seg.Point1) && this.PointLiesOn(seg.Point2)) return true;

            return center.StructurallyEquals(seg.Point1) && this.PointLiesOn(seg.Point2);
        }

        public bool DefinesDiameter(Segment seg)
        {
            if (!seg.PointLiesOnAndExactlyBetweenEndpoints(center)) return false;

            return this.PointLiesOn(seg.Point1) && this.PointLiesOn(seg.Point2);
        }

        //
        // Area-Related Computations
        //
        protected double Area(double radius)
        {
            return radius * radius * Math.PI;
        }
        protected double RationalArea(double radius)
        {
            return Area(radius) / Math.PI;
        }
        public override bool IsComputableArea() { return true; }
        public override bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Any Radius known?
            foreach (Segment thisRadius in radii)
            {
                double length = known.GetSegmentLength(thisRadius);
                if (length > 0) return true;
            }

            return false;
        }
        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Any Radius known?
            double length = -1;
            foreach (Segment thisRadius in radii)
            {
                length = known.GetSegmentLength(thisRadius);
                if (length > 0) break;
            }

            if (length < 0) return -1;

            return Area(length);
        }

        //
        // For arcs, order the points so that there is a consistency: A, B, C, D-> B between AC, B between AD, etc.
        // Only need to order the points if there are more than three points
        //
        public List<Point> OrderPoints(List<Point> points)
        {
            List<KeyValuePair<double, Point>> pointAngleMap = new List<KeyValuePair<double, Point>>();

            foreach (Point point in points)
            {
                double radianAngle = Point.GetRadianStandardAngleWithCenter(this.center, point);

                // Angles are between 0 and 2pi
                // insert the point into the correct position (starting from the back); insertion sort-style
                int index;
                for (index = 0; index < pointAngleMap.Count; index++)
                {
                    if (radianAngle > pointAngleMap[index].Key) break;
                }
                pointAngleMap.Insert(index, new KeyValuePair<double, Point>(radianAngle, point));
            }

            //
            // Put all the points in the final ordered list
            //
            List<Point> ordered = new List<Point>();
            foreach (KeyValuePair<double, Point> pair in pointAngleMap)
            {
                pointsOnCircle.Add(pair.Value);
                ordered.Add(pair.Value);
            }

            return ordered;
        }

        public List<Point> ConstructAllMidpoints(List<Point> given)
        {
            List<Point> ordered = this.OrderPoints(given);
            List<Point> ptsWithMidpoints = new List<Point>();

            if (ordered.Count < 2) return ordered;

            //
            // Walk around the ordered points in a COUNTER-CLOCKWISE direction.
            //
            for (int p = 0; p < ordered.Count; p++)
            {
                ptsWithMidpoints.Add(ordered[p]);

                Point midpt = this.Midpoint(ordered[p], ordered[(p + 1) % ordered.Count]);
                Point opp = this.OppositePoint(midpt);

                if (Point.CounterClockwise(ordered[p], midpt, ordered[(p + 1) % ordered.Count]))
                {
                    ptsWithMidpoints.Add(midpt);
                }
                else ptsWithMidpoints.Add(opp);
            }

            return ptsWithMidpoints;
        }

        public Segment GetRadius(Segment r)
        {
            if (r == null) return null;

            return Utilities.GetStructurally<Segment>(radii, r);
        }

        public override bool PointLiesInOrOn(Point pt)
        {
            if (pt == null) return false;

            return PointLiesOn(pt) || PointLiesInside(pt);
        }

        public override bool PointLiesInside(Point pt)
        {
            if (pt == null) return false;

            if (PointLiesOn(pt)) return false;

            return PointIsInterior(pt);
        }

        public override List<Segment> Segmentize()
        {
            if (approxSegments.Any()) return approxSegments;

            // How much we will change the angle measure as we create segments.
            double angleIncrement = 2 * Math.PI / Figure.NUM_SEGS_TO_APPROX_ARC;

            // The first point will always be at 0 degrees.
            Point firstPoint = Point.GetPointFromAngle(center, radius, 0.0);
            Point secondPoint = null;
            double angle = 0;
            for (int i = 1; i <= Figure.NUM_SEGS_TO_APPROX_ARC; i++)
            {
                approxPoints.Add(firstPoint);

                // Get the next point.
                angle += angleIncrement;
                secondPoint = Point.GetPointFromAngle(center, radius, angle);

                // Make the segment.
                approxSegments.Add(new Segment(firstPoint, secondPoint));

                // Rotate points.
                firstPoint = secondPoint;
            }

            approxPoints.Add(secondPoint);

            return approxSegments;
        }

        // Make the circle into a regular n-gon that approximates it.
        public override Polygon GetPolygonalized()
        {
            if (polygonalized != null) return polygonalized;

            polygonalized = Polygon.MakePolygon(Segmentize());

            return polygonalized;
        }

        //
        // For each polygon, it is inscribed in the circle? Is it circumscribed?
        //
        public void AnalyzePolygonInscription(Polygon poly)
        {
            int index = Polygon.GetPolygonIndex(poly.orderedSides.Count);

            if (PolygonCircumscribesCircle(poly.orderedSides)) circumPolys[index].Add(poly);
            if (CircleCircumscribesPolygon(poly.orderedSides)) inscribedPolys[index].Add(poly);
        }

        //
        // The input polygon is in the form of segments (so it can be used by a polygon)
        //
        public bool PolygonCircumscribesCircle(List<Segment> segments)
        {
            // All of the sides of the polygon must be tangent to the circle to be circumscribed about the circle.
            foreach (Segment segment in segments)
            {
                if (this.IsTangent(segment) == null) return false;
            }

            return true;
        }

        //
        // The input polygon is in the form of segments (so it can be used by a polygon)
        //
        public bool CircleCircumscribesPolygon(List<Segment> segments)
        {
            // All of the vertices of the polygon must be on the circle to be inscribed in the circle.
            // That is, all of the segments must be chords.
            foreach (Segment segment in segments)
            {
                if (!this.IsChord(segment)) return false;
            }

            return true;
        }

        //
        // Determine if this segment is applicable to the circle: secants, tangent, and chords.
        //
        public void AnalyzeSegment(Segment thatSegment, List<Point> figPoints)
        {
            Segment tangentRadius = IsTangent(thatSegment);
            if (tangentRadius != null) tangents.Add(thatSegment, tangentRadius);

            if (DefinesDiameter(thatSegment))
            {
                // Add radii to the list.
                Utilities.AddStructurallyUnique<Segment>(radii, new Segment(this.center, thatSegment.Point1));
                Utilities.AddStructurallyUnique<Segment>(radii, new Segment(this.center, thatSegment.Point2));
            }

            if (IsChord(thatSegment))
            {
                Utilities.AddUnique<Segment>(chords, thatSegment);
            }
            else
            {
                // Atomizer.AtomicRegion secants and diameters (thus radii in special cases)
                Segment chord;
                if (IsSecant(thatSegment, figPoints, out chord))
                {
                    // Add to the secants for this circle.
                    secants.Add(thatSegment, chord);

                    // Also add to the chord list.
                    Utilities.AddUnique<Segment>(chords, chord);
                }
            }

            // Is a radius the result of a segment starting at the center and extending outward?
            // We collect all other types below.
            Segment radius = IsRadius(thatSegment, figPoints);
            if (radius != null) Utilities.AddUnique<Segment>(radii, radius);
        }

        //
        // Determine all applicable secants, tangent, and chords for this circle
        //
        public void CleanUp()
        {
            // Now that we have all the chords for this triangle, which are diameters?
            foreach (Segment chord in chords)
            {
                // The center needs to be the midpoint, but verifying the center is on the chord suffices in this context.
                if (chord.PointLiesOnAndExactlyBetweenEndpoints(this.center))
                {
                    // Add to diameters....
                    Utilities.AddUnique<Segment>(diameters, chord);

                    // but also collect radii
                    Segment newRadius = Segment.GetFigureSegment(this.center, chord.Point1);
                    if (newRadius == null) newRadius = new Segment(this.center, chord.Point1);
                    Utilities.AddStructurallyUnique<Segment>(radii, newRadius);
 
                    newRadius = Segment.GetFigureSegment(this.center, chord.Point2);
                    if (newRadius == null) newRadius = new Segment(this.center, chord.Point2);
                    Utilities.AddStructurallyUnique<Segment>(radii, newRadius);
                }
            }
        }

        //
        // Determine tangency of the given segment.
        // Indicate tangency by returning the segment which creates the 90^0 angle.
        //
        public Segment IsTangent(Segment segment)
        {
            // If the center and the segment points are collinear, this will not be a tangent.
            if (segment.PointLiesOn(this.center)) return null;

            // Acquire the line perpendicular to the segment that passes through the center of the circle.
            Segment perpendicular = segment.GetPerpendicular(this.center);

            // If the segment was found to pass through the center, it is not a tangent
            if (perpendicular.Equals(segment)) return null;

            // Is this perpendicular segment a radius? Check length
            //if (!Utilities.CompareValues(perpendicular.Length, this.radius)) return null;

            // Is the perpendicular a radius? Check that the intersection of the segment and the perpendicular is on the circle
            Point intersection = segment.FindIntersection(perpendicular);
            if (!this.PointLiesOn(intersection)) return null;

            // The intersection between the perpendicular and the segment must be within the endpoints of the segment.
            return segment.PointLiesOnAndBetweenEndpoints(intersection) ? perpendicular : null;
        }

        //
        // Does the given segment pass through the circle so that it acts as a diameter (or contains a diameter)?
        //
        private bool ContainsDiameter(Segment segment)
        {
            if (!segment.PointLiesOnAndBetweenEndpoints(this.center)) return false;

            // the endpoints of the segment must be on or outside the circle.
            double distance = Point.calcDistance(this.center, segment.Point1);
            if (distance < this.radius) return false;

            distance = Point.calcDistance(this.center, segment.Point2);
            if (distance < this.radius) return false;

            return true;
        }


        //
        // Given the secant, there is a midpoint along the secant (wrt to the circle), given the distance,
        // find the two points of intersection between the secant and the circle.
        // Return the resultant chord segment.
        //
        private Segment ConstructChord(Segment secantSegment, Point midpt, double distance, List<Point> figPoints)
        {
            //                distance
            //      circPt1    _____   circPt2
            //
            // Find the exact coordinates of the two 'circ' points.
            //
            double deltaX = 0;
            double deltaY = 0;
            if (secantSegment.IsVertical())
            {
                deltaX = 0;
                deltaY = distance;
            }
            else if (secantSegment.IsHorizontal())
            {
                deltaX = distance;
                deltaY = 0;
            }
            else
            {
                deltaX = Math.Sqrt(Math.Pow(distance, 2) / (1 + Math.Pow(secantSegment.Slope, 2)));
                deltaY = secantSegment.Slope * deltaX;
            }
            Point circPt1 = Utilities.AcquirePoint(figPoints, new Point("", midpt.X + deltaX, midpt.Y + deltaY));

            // intersection is the midpoint of circPt1 and pt2.
            Point circPt2 = Utilities.AcquirePoint(figPoints, new Point("", 2 * midpt.X - circPt1.X, 2 * midpt.Y - circPt1.Y));

            // Create the actual chord
            return new Segment(circPt1, circPt2);
        }

        //
        // Determine if the segment passes through the circle (we know it is not a chord since they have been filtered).
        //
        private bool IsSecant(Segment segment, List<Point> figPoints, out Segment chord)
        {
            // Make it null and overwrite when necessary.
            chord = null;

            // Is the segment exterior to the circle, but intersects at an endpoint (and wasn't tangent).
            if (this.PointIsExterior(segment.Point1) && this.PointLiesOn(segment.Point2)) return false;
            if (this.PointIsExterior(segment.Point2) && this.PointLiesOn(segment.Point1)) return false;

            // Is one endpoint of the segment simply on the interior of the circle (so we have nothing)?
            if (this.PointIsInterior(segment.Point1) || this.PointIsInterior(segment.Point2)) return false;

            if (ContainsDiameter(segment))
            {
                chord = ConstructChord(segment, this.center, this.radius, figPoints);

                // Add radii to the list.
                radii.Add(new Segment(this.center, chord.Point1));
                radii.Add(new Segment(this.center, chord.Point2));

                return true;
            }

            // Acquire the line perpendicular to the segment that passes through the center of the circle.
            Segment perpendicular = segment.GetPerpendicular(this.center);

            // Is this perpendicular segment a radius? If so, it's tangent, not a secant
            //if (Utilities.CompareValues(perpendicular.Length, this.radius)) return false;

            // Is the perpendicular a radius? Check if the intersection of the segment and the perpendicular is on the circle. If so, it's tangent
            Point intersection = segment.FindIntersection(perpendicular);
            if (this.PointLiesOn(intersection)) return false;

            //Adjust perpendicular segment to include intersection with segment
            perpendicular = new Segment(intersection, this.center);

            // Filter the fact that there are no intersections
            if (perpendicular.Length > this.radius) return false;

            //            1/2 chord length
            //                 _____   circPoint
            //                |    /
            //                |   /
            // perp.Length    |  / radius
            //                | /
            //                |/
            // Determine the half-chord length via Pyhtagorean Theorem.
            double halfChordLength = Math.Sqrt(Math.Pow(this.radius, 2) - Math.Pow(perpendicular.Length, 2));

            chord = ConstructChord(segment, perpendicular.OtherPoint(this.center), halfChordLength, figPoints);

            return true;
        }

        //
        // Is this a direct radius segment where one endpoint originates at the origin and extends outward?
        // Return the exact radius.
        private Segment IsRadius(Segment segment, List<Point> figPoints)
        {
            // The segment must originate from the circle center.
            if (!segment.HasPoint(this.center)) return null;

            // The segment must be at least as long as a radius.
            if (!Utilities.CompareValues(segment.Length, this.radius)) return null;

            Point nonCenterPt = segment.OtherPoint(this.center);

            // Check for a direct radius.
            if (this.PointLiesOn(nonCenterPt)) return segment;

            //
            // Check for an extended segment.
            //
            //                radius
            //      center    _____   circPt
            //
            // Find the exact coordinates of the 'circ' points.
            //
            Point inter1 = null;
            Point inter2 = null;
            this.FindIntersection(segment, out inter1, out inter2);

            Point figPoint = Utilities.GetStructurally<Point>(figPoints, inter1);
            if (figPoint == null) figPoint = Utilities.GetStructurally<Point>(figPoints, inter2);

            return new Segment(center, figPoint);
        }

        //
        // Find the points of intersection of two circles; may be 0, 1, or 2.
        //
        public override void FindIntersection(Segment ts, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;

            Segment s = new Segment(ts.Point1, ts.Point2);

            // SEE: http://stackoverflow.com/questions/1073336/circle-line-collision-detection

            // We have line AB, cicle center C, and radius R.
            double lengthAB = s.Length;
            double[] D = { (ts.Point2.X - ts.Point1.X) / lengthAB, (ts.Point2.Y - ts.Point1.Y) / lengthAB }; //Direction vector from A to B

            // Now the line equation is x = D[0]*t + A.X, y = D[1]*t + A.Y with 0 <= t <= 1.
            double t = D[0] * (this.center.X - ts.Point1.X) + D[1] * (this.center.Y - ts.Point1.Y); //Closest point to circle center
            double[] E = { t * D[0] + ts.Point1.X, t * D[1] + ts.Point1.Y }; //The point described by t.

            double lengthEC = System.Math.Sqrt(System.Math.Pow(E[0] - this.center.X, 2) + System.Math.Pow(E[1] - this.center.Y, 2));

            // Possible Intersection?
            if (lengthEC < this.radius)
            {
                // Compute distance from t to circle intersection point
                double dt = System.Math.Sqrt(System.Math.Pow(this.radius, 2) - System.Math.Pow(lengthEC, 2));

                // First intersection - find and verify that the point lies on the segment
                Point possibleInter1 = new Point("", (t - dt) * D[0] + ts.Point1.X, (t - dt) * D[1] + ts.Point1.Y);
                /* if (ts.PointLiesOnAndBetweenEndpoints(possibleInter1)) */ inter1 = possibleInter1;

                // Second intersection - find and verify that the point lies on the segment
                Point possibleInter2 = new Point("", (t + dt) * D[0] + ts.Point1.X, (t + dt) * D[1] + ts.Point1.Y);
                /* if (ts.PointLiesOnAndBetweenEndpoints(possibleInter2)) */ inter2 = possibleInter2;
            }
            //
            // Tangent point (E)
            //
            else if (Utilities.CompareValues(lengthEC, this.radius))
            {
                // First intersection
                inter1 = new Point("", E[0], E[1]);
            }

            // Put the intersection into inter1 if there is only one intersection.
            if (inter1 == null && inter2 != null) { inter1 = inter2; inter2 = null; }
        }

        //
        // Find the points of intersection of two circles; may be 0, 1, or 2.
        // Uses the technique found here: http://mathworld.wolfram.com/Circle-CircleIntersection.html
        //
        public void FindIntersection(Circle thatCircle, out Point inter1, out Point inter2)
        {
            inter1 = null;
            inter2 = null;

            // SEE: http://stackoverflow.com/questions/3349125/circle-circle-intersection-points

            // Distance between centers
            double d = System.Math.Sqrt(System.Math.Pow(thatCircle.center.X - this.center.X, 2) +
                                        System.Math.Pow(thatCircle.center.Y - this.center.Y, 2));

            // Separate circles
            if (d > this.radius + thatCircle.radius) { }

            // One circle contained in the other
            else if (d < System.Math.Abs(this.radius - thatCircle.radius)) { }

            // Coinciding circles
            else if (d == 0 && this.radius == thatCircle.radius) { }

            // We have intersection(s)!
            else
            {
                // Distance from center of this to midpt of intersections
                double a = (System.Math.Pow(this.radius, 2) - System.Math.Pow(thatCircle.radius, 2) + System.Math.Pow(d, 2)) / (2 * d);

                // Midpoint of the intersections
                double[] midpt = { this.center.X + a * (thatCircle.center.X - this.center.X) / d, this.center.Y + a * (thatCircle.center.Y - this.center.Y) / d };

                // Distance from midpoint to intersections
                double h = System.Math.Sqrt(System.Math.Pow(this.radius, 2) - System.Math.Pow(a, 2));

                // Only one intersection
                if (h == 0)
                {
                    inter1 = new Point("", midpt[0], midpt[1]);
                }
                // Two intersections
                else
                {
                    inter1 = new Point("", midpt[0] + h * (thatCircle.center.Y - this.center.Y) / d,
                                           midpt[1] - h * (thatCircle.center.X - this.center.X) / d);

                    inter2 = new Point("", midpt[0] - h * (thatCircle.center.Y - this.center.Y) / d,
                                           midpt[1] + h * (thatCircle.center.X - this.center.X) / d);
                }
            }

            // Put the intersection into inter1 if there is only one intersection.
            if (inter1 == null && inter2 != null)
            {
                inter1 = inter2;
                inter2 = null;
            }

            //
            // Are the circles close enough to merit one intersection point instead of two?
            // That is, are the intersection points the same (within epsilon)?
            //
            if (inter1 != null && inter2 != null)
            {
                if (inter1.StructurallyEquals(inter2)) inter2 = null;
            }
        }

        //
        // Are the segment endpoints directly on the circle? 
        //
        private bool IsChord(Segment segment)
        {
            return this.PointLiesOn(segment.Point1) && this.PointLiesOn(segment.Point2);
        }

        //
        // Determine if the given point is on the circle via substitution into (x1 - x2)^2 + (y1 - y2)^2 = r^2
        //
        public override bool PointLiesOn(Point pt)
        {
            return Utilities.CompareValues(Math.Pow(center.X - pt.X, 2) + Math.Pow(center.Y - pt.Y, 2), Math.Pow(this.radius, 2));
        }

        //
        // Determine if the given point is a point in the interioir of the circle: via substitution into (x1 - x2)^2 + (y1 - y2)^2 = r^2
        //
        public bool PointIsInterior(Point pt)
        {
            return Utilities.LessThan(Point.calcDistance(this.center, pt), this.radius);
        }
        public bool PointIsExterior(Point pt)
        {
            return Utilities.GreaterThan(Point.calcDistance(this.center, pt), this.radius);
        }

        //
        // Concentric circles share the same center, but radii differ.
        //
        public bool AreConcentric(Circle thatCircle)
        {
            return this.center.StructurallyEquals(thatCircle.center) && !Utilities.CompareValues(thatCircle.radius, this.radius);
        }

        //
        // Orthogonal Circles intersect at 90^0: radii connecting to intersection point are perpendicular
        //
        public bool AreOrthognal(Circle thatCircle)
        {
            // Find the intersection points
            Point inter1;
            Point inter2;
            FindIntersection(thatCircle, out inter1, out inter2);

            // If the circles intersect at 0 points.
            if (inter1 == null) return false;

            // If the circles intersect at 1 point they are tangent
            if (inter2 == null) return false;

            // Create two radii, one for each circle; arbitrarily choose the first point (both work)
            Segment radiusThis = new Segment(this.center, inter1);
            Segment radiusThat = new Segment(this.center, inter1);

            return radiusThis.IsPerpendicularTo(radiusThat);
        }

        //
        // Tangent circle have 1 intersection point
        //
        public Point AreTangent(Circle thatCircle)
        {
            // Find the intersection points
            Point inter1;
            Point inter2;
            FindIntersection(thatCircle, out inter1, out inter2);

            // If the circles have one valid point of intersection.
            if (inter1 != null && inter2 == null) return inter1;

            return null;
        }

        //
        // Does the given segment contain a radius of this circle?
        //
        public bool ContainsRadiusWithin(Segment thatSegment)
        {
            foreach (Segment radius in radii)
            {
                if (thatSegment.HasSubSegment(radius)) return true;
            }

            return false;
        }

        //
        // Does the given segment contain a chord? Return the chord.
        //
        public Segment ContainsChord(Segment thatSegment)
        {
            foreach (KeyValuePair<Segment, Segment> pair in secants)
            {
                // Does the secant contain that segment? If so, is the chord contained in that Segment?
                if (pair.Key.HasSubSegment(thatSegment))
                {
                    if (thatSegment.HasSubSegment(pair.Value)) return pair.Value;
                }
            }

            return null;
        }

        //
        // Maintain a public repository of all circle objects in the figure.
        //
        public static void Clear()
        {
            figureCircles.Clear();
        }
        public static List<Circle> figureCircles = new List<Circle>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Circle) figureCircles.Add(clause as Circle);
        }
        public static Circle GetFigureCircle(Point cen, double rad)
        {
            Circle candCircle = new Circle(cen, rad);

            // Search for exact segment first
            foreach (Circle circle in figureCircles)
            {
                if (circle.StructurallyEquals(candCircle)) return circle;
            }

            return null;
        }

        public static List<Circle> GetFigureCirclesByRadius(Segment radius)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.radii.Contains(radius))
                {
                    circles.Add(circle);
                }
            }

            return circles;
        }

        public bool IsCentral(Angle angle)
        {
            if (this.center.StructurallyEquals(angle.GetVertex()))
            {
                // The rays need to contain radii of the circle.
                if (this.ContainsRadiusWithin(angle.ray1) && this.ContainsRadiusWithin(angle.ray2))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Circle> IsCentralAngle(Angle angle)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.IsCentral(angle))
                {
                    circles.Add(circle);
                }
            }

            return circles;
        }

        public bool IsInscribed(Angle angle)
        {
            // If the angle has vertex on the circle
            if (!this.PointLiesOn(angle.GetVertex())) return false;

            // Do the angle rays form or contain chords? 
            // GetChord() will check if the segment is a chord, and if it is not, it will check if the segment is a secant containing a chord
            Segment chord1 = this.GetChord(angle.ray1);
            Segment chord2 = this.GetChord(angle.ray2);

            return chord1 != null && chord2 != null;
        }

        public static List<Circle> IsInscribedAngle(Angle angle)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.IsInscribed(angle)) circles.Add(circle);
            }

            return circles;
        }

        public static List<Circle> GetChordCircles(Segment segment)
        {
            List<Circle> circles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.chords.Contains(segment)) circles.Add(circle);
            }

            return circles;
        }

        public static List<Circle> GetSecantCircles(Segment segment)
        {
            List<Circle> secCircles = new List<Circle>();

            foreach (Circle circle in figureCircles)
            {
                if (circle.secants.ContainsKey(segment)) secCircles.Add(circle);
                if (circle.chords.Contains(segment)) secCircles.Add(circle);
            }

            return secCircles;
        }

        // A lookup of a chord based on the given secant.
        public Segment GetChord(Segment thatSegment)
        {
            Segment chord = null;

            // If the given segment is a chord, return that segment
            if (chords.Contains(thatSegment)) return thatSegment;

            // Otherwise, check to see if it is a secant containing a chord
            secants.TryGetValue(thatSegment, out chord);

            return chord;
        }

        // return the midpoint between these two on the circle.
        public Point Midpoint(Point a, Point b)
        {
            if (!this.PointLiesOn(a)) return null;
            if (!this.PointLiesOn(b)) return null;

            // Make the chord.
            Segment chord = new Segment(a, b);

            Point pt1 = null;
            Point pt2 = null;

            // Is this a diameter? If so, quickly return a point perpendicular to the diameter
            if (DefinesDiameter(chord))
            {
                Segment perp = chord.GetPerpendicular(center);

                this.FindIntersection(perp, out pt1, out pt2);

                // Arbitrarily choose one of the points.
                return pt1 != null ? pt1 : pt2;
            }

            // Make radius through the midpoint of the chord.
            Segment radius = new Segment(center, chord.Midpoint());

            this.FindIntersection(radius, out pt1, out pt2);

            if (pt2 == null) return pt1;
            
            Point theMidpoint = Arc.StrictlyBetweenMinor(pt1, new MinorArc(this, a, b)) ? pt1 : pt2;

            double angle1 = new Angle(a, center, theMidpoint).measure;
            double angle2 = new Angle(b, center, theMidpoint).measure;
            if (!Utilities.CompareValues(angle1, angle2))
            {
                throw new ArgumentException("Midpoint is incorrect; angles do not equate: " + angle1 + " " + angle2);
            }

            return theMidpoint;
        }

        // return the midpoint between these two on the circle.
        public Point Midpoint(Point a, Point b, Point sameSide)
        {
            Point midpt = Midpoint(a, b);

            Segment segment = new Segment(a, b);
            Segment other = new Segment(midpt, sameSide);

            Point intersection = segment.FindIntersection(other);

            if (Segment.Between(intersection, midpt, sameSide)) return this.OppositePoint(midpt);

            return midpt;
        }

        // return the midpoint between these two on the circle.
        public Point OppositePoint(Point that)
        {
            if (!this.PointLiesOn(that)) return null;

            // Make the radius
            Segment radius = new Segment(center, that);

            Point pt1 = null;
            Point pt2 = null;
            this.FindIntersection(radius, out pt1, out pt2);

            if (pt2 == null) return null;

            return pt1.StructurallyEquals(that) ? pt2 : pt1;
        }

        public bool CircleContains(Circle that)
        {
            return Point.calcDistance(this.center, that.center) <= Math.Abs(this.radius - that.radius);
        }

        public bool HasArc(Arc arc)
        {
            return this.StructurallyEquals(arc.theCircle);
        }

        public bool HasArc(Point p1, Point p2)
        {
            return this.PointLiesOn(p1) && this.PointLiesOn(p2);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Circle thatCircle = obj as Circle;
            if (thatCircle == null) return false;

            return thatCircle.center.StructurallyEquals(center) && Utilities.CompareValues(thatCircle.radius, this.radius);
        }

        public override bool Equals(Object obj)
        {
            Circle thatCircle = obj as Circle;
            if (thatCircle == null) return false;

            return thatCircle.center.Equals(center) && Utilities.CompareValues(thatCircle.radius, this.radius);
        }

        public override string ToString()
        {
            return "Circle(" + this.center + ": r = " + this.radius + ")";
        }

        public override string CheapPrettyString()
        {
            return "Circle(" + this.center.SimpleToString() + ")";
        }

        public List<Area_Based_Analyses.Atomizer.AtomicRegion> Atomize(List<Point> figurePoints)
        {
            List<Segment> constructedChords = new List<Segment>();
            List<Segment> constructedRadii = new List<Segment>();
            List<Point> imagPoints = new List<Point>();

            List<Point> interPts = GetIntersectingPoints();

            //
            // Construct the radii
            //
            switch (interPts.Count)
            {
                // If there are no points of interest, the circle is the atomic region.
                case 0:
                  return Utilities.MakeList<AtomicRegion>(new ShapeAtomicRegion(this));

                // If only 1 intersection point, create the diameter.
                case 1:
                  Point opp = Utilities.AcquirePoint(figurePoints, this.OppositePoint(interPts[0]));
                  constructedRadii.Add(new Segment(center, interPts[0]));
                  constructedRadii.Add(new Segment(center, opp));
                  imagPoints.Add(opp);
                  interPts.Add(opp);
                  break;

                default:
                  foreach (Point interPt in interPts)
                  {
                      constructedRadii.Add(new Segment(center, interPt));
                  }
                  break;
            }

            //
            // Construct the chords
            //
            List<Segment> chords = new List<Segment>();
            for (int p1 = 0; p1 < interPts.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < interPts.Count; p2++)
                {
                    Segment chord = new Segment(interPts[p1], interPts[p2]);
                    if (!DefinesDiameter(chord)) constructedChords.Add(chord);
                }
            }

            //
            // Do any of the created segments result in imaginary intersection points.
            //
            foreach (Segment chord in constructedChords)
            {
                foreach (Segment radius in constructedRadii)
                {
                    Point inter = Utilities.AcquireRestrictedPoint(figurePoints, chord.FindIntersection(radius), chord, radius);
                    if (inter != null)
                    {
                        chord.AddCollinearPoint(inter);
                        radius.AddCollinearPoint(inter);

                        // if (!Utilities.HasStructurally<Point>(figurePoints, inter)) imagPoints.Add(inter);
                        Utilities.AddUnique<Point>(imagPoints, inter);
                    }
                }
            }

            for (int c1 = 0; c1 < constructedChords.Count - 1; c1++)
            {
                for (int c2 = c1 + 1; c2 < constructedChords.Count; c2++)
                {
                    Point inter = constructedChords[c1].FindIntersection(constructedChords[c2]);
                    inter = Utilities.AcquireRestrictedPoint(figurePoints, inter, constructedChords[c1], constructedChords[c2]);
                    if (inter != null)
                    {
                        constructedChords[c1].AddCollinearPoint(inter);
                        constructedChords[c2].AddCollinearPoint(inter);

                        //if (!Utilities.HasStructurally<Point>(figurePoints, inter)) imagPoints.Add(inter);
                        Utilities.AddUnique<Point>(imagPoints, inter);
                    }
                }
            }

            //
            // Add all imaginary points to the list of figure points.
            //
            Utilities.AddUniqueList<Point>(figurePoints, imagPoints);

            //
            // Construct the Planar graph for atomic region identification.
            //
            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph graph = new Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph();

            //
            // Add all imaginary points, intersection points, and center.
            //
            foreach (Point pt in imagPoints)
            {
                graph.AddNode(pt);
            }

            foreach (Point pt in interPts)
            {
                graph.AddNode(pt);
            }

            graph.AddNode(this.center);

            //
            // Add all chords and radii as edges.
            //
            foreach (Segment chord in constructedChords)
            {
                for (int p = 0; p < chord.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(chord.collinear[p], chord.collinear[p + 1],
                                            new Segment(chord.collinear[p], chord.collinear[p + 1]).Length,
                                            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                }
            }

            foreach (Segment radius in constructedRadii)
            {
                for (int p = 0; p < radius.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(radius.collinear[p], radius.collinear[p + 1],
                                            new Segment(radius.collinear[p], radius.collinear[p + 1]).Length,
                                            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                }
            }

            //
            // Add all arcs
            //
            List<Point> arcPts = this.ConstructAllMidpoints(interPts);
            for (int p = 0; p < arcPts.Count; p++)
            {
                graph.AddNode(arcPts[p]);
                graph.AddNode(arcPts[(p + 1) % arcPts.Count]);

                graph.AddUndirectedEdge(arcPts[p], arcPts[(p + 1) % arcPts.Count],
                                        new Segment(arcPts[p], arcPts[(p + 1) % interPts.Count]).Length,
                                        Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_ARC);
            }

            //
            // Convert the planar graph to atomic regions.
            //
            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();
            List<AtomicRegion> atoms = PrimitiveToRegionConverter.Convert(graph, primitives, Utilities.MakeList<Circle>(this));

            //
            // A filament may result in the creation of a major AND minor arc; both are not required.
            // Figure out which one to omit.
            // Multiple semi-circles may arise as well; omit if they can be broken into constituent elements.
            //
            List <AtomicRegion> trueAtoms = new List<AtomicRegion>();

            for (int a1 = 0; a1 < atoms.Count; a1++)
            {
                bool trueAtom = true;
                for (int a2 = 0; a2 < atoms.Count; a2++)
                {
                    if (a1 != a2)
                    {
                        if (atoms[a1].Contains(atoms[a2]))
                        {
                            trueAtom = false;
                            break;
                        }

                    }
                }

                if (trueAtom) trueAtoms.Add(atoms[a1]);
            }

            atoms = trueAtoms;

            return trueAtoms;
        }

        //private double CentralAngleMeasure(Point pt1, Point pt2)
        //{
        //    return (new MinorArc(this, pt1, pt2)).GetMinorArcMeasureDegrees();
        //}

        public void ConstructImpliedAreaBasedSectors(out List<Sector> minorSectors,
                                             out List<Sector> majorSectors,
                                             out List<Semicircle> semicircles)
        {
            minorSectors = new List<Sector>();
            majorSectors = new List<Sector>();
            semicircles = new List<Semicircle>();

            // Points of interest for atomic region identification (and thus arc / sectors).
            List<Point> interPts = this.OrderPoints(GetIntersectingPoints());

            // If there are no points of interest, the circle is the atomic region.
            if (!interPts.Any()) return;

            // Cycle through all n C 2 intersection points and resultant arcs / sectors.
            for (int p1 = 0; p1 < interPts.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < interPts.Count; p2++)
                {
                    //
                    // Do we have a diameter?
                    //
                    Segment diameter = new Segment(interPts[p1], interPts[p2]);
                    if (this.DefinesDiameter(diameter))
                    {
                        // Create two semicircles; for simplicity, we choose the points on the semi-circle to be midpoints o neither, respective side.
                        Point midpoint = this.Midpoint(interPts[p1], interPts[p2]);
                        Point oppMidpoint = this.OppositePoint(midpoint);

                        // Altogether, these 4 points define 4 quadrants (with the center).
                        semicircles.Add(new Semicircle(this, interPts[p1], interPts[p2], midpoint, diameter));
                        semicircles.Add(new Semicircle(this, interPts[p1], interPts[p2], oppMidpoint, diameter));
                    }

                    //
                    // Normal major / minor sector construction.
                    //
                    else
                    {
                        minorSectors.Add(new Sector(new MinorArc(this, interPts[p1], interPts[p2])));
                        majorSectors.Add(new Sector(new MajorArc(this, interPts[p1], interPts[p2])));
                    }
                }
            }
        }

        /// <summary>
        /// Make a set of connections for atomic region analysis.
        /// </summary>
        /// <returns></returns>
        public override List<Connection> MakeAtomicConnections()
        {
            List<Segment> segments = this.Segmentize();
            List<Connection> connections = new List<Connection>();

            foreach (Segment approxSide in segments)
            {
                connections.Add(new Connection(approxSide.Point1, approxSide.Point2, ConnectionType.ARC,
                                new MinorArc(this, approxSide.Point1, approxSide.Point2)));
            }

            return connections;
        }

        //
        // that circle lies within this circle.
        //
        private bool ContainsCircle(Circle that)
        {
            if (this.radius - that.radius < 0) return false;

            return Point.calcDistance(this.center, that.center) <= this.radius - that.radius;
        }

        //
        // that Polygon lies within this circle.
        //
        private bool ContainsPolygon(Polygon that)
        {
            //
            // All points are interior to the polygon.
            //
            foreach (Point thatPt in that.points)
            {
                if (!this.PointLiesInOrOn(thatPt)) return false;
            }

            return true;
        }

        //
        // that Polygon lies within this circle.
        //
        private bool ContainsSector(Sector that)
        {
            if (!this.PointLiesInOrOn(that.theArc.endpoint1)) return false;
            if (!this.PointLiesInOrOn(that.theArc.endpoint2)) return false;

            if (!this.PointLiesInOrOn(that.theArc.theCircle.center)) return false;
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
        // Does this particular segment intersect one of the sides.
        //
        public bool Covers(Segment that)
        {
            return this.PointLiesOn(that.Point1) && this.PointLiesOn(that.Point2);
        }

        //
        // An arc is covered if one side of the polygon defines the endpoints of the arc.
        //
        public bool Covers(Arc that)
        {
            return this.StructurallyEquals(that.theCircle);
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