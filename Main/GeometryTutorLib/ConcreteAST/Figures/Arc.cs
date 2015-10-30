using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Arc : Figure
    {
        public Circle theCircle { get; protected set; }
        public Point endpoint1 { get; protected set; }
        public Point endpoint2 { get; protected set; }
        public List<Point> arcMinorPoints { get; protected set; }
        public List<Point> arcMajorPoints { get; protected set; }
        public double minorMeasure { get; protected set; }
        public double length { get; protected set; }

        public List<Point> approxPoints { get; protected set; }
        public List<Segment> approxSegments { get; protected set; }

        public Arc(Circle circle, Point e1, Point e2) : this(circle, e1, e2, new List<Point>(), new List<Point>()) { }

        public Arc(Circle circle, Point e1, Point e2, List<Point> minorPts, List<Point> majorPts) : base()
        {
            theCircle = circle;
            endpoint1 = e1;
            endpoint2 = e2;
            arcMinorPoints = new List<Point>(minorPts);
            arcMajorPoints = new List<Point>(majorPts);

            Utilities.AddUniqueStructurally(e1.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(e2.getSuperFigures(), this);

            minorMeasure = CalculateArcMinorMeasureDegrees();
            length = CalculateArcMinorLength();
            approxPoints = new List<Point>();
            approxSegments = new List<Segment>();

            collinear = new List<Point>();
            // We add the two points arbitrarily since this list is vacuously ordered.
            collinear.Add(endpoint1);
            collinear.Add(endpoint2);
        }

        //
        // Do these arcs cross each other (pseudo-X)?
        //
        public bool Crosses(Arc that)
        {
            // Need to be from different circles.
            if (this.theCircle.StructurallyEquals(that.theCircle)) return false;

            // Need to touch at a point not at the endpoints; 
            if (this.HasEndpoint(that.endpoint1) && this.HasEndpoint(that.endpoint2)) return false;

            Point inter1, inter2;
            this.FindIntersection(that, out inter1, out inter2);

            if (inter1 == null && inter2 == null) return false;

            // Pseudo-X
            if (inter1 != null && inter2 == null)
            {
                return this.PointLiesStrictlyOn(inter1) && that.PointLiesStrictlyOn(inter1);
            }

            // Cursive r; an endpoint may be shared here.
            if (inter1 != null && inter2 != null)
            {
                return true;
            }

            return false;
        }

        public abstract Point Midpoint();

        public KeyValuePair<Segment, Segment> GetRadii()
        {
            return new KeyValuePair<Segment, Segment>(new Segment(theCircle.center, endpoint1), new Segment(theCircle.center, endpoint2));
        }
        public Angle GetCentralAngle()
        {
            return new Angle(endpoint1, theCircle.center, endpoint2);
        }
        public override List<Point> GetApproximatingPoints()
        {
            if (!approxPoints.Any()) Segmentize();

            return approxPoints;
        }
        public List<Segment> GetApproximatingSegments()
        {
            if (!approxSegments.Any()) return Segmentize();

            return approxSegments;
        }

        public Point OtherEndpoint(Point that)
        {
            if (that == null) return null;

            if (that.StructurallyEquals(endpoint1)) return endpoint2;
            if (that.StructurallyEquals(endpoint2)) return endpoint1;

            return null;
        }

        public Point SharedEndpoint(Arc that)
        {
            if (this.endpoint1.StructurallyEquals(that.endpoint1)) return endpoint1;
            if (this.endpoint1.StructurallyEquals(that.endpoint2)) return endpoint1;

            if (this.endpoint2.StructurallyEquals(that.endpoint1)) return endpoint2;
            if (this.endpoint2.StructurallyEquals(that.endpoint2)) return endpoint2;
            
            return null;
        }

        public abstract bool PointLiesStrictlyOn(Point pt);
        public abstract bool HasSubArc(Arc that);

        public override void AddCollinearPoint(Point newPt)
        {
            if (Utilities.HasStructurally<Point>(collinear, newPt)) return;

            collinear.Add(newPt);

            collinear = theCircle.OrderPoints(collinear);
        }

        //
        // The goal is the return the set of collinear points such that the endpoints bookend the list.
        //
        public List<Point> GetOrderedCollinearPointsByEndpoints(List<Point> given)
        {
            // Find only the points on this arc.
            List<Point> applicable = new List<Point>();

            foreach (Point pt in given)
            {
                if (this.PointLiesOn(pt)) applicable.Add(pt);
            }

            List<Point> ordered = theCircle.OrderPoints(applicable);

            int index1 = ordered.IndexOf(endpoint1);
            int index2 = ordered.IndexOf(endpoint2);

            if ((index1 == 0 && index2 == ordered.Count-1) || (index2 == 0 && index1 == ordered.Count-1)) return ordered;

            if (index1 + 1 != index2 && index2 + 1 != index1) throw new Exception("Logic failure to order points...");

            List<Point> bookend = new List<Point>();
            int start = -1;
            int end = -1;

            if (index1 > index2)
            {
                start = index1;
                end = index2;
            }
            else if (index2 > index1)
            {
                start = index2;
                end = index1;
            }
            else throw new Exception("Logic failure to order points...");

            for (int i = start; i != end; )
            {
                bookend.Add(ordered[i]);
                if (i + 1 == ordered.Count) i = 0;
                else i++;
            }
            bookend.Add(ordered[end]);

            return bookend;
        }

        public List<Point> GetOrderedByEndpointsWithMidpoints(List<Point> given)
        {
            List<Point> givenWithMidpoints = theCircle.ConstructAllMidpoints(given);

            return GetOrderedCollinearPointsByEndpoints(givenWithMidpoints);
        }

        public override void ClearCollinear()
        {
            collinear.Clear();
            collinear.Add(endpoint1);
            collinear.Add(endpoint2);
        }

        //
        // Calculate the length of the arc: s = r * theta (radius * central angle)
        //
        private double CalculateArcMinorLength() { return GetMinorArcMeasureRadians() * theCircle.radius; }

        //
        // The measure of the minor arc is equal to the measure of the central angle it cuts out.
        // This is calculated in degrees.
        //
        private double CalculateArcMinorMeasureDegrees()
        {
            return new Angle(new Segment(theCircle.center, endpoint1), new Segment(theCircle.center, endpoint2)).measure;
        }
        private double CalculateArcMinorMeasureRadians()
        {
            return Angle.toRadians(new Angle(new Segment(theCircle.center, endpoint1), new Segment(theCircle.center, endpoint2)).measure);
        }

        public double GetMinorArcMeasureDegrees() { return minorMeasure; }
        public double GetMinorArcMeasureRadians() { return Angle.toRadians(GetMinorArcMeasureDegrees()); }
        public double GetMajorArcMeasureDegrees() { return 360 - minorMeasure; }
        public double GetMajorArcMeasureRadians() { return Angle.toRadians(GetMajorArcMeasureDegrees()); }

        //
        // Maintain a public repository of all segment objects in the figure.
        //
        public static void Clear()
        {
            figureMinorArcs.Clear();
            figureMajorArcs.Clear();
            figureSemicircles.Clear();
        }
        public static List<MinorArc> figureMinorArcs = new List<MinorArc>();
        public static List<MajorArc> figureMajorArcs = new List<MajorArc>();
        public static List<Semicircle> figureSemicircles = new List<Semicircle>();
        public static void Record(GroundedClause clause)
        {
            if (clause is MinorArc) figureMinorArcs.Add(clause as MinorArc);
            if (clause is MajorArc) figureMajorArcs.Add(clause as MajorArc);
            if (clause is Semicircle) figureSemicircles.Add(clause as Semicircle);
        }
        public static Arc GetFigureMinorArc(Circle circle, Point pt1, Point pt2)
        {
            MinorArc candArc = new MinorArc(circle, pt1, pt2);

            // Search for exact segment first
            foreach (MinorArc arc in figureMinorArcs)
            {
                if (arc.StructurallyEquals(candArc)) return arc;
            }

            return null;
        }
        public static Arc GetFigureMajorArc(Circle circle, Point pt1, Point pt2)
        {
            MajorArc candArc = new MajorArc(circle, pt1, pt2);

            // Search for exact segment first
            foreach (MajorArc arc in figureMajorArcs)
            {
                if (arc.StructurallyEquals(candArc)) return arc;
            }

            return null;
        }
        public static Arc GetFigureSemicircle(Circle circle, Point pt1, Point pt2, Point middle)
        {
            Segment diameter = new Segment(pt1, pt2);
            Semicircle candArc = new Semicircle(circle, pt1, pt2, middle, diameter);

            foreach (Semicircle arc in figureSemicircles)
            {
                if (arc.StructurallyEquals(candArc)) return arc;
            }

            return null;
        }
        private static Arc GetInscribedInterceptedArc(Circle circle, Angle angle)
        {
            Point endpt1, endpt2;

            Point pt1, pt2;
            circle.FindIntersection(angle.ray1, out pt1, out pt2);
            endpt1 = pt1.StructurallyEquals(angle.GetVertex()) ? pt2 : pt1;

            circle.FindIntersection(angle.ray2, out pt1, out pt2);
            endpt2 = pt1.StructurallyEquals(angle.GetVertex()) ? pt2 : pt1;

            // Need to check if the angle is a diameter and create a semicircle
            Segment chord = new Segment(endpt1, endpt2);
            if (circle.DefinesDiameter(chord))
            {
                Point opp = circle.Midpoint(endpt1, endpt2, angle.GetVertex());
                Semicircle semi = new Semicircle(circle, endpt1, endpt2, circle.OppositePoint(opp), chord);
                //Find a defined semicircle of the figure that lies on the same side
                Semicircle sameSideSemi = figureSemicircles.Where(s => semi.SameSideSemicircle(s)).FirstOrDefault();
                //If none were found, should we throw an exception or just return the original semi?
                if (sameSideSemi == null) return semi;
                else return sameSideSemi;
            }

            //Initially assume intercepted arc is the minor arc
            Arc intercepted = null;
            intercepted = new MinorArc(circle, endpt1, endpt2);
            //Verify assumption, create major arc if necessary
            if (Arc.BetweenMinor(angle.GetVertex(), intercepted)) intercepted = new MajorArc(circle, endpt1, endpt2);
            return intercepted;
        }

        //
        // Returns the single (closest) intercepted arc for an angle.
        //
        public static Arc GetInterceptedArc(Circle circle, Angle angle)
        {
            if (circle.IsInscribed(angle)) return GetInscribedInterceptedArc(circle, angle);

            KeyValuePair<Arc, Arc> intercepted = Arc.GetInterceptedArcs(circle, angle);

            return intercepted.Key;
        }


        //
        // Acquires one or two intercepted arcs from an exterior or interior angle vertex.
        //
        public static KeyValuePair<Arc, Arc> GetInterceptedArcs(Circle circle, Angle angle)
        {
            KeyValuePair<Arc, Arc> nullPair = new KeyValuePair<Arc, Arc>(null, null);

            //
            // Get the intersection points of the rays of the angle.
            //
            Point interRay11 = null;
            Point interRay12 = null;
            circle.FindIntersection(angle.ray1, out interRay11, out interRay12);
            if (!angle.ray1.PointLiesOnAndBetweenEndpoints(interRay11)) interRay11 = null;
            if (!angle.ray1.PointLiesOnAndBetweenEndpoints(interRay12)) interRay12 = null;
            if (interRay11 == null && interRay12 != null) interRay11 = interRay12;

            // non-intersection
            if (interRay11 == null && interRay12 == null) return nullPair;

            Point interRay21 = null;
            Point interRay22 = null;
            circle.FindIntersection(angle.ray2, out interRay21, out interRay22);
            if (!angle.ray2.PointLiesOnAndBetweenEndpoints(interRay21)) interRay21 = null;
            if (!angle.ray2.PointLiesOnAndBetweenEndpoints(interRay22)) interRay22 = null;
            if (interRay21 == null && interRay22 != null) interRay21 = interRay22;

            // non-intersection
            if (interRay21 == null && interRay22 == null) return nullPair;

            //
            // Split the rays into cases based on if they are secants or not.
            //
            bool isSecRay1 = angle.ray1.IsSecant(circle);
            bool isSecRay2 = angle.ray2.IsSecant(circle);

            //
            // One Arc: No secants
            //
            if (!isSecRay1 && !isSecRay2)
            {
                // This means the endpoints of the ray were on the circle directly for each.
                return new KeyValuePair<Arc,Arc>(Arc.GetFigureMinorArc(circle, interRay11, interRay21), null);
            }
            //
            // One Arc; with one secant and one not.
            //
            else if (!isSecRay1 || !isSecRay2)
            {
                Segment secant = null;
                Segment nonSecant = null;
                Point endPtNonSecant = null;

                if (isSecRay1)
                {
                    secant = angle.ray1;
                    nonSecant = angle.ray2;
                    endPtNonSecant = interRay21;
                }
                else
                {
                    secant = angle.ray2;
                    nonSecant = angle.ray1;
                    endPtNonSecant = interRay11;
                }

                Segment chordOfSecant = circle.ContainsChord(secant);

                Point endptSecant = Segment.Between(chordOfSecant.Point1, angle.GetVertex(), chordOfSecant.Point2) ?
                                                                        chordOfSecant.Point1 : chordOfSecant.Point2;

                return new KeyValuePair<Arc,Arc>(Arc.GetFigureMinorArc(circle, endPtNonSecant, endptSecant), null);
            }

            //
            // Two arcs
            //
            else
            {
                //
                // Ensure proper ordering of points
                //
                Point closeRay1, farRay1;
                Point closeRay2, farRay2;

                if (Segment.Between(interRay11, angle.GetVertex(), interRay12))
                {
                    closeRay1 = interRay11;
                    farRay1 = interRay12;
                }
                else
                {
                    closeRay1 = interRay12;
                    farRay1 = interRay11;
                }

                if (Segment.Between(interRay21, angle.GetVertex(), interRay22))
                {
                    closeRay2 = interRay21;
                    farRay2 = interRay22;
                }
                else
                {
                    closeRay2 = interRay22;
                    farRay2 = interRay21;
                }

                return new KeyValuePair<Arc, Arc>(Arc.GetFigureMinorArc(circle, closeRay1, closeRay2),
                                                  Arc.GetFigureMinorArc(circle, farRay1, farRay2));
            }
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        public override bool StructurallyEquals(object obj)
        {
            Arc arc = obj as Arc;
            if (arc == null) return false;

            return this.theCircle.StructurallyEquals(arc.theCircle) && ((this.endpoint1.StructurallyEquals(arc.endpoint1)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint2))
                                                                    || (this.endpoint1.StructurallyEquals(arc.endpoint2)
                                                                    && this.endpoint2.StructurallyEquals(arc.endpoint1)));
        }

        public override bool Equals(Object obj)
        {
            Arc arc = obj as Arc;
            if (arc == null) return false;

            // Check equality of arc minor / major points?

            // Verify that the arcs match
            bool arcsMatch = this.theCircle.Equals(arc.theCircle) && ((this.endpoint1.Equals(arc.endpoint1)
                                                                  && this.endpoint2.Equals(arc.endpoint2))
                                                                  || (this.endpoint1.Equals(arc.endpoint2)
                                                                  && this.endpoint2.Equals(arc.endpoint1)));

            // Verify that the multipliers match
            return arcsMatch && base.Equals(obj);
        }

        public override bool ContainsClause(GroundedClause target)
        {
            return this.Equals(target);
        }

        public override string ToString() { return "Arc(" + theCircle + "(" + endpoint1.ToString() + ", " + endpoint2.ToString() + "))"; }

        // Does this arc contain a sub-arc:
        // A-------B-------C------D
        // A subarc is: AB, AC, AD, BC, BD, CD
        public bool HasMinorSubArc(Arc arc)
        {
            return Arc.BetweenMinor(arc.endpoint1, this) && Arc.BetweenMinor(arc.endpoint2, this);
        }

        public bool HasStrictMinorSubArc(Arc arc)
        {
            return Arc.StrictlyBetweenMinor(arc.endpoint1, this) && Arc.StrictlyBetweenMinor(arc.endpoint2, this);
        }

        public bool HasMajorSubArc(Arc arc)
        {
            return Arc.BetweenMajor(arc.endpoint1, this) && Arc.BetweenMajor(arc.endpoint2, this);
        }

        public bool HasStrictMajorSubArc(Arc arc)
        {
            return Arc.StrictlyBetweenMajor(arc.endpoint1, this) && Arc.StrictlyBetweenMajor(arc.endpoint2, this);
        }

        //
        // Is M between A and B in the minor arc
        //
        public static bool BetweenMinor(Point m, Arc originalArc)
        {
            if (m == null) return false;

            // Is the point on this circle?
            if (!originalArc.theCircle.PointLiesOn(m)) return false;

            // Create two arcs from this new point to the endpoints; just like with segments,
            // the sum of the arc measures must equate to the overall arc measure.
            MinorArc arc1 = new MinorArc(originalArc.theCircle, m, originalArc.endpoint1);
            MinorArc arc2 = new MinorArc(originalArc.theCircle, m, originalArc.endpoint2);
 
            return Utilities.CompareValues(arc1.minorMeasure + arc2.minorMeasure, originalArc.minorMeasure);
        }

        public static bool StrictlyBetweenMinor(Point m, Arc originalArc)
        {
            if (m == null) return false;

            if (originalArc.HasEndpoint(m)) return false;

            return BetweenMinor(m, originalArc);
        }

        //
        // If it's on the circle and not in the minor arc, it's in the major arc.
        //
        public static bool BetweenMajor(Point m, Arc originalArc)
        {
            if (originalArc.HasEndpoint(m)) return true;

            if (m == null) return false;

            // Is the point on this circle?
            if (!originalArc.theCircle.PointLiesOn(m)) return false;

            // Is it on the arc minor?
            if (BetweenMinor(m, originalArc)) return false;

            return true;
        }

        public static bool StrictlyBetweenMajor(Point m, Arc originalArc)
        {
            if (m == null) return false;

            if (originalArc.HasEndpoint(m)) return false;

            return BetweenMajor(m, originalArc);
        }

        public bool HasEndpoint(Point p)
        {
            return endpoint1.Equals(p) || endpoint2.Equals(p);
        }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            Arc other = (Arc)(this.MemberwiseClone());
            other.endpoint1 = (Point)endpoint1.DeepCopy();
            other.endpoint2 = (Point)endpoint2.DeepCopy();

            return other;
        }

        //
        // Is this arc congruent to the given arc in terms of the coordinatization from the UI?
        //
        public bool CoordinateCongruent(Arc a) { return Utilities.CompareValues(this.length, a.length); }

        //
        // Is this segment proportional to the given segment in terms of the coordinatization from the UI?
        // We should not report proportional if the ratio between segments is 1
        //
        public KeyValuePair<int, int> CoordinateProportional(Arc a) { return Utilities.RationalRatio(this.length, a.length); }

        //
        // Concentric
        //
        public bool IsConcentricWith(Arc thatArc) { return this.theCircle.AreConcentric(thatArc.theCircle); }
        //
        // Orthogonal
        //
        //
        // Orthogonal arcs intersect at 90^0: radii connecting to intersection point are perpendicular.
        //
        public bool AreOrthognal(Arc thatArc)
        {
            if (!this.theCircle.AreOrthognal(thatArc.theCircle)) return false;
            
            // Find the intersection points
            Point inter1;
            Point inter2;
            this.theCircle.FindIntersection(thatArc.theCircle, out inter1, out inter2);

            // Is the intersection between the endpoints of both arcs? Check both.
            if (Arc.BetweenMinor(inter1, this) && Arc.BetweenMinor(inter1, thatArc)) return true;
            if (Arc.BetweenMinor(inter2, this) && Arc.BetweenMinor(inter2, thatArc)) return true;

            return false;
        }

        //
        // Tangent circle have 1 intersection point
        //
        public Point AreTangent(Arc thatArc)
        {
            Point intersection = this.theCircle.AreTangent(thatArc.theCircle);

            // Is the intersection between the endpoints of both arcs? Check both.
            if (Arc.BetweenMinor(intersection, this) && Arc.BetweenMinor(intersection, thatArc)) return intersection;
            if (Arc.BetweenMinor(intersection, this) && Arc.BetweenMinor(intersection, thatArc)) return intersection;

            return null;
        }

        public void GetRadii(out Segment radius1, out Segment radius2)
        {
            radius1 = this.theCircle.GetRadius(new Segment(this.theCircle.center, this.endpoint1));
            radius2 = this.theCircle.GetRadius(new Segment(this.theCircle.center, this.endpoint2));

            if (radius1 == null && radius2 != null)
            {
                radius1 = radius2;
                radius2 = null;
            }
        }

        public override void FindIntersection(Arc that, out Point inter1, out Point inter2)
        {
            // Find the points of intersection
            this.theCircle.FindIntersection(that.theCircle, out inter1, out inter2);

            // The points must be on this minor arc.
            if (this is MinorArc)
            {
                if (!Arc.BetweenMinor(inter1, this)) inter1 = null;
                if (!Arc.BetweenMinor(inter2, this)) inter2 = null;
            }
            else
            {
                if (!Arc.BetweenMajor(inter1, this)) inter1 = null;
                if (!Arc.BetweenMajor(inter2, this)) inter2 = null;
            }

            // The points must be on thatArc
            if (that is MinorArc)
            {
                if (!Arc.BetweenMinor(inter1, that)) inter1 = null;
                if (!Arc.BetweenMinor(inter2, that)) inter2 = null;
            }
            else
            {
                if (!Arc.BetweenMajor(inter1, that)) inter1 = null;
                if (!Arc.BetweenMajor(inter2, that)) inter2 = null;
            }

            if (inter1 == null && inter2 != null)
            {
                inter1 = inter2;
                inter2 = null;
            }
        }

        public override void FindIntersection(Segment that, out Point inter1, out Point inter2)
        {
            // Find the points of intersection
            this.theCircle.FindIntersection(that, out inter1, out inter2);

            // The points must be on this minor arc.
            if (this is MinorArc)
            {
                if (!Arc.BetweenMinor(inter1, this)) inter1 = null;
                if (!Arc.BetweenMinor(inter2, this)) inter2 = null;
            }
            else if (this is MajorArc)
            {
                if (!Arc.BetweenMajor(inter1, this)) inter1 = null;
                if (!Arc.BetweenMajor(inter2, this)) inter2 = null;
            }
            else if (this is Semicircle)
            {
                if (!(this as Semicircle).PointLiesOn(inter1)) inter1 = null;
                if (!(this as Semicircle).PointLiesOn(inter2)) inter2 = null;
            }

            if (!that.PointLiesOnAndBetweenEndpoints(inter1)) inter1 = null;
            if (!that.PointLiesOnAndBetweenEndpoints(inter2)) inter2 = null;

            if (inter1 == null && inter2 != null)
            {
                inter1 = inter2;
                inter2 = null;
            }
        }

        public virtual bool Covers(Segment that)
        {
            return (this.HasEndpoint(that.Point1) && this.HasEndpoint(that.Point2));
        }

        public bool Covers(Arc that)
        {
            return this.PointLiesStrictlyOn(that.endpoint1) || this.PointLiesStrictlyOn(that.endpoint1);
        }
    }
}