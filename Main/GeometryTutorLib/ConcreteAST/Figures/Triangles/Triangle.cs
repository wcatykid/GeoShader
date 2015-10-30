using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public partial class Triangle : Polygon
    {
        public Point Point1 { get; private set; }
        public Point Point2 { get; private set; }
        public Point Point3 { get; private set; }

        public Segment SegmentA { get; private set; }
        public Segment SegmentB { get; private set; }
        public Segment SegmentC { get; private set; }
        public Angle AngleA { get; private set; }
        public Angle AngleB { get; private set; }
        public Angle AngleC { get; private set; }
        protected bool isRight;
        public bool givenRight { get; set; }
        public bool provenRight { get; protected set; }
        public Angle rightAngle { get; protected set; }
        protected bool isIsosceles;
        public bool provenIsosceles { get; protected set; }
        protected bool isEquilateral;
        public bool provenEquilateral { get; protected set; }
        private List<Triangle> congruencePairs;
        public bool WasDeducedCongruent(Triangle that) { return congruencePairs.Contains(that); }
        private List<Triangle> similarPairs;
        public bool WasDeducedSimilar(Triangle that) { return similarPairs.Contains(that); }

        public override bool Strengthened() { return provenIsosceles || provenRight || provenEquilateral; }

        /// <summary>
        /// Create a new triangle bounded by the 3 given segments. The set of points that define these segments should have only 3 distinct elements.
        /// </summary>
        /// <param name="a">The segment opposite point a</param>
        /// <param name="b">The segment opposite point b</param>
        /// <param name="c">The segment opposite point c</param>
        public Triangle(Segment a, Segment b, Segment c) : base(a, b, c)
        {
            SegmentA = a;
            SegmentB = b;
            SegmentC = c;

            Point1 = SegmentA.Point1;
            Point2 = SegmentA.Point2;
            Point3 = Point1.Equals(SegmentB.Point1) || Point2.Equals(SegmentB.Point1) ? SegmentB.Point2 : SegmentB.Point1;

            AngleA = new Angle(Point1, Point2, Point3);
            AngleB = new Angle(Point2, Point3, Point1);
            AngleC = new Angle(Point3, Point1, Point2);

            isRight = isRightTriangle();
            provenRight = false;
            givenRight = false;
            isIsosceles = IsIsosceles();
            provenIsosceles = false;
            isEquilateral = IsEquilateral();
            provenEquilateral = false;

            congruencePairs = new List<Triangle>();
            similarPairs = new List<Triangle>();

            addSuperFigureToDependencies();
        }

        public Triangle(Point a, Point b, Point c) : this(new Segment(a, b), new Segment(b, c), new Segment(a, c)) { }
        public Triangle(List<Point> pts) : this(pts[0], pts[1], pts[2]) { }
        public Triangle(List<Segment> segs) : this(segs[0], segs[1], segs[2])
        {
            if (segs.Count != 3) throw new ArgumentException("Triangle constructed with " + segs.Count + " segments.");
        }

        public override void DumpXML(Action<string, List<GroundedClause>> write)
        {
            List<GroundedClause> children = new List<GroundedClause>();
            foreach (Point pt in points)
            {
                children.Add(pt);
            }
            write("Triangle", children);
	    }

        protected void addSuperFigureToDependencies()
        {
            Utilities.AddUniqueStructurally(SegmentA.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(SegmentB.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(SegmentC.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point1.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point2.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(Point3.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(AngleA.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(AngleB.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(AngleC.getSuperFigures(), this);
        }

        public void SetProvenToBeRight() { provenRight = true; }
        public void SetProvenToBeIsosceles() { provenIsosceles = true; } 
        public void SetProvenToBeEquilateral() { provenEquilateral = true; }


        //
        //
        // Given 1 side of a right triangle and an angle, calculate the other 2 sides.
        //
        //
        private List<KeyValuePair<Segment, double>> CalcSidesHypotenuseKnown(RightTriangle tri, Angle knownAngle, double knownAngleVal, Segment hypotenuse, double hypotVal)
        {
            List<KeyValuePair<Segment, double>> pairs = new List<KeyValuePair<Segment, double>>();

            double oppSideLength = hypotVal * Math.Sin(Angle.toRadians(knownAngleVal));
            pairs.Add(new KeyValuePair<Segment,double>(tri.GetOppositeSide(knownAngle), oppSideLength));

            double adjSideLength = hypotVal * Math.Cos(Angle.toRadians(knownAngleVal));
            pairs.Add(new KeyValuePair<Segment, double>(knownAngle.OtherRay(hypotenuse), adjSideLength));

            return pairs;
        }
        private List<KeyValuePair<Segment, double>> CalcSidesHypotenuseUnknown(RightTriangle tri, Angle knownAngle, double knownAngleVal, Segment knownSide, double sideVal)
        {
            List<KeyValuePair<Segment, double>> pairs = new List<KeyValuePair<Segment, double>>();

            Segment hypotenuse = tri.GetHypotenuse();
            Segment oppSideOfAngle = tri.GetOppositeSide(knownAngle);
            Segment adjacent = knownAngle.OtherRay(hypotenuse);

            if (oppSideOfAngle.StructurallyEquals(knownSide))
            {
                double adjVal = sideVal / Math.Tan(Angle.toRadians(knownAngleVal));

                pairs.Add(new KeyValuePair<Segment, double>(adjacent, adjVal));
                pairs.Add(new KeyValuePair<Segment, double>(hypotenuse, Math.Sqrt(sideVal * sideVal + adjVal * adjVal)));
            }
            else if (adjacent.StructurallyEquals(knownSide))
            {
                double oppVal = sideVal * Math.Tan(Angle.toRadians(knownAngleVal));

                pairs.Add(new KeyValuePair<Segment, double>(oppSideOfAngle, oppVal));
                pairs.Add(new KeyValuePair<Segment, double>(hypotenuse, Math.Sqrt(sideVal * sideVal + oppVal * oppVal)));
            }

            return pairs;
        }
        private List<KeyValuePair<Segment, double>> CalcSides(RightTriangle tri, Angle rightAngle, Angle knownAngle, double knownAngleVal, Segment knownSeg, double knownSegVal)
        {
            //
            // Determine the nature of the known Segment w.r.t. to the known angle.
            //
            Segment hypotenuse = tri.GetHypotenuse();
            
            // Hypotenuse known
            if (knownSeg.StructurallyEquals(hypotenuse))
            {
                return CalcSidesHypotenuseKnown(tri, knownAngle, knownAngleVal, hypotenuse, knownSegVal);
            }
            else
            {
                return CalcSidesHypotenuseUnknown(tri, knownAngle, knownAngleVal, knownSeg, knownSegVal);
            }
        }

        public List<KeyValuePair<Segment, double>> RightTriangleTrigApplies(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            List<KeyValuePair<Segment, double>> pairs = new List<KeyValuePair<Segment, double>>();

            if (!(this is RightTriangle) && !this.provenRight) return pairs;

            //
            // Make a right 
            //
            RightTriangle rightTri = new RightTriangle(this);
            Angle otherAngle1, otherAngle2;
            Angle rightAngle = rightTri.rightAngle;
            rightTri.GetOtherAngles(rightAngle, out otherAngle1, out otherAngle2);

            double angleMeasure1 = known.GetAngleMeasure(otherAngle1);
            double angleMeasure2 = known.GetAngleMeasure(otherAngle2);

            // Need to know one side length
            if (angleMeasure1 < 0 && angleMeasure2 < 0) return pairs;

            double knownSegVal = -1;
            Segment knownSeg = null;
            foreach (Segment side in orderedSides)
            {
                knownSegVal = known.GetSegmentLength(side);
                if (knownSegVal > 0)
                {
                    knownSeg = side;
                    break;
                }
            }

            // Need to know one side length
            if (knownSegVal < 0) return pairs;

            // Need at least one measure.
            if (angleMeasure1 > 0) return CalcSides(rightTri, rightAngle, otherAngle1, angleMeasure1, knownSeg, knownSegVal);
            if (angleMeasure2 > 0) return CalcSides(rightTri, rightAngle, otherAngle2, angleMeasure2, knownSeg, knownSegVal);

            return pairs;
        }

        //
        // Given known values, can the third side be determined: isosceles right triangle (with base known)
        //
        public List<KeyValuePair<Segment, double>> IsoscelesRightApplies(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            List<KeyValuePair<Segment, double>> pairs = new List<KeyValuePair<Segment, double>>();

            if (!this.provenIsosceles || !this.provenRight) return pairs;

            //
            // Make an isosceles triangle to acquire segments.
            //
            IsoscelesTriangle isoTri = new IsoscelesTriangle(this);
            Segment baseSeg = isoTri.baseSegment;

            double baseVal = known.GetSegmentLength(baseSeg);

            if (baseVal < 0) return pairs;

            // Compute the value of the other sides.
            double otherSideVal = Math.Sqrt(Math.Pow(baseVal, 2) / 2.0);

            // Get the other sides.
            Segment otherSide1, otherSide2;
            isoTri.GetOtherSides(baseSeg, out otherSide1, out otherSide2);

            if (!Utilities.CompareValues(otherSide1.Length, otherSideVal))
            {
                System.Diagnostics.Debug.WriteLine("Error in known measurements.");
            }
            if (!Utilities.CompareValues(otherSide2.Length, otherSideVal))
            {
                System.Diagnostics.Debug.WriteLine("Error in known measurements.");
            }

            pairs.Add(new KeyValuePair<Segment,double>(otherSide1, otherSideVal));
            pairs.Add(new KeyValuePair<Segment, double>(otherSide2, otherSideVal));

            return pairs;
        }

        //
        // Given known values, can the third side be determined: isosceles right triangle (with base known)
        //
        public List<KeyValuePair<Segment, double>> CalculateBaseOfIsosceles(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            List<KeyValuePair<Segment, double>> pairs = new List<KeyValuePair<Segment, double>>();

            if (!this.provenIsosceles || !(this is IsoscelesTriangle)) return pairs;

            //
            // Make an isosceles triangle to acquire segments.
            //
            IsoscelesTriangle isoTri = new IsoscelesTriangle(this);
            Segment baseSeg = isoTri.baseSegment;

            double baseVal = known.GetSegmentLength(baseSeg);
            if (baseVal > 0) return pairs;

            //
            // Get the other sides.
            //
            Segment otherSide1, otherSide2;
            isoTri.GetOtherSides(baseSeg, out otherSide1, out otherSide2);

            // If we know 1 we should know the other, check anyway
            double otherVal1 = known.GetSegmentLength(otherSide1);
            double otherVal2 = known.GetSegmentLength(otherSide2);

            if (otherVal1 < 0 && otherVal2 < 0) return pairs;

            //
            // Get the base angle.
            //
            double baseAngleVal = known.GetAngleMeasure(isoTri.baseAngleOppositeLeg1);
            if (baseAngleVal < 0) baseAngleVal = known.GetAngleMeasure(isoTri.baseAngleOppositeLeg2);

            if (baseAngleVal < 0) return pairs;

            //
            // Compute the value of the base
            //
            double baseSideVal = 2.0 * otherVal1 * Math.Cos(Angle.toRadians(baseAngleVal));

            pairs.Add(new KeyValuePair<Segment, double>(baseSeg, baseSideVal));

            return pairs;
        }

        //
        // Given known values, can the third side be determined.
        //
        public KeyValuePair<Segment, double> PythagoreanTheoremApplies(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            KeyValuePair<Segment, double> nullPair = new KeyValuePair<Segment, double>(null, -1);

            if (!(this is RightTriangle) && !this.provenRight) return nullPair;

            // Acquire the known lengths and determine if 2 of 3 are known (1 unknown).
            Segment hypotenuse = GetHypotenuse();
            Segment otherSeg1, otherSeg2;
            Segment unknown = null;
            GetOtherSides(hypotenuse, out otherSeg1, out otherSeg2);

            int knownCount = 0;
            double hypLength = known.GetSegmentLength(hypotenuse);
            if (hypLength > 0) knownCount++;
            else unknown = hypotenuse;

            double other1Length = known.GetSegmentLength(otherSeg1);
            if (other1Length > 0) knownCount++;
            else unknown = otherSeg1;

            double other2Length = known.GetSegmentLength(otherSeg2);
            if (other2Length > 0) knownCount++;
            else unknown = otherSeg2;

            if (knownCount != 2) return nullPair;

            // Which side don't we know?
            // Hypotenuse unknown.
            if (hypLength < 0)
            {
                return new KeyValuePair<Segment, double>(unknown, Math.Sqrt(Math.Pow(other1Length, 2) + Math.Pow(other2Length, 2)));
            }
            else
            {
                double otherKnown = other1Length < 0 ? other2Length : other1Length;
                return new KeyValuePair<Segment, double>(unknown, Math.Sqrt(Math.Pow(hypLength, 2) - Math.Pow(otherKnown, 2)));
            }
        }

        //
        // Maintain a public repository of all triangles objects in the figure.
        //
        public static void Clear()
        {
            figureTriangles.Clear();
        }
        public static List<Triangle> figureTriangles = new List<Triangle>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Triangle) figureTriangles.Add(clause as Triangle);
        }
        public static Triangle GetFigureTriangle(Point pt1, Point pt2, Point pt3)
        {
            Triangle candTriangle = new Triangle(pt1, pt2, pt3);

            // Search for exact segment first
            foreach (Triangle tri in figureTriangles)
            {
                if (tri.StructurallyEquals(candTriangle)) return tri;
            }

            return null;
        }

        /// <summary>
        /// Determines if this is a right traingle.
        /// </summary>
        /// <returns>TRUE if this is a right triangle.</returns>
        public bool isRightTriangle()
        {
            return Utilities.CompareValues(AngleA.measure, 90) ||
                   Utilities.CompareValues(AngleB.measure, 90) ||
                   Utilities.CompareValues(AngleC.measure, 90);
        }

        /// <summary>
        /// Determines if this is an isosceles traingle.
        /// </summary>
        /// <returns>TRUE if this is an isosceles triangle.</returns>
        public bool IsIsosceles()
        {
            Segment[] segments = new Segment[3];
            segments[0] = SegmentA;
            segments[1] = SegmentB;
            segments[2] = SegmentC;

            for (int i = 0; i < segments.Length; i++)
            {
                if (Utilities.CompareValues(segments[i].Length, segments[(i + 1) % segments.Length].Length)) return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if this is an equilateral traingle.
        /// </summary>
        /// <returns>TRUE if this is an equilateral triangle.</returns>
        public bool IsEquilateral()
        {
            return Utilities.CompareValues(SegmentA.Length, SegmentB.Length) &&
                   Utilities.CompareValues(SegmentB.Length, SegmentC.Length);
        }

        public Angle GetOppositeAngle(Segment s)
        {
            Point oppVertex = this.OtherPoint(s);

            if (oppVertex.Equals(AngleA.GetVertex())) return AngleA;
            if (oppVertex.Equals(AngleB.GetVertex())) return AngleB;
            if (oppVertex.Equals(AngleC.GetVertex())) return AngleC;

            return null;
        }

        public void GetOtherAngles(Angle that, out Angle outAng1, out Angle outAng2)
        {
            outAng1 = null;
            outAng2 = null;

            if (AngleA.StructurallyEquals(that))
            {
                outAng1 = AngleB;
                outAng2 = AngleC;
            }
            else if (AngleB.StructurallyEquals(that))
            {
                outAng1 = AngleA;
                outAng2 = AngleC;
            }
            else if (AngleC.StructurallyEquals(that))
            {
                outAng1 = AngleA;
                outAng2 = AngleB;
            }
        }

        public void GetOtherSides(Segment s, out Segment outSeg1, out Segment outSeg2)
        {
            outSeg1 = null;
            outSeg2 = null;

            if (s.StructurallyEquals(SegmentA))
            {
                outSeg1 = SegmentB;
                outSeg2 = SegmentC;
            }
            else if (s.StructurallyEquals(SegmentB))
            {
                outSeg1 = SegmentA;
                outSeg2 = SegmentC;
            }
            else if (s.StructurallyEquals(SegmentC))
            {
                outSeg1 = SegmentA;
                outSeg2 = SegmentB;
            }
        }

        public Segment GetOppositeSide(Angle angle)
        {
            Point vertex = angle.GetVertex();

            if (!SegmentA.HasPoint(vertex)) return SegmentA;
            if (!SegmentB.HasPoint(vertex)) return SegmentB;
            if (!SegmentC.HasPoint(vertex)) return SegmentC;

            return null;
        }

        public Segment GetOppositeSide(Point vertex)
        {
            if (!SegmentA.HasPoint(vertex)) return SegmentA;
            if (!SegmentB.HasPoint(vertex)) return SegmentB;
            if (!SegmentC.HasPoint(vertex)) return SegmentC;

            return null;
        }

        public Segment OtherSide(Angle a)
        {
            Point vertex = a.GetVertex();

            if (!SegmentA.HasPoint(vertex)) return SegmentA;
            if (!SegmentB.HasPoint(vertex)) return SegmentB;
            if (!SegmentC.HasPoint(vertex)) return SegmentC;

            return null;
        }

        public Segment GetSegmentWithPointOnOrExtends(Point pt)
        {
            if (SegmentA.PointLiesOn(pt)) return SegmentA;
            if (SegmentB.PointLiesOn(pt)) return SegmentB;
            if (SegmentC.PointLiesOn(pt)) return SegmentC;

            return null;
        }

        public Segment GetSegmentWithPointDirectlyOn(Point pt)
        {
            if (Segment.Between(pt, SegmentA.Point1, SegmentA.Point2)) return SegmentA;
            if (Segment.Between(pt, SegmentB.Point1, SegmentB.Point2)) return SegmentB;
            if (Segment.Between(pt, SegmentC.Point1, SegmentC.Point2)) return SegmentC;

            return null;
        }

        public void MakeIsosceles()
        {
            if (!IsIsosceles())
            {
                Console.WriteLine("Deduced fact that this triangle is isosceles does not match empirical information for " + this.ToString());
            }

            provenIsosceles = true;
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public bool LiesOn(Segment cs)
        {
            return SegmentA.IsCollinearWith(cs) || SegmentB.IsCollinearWith(cs) || SegmentC.IsCollinearWith(cs);
        }

        // Does this triangle have this specific angle with these vertices
        private bool HasThisSpecificAngle(Angle ca)
        {
            return (HasSegment(ca.ray1) && HasSegment(ca.ray2)); // Could call SharedVertex
        }

        // Does the given angle correspond to any of the angles?
        public bool ExtendsAnAngle(Angle ca)
        {
            return ExtendsSpecificAngle(ca) != null;
        }

        // Does the given angle correspond to any of the angles?
        public Angle ExtendsSpecificAngle(Angle ca)
        {
            if (AngleA.Equates(ca)) return AngleA;
            if (AngleB.Equates(ca)) return AngleB;
            if (AngleC.Equates(ca)) return AngleC;

            return null;
        }

        // Does the given angle correspond to any of the angles?
        public Angle GetAngleWithVertex(Point pt)
        {
            if (AngleA.GetVertex().Equals(pt)) return AngleA;
            if (AngleB.GetVertex().Equals(pt)) return AngleB;
            if (AngleC.GetVertex().Equals(pt)) return AngleC;

            return null;
        }

        //
        // Check directly if this angle is in the triangle
        // Also check indirectly that the given angle is an extension (subangle) of this angle
        // That is, all the rays are shared and the vertex is shared, but the endpoint of rays may be different
        //
        public bool HasAngle(Angle ca)
        {
            return HasThisSpecificAngle(ca) || ExtendsAnAngle(ca);
        }

        public Segment SharesSide(Triangle cs)
        {
            if (SegmentA.Equals(cs.SegmentA) || SegmentA.Equals(cs.SegmentB) || SegmentA.Equals(cs.SegmentC))
            {
                return SegmentA;
            }
            if (SegmentB.Equals(cs.SegmentA) || SegmentB.Equals(cs.SegmentB) || SegmentB.Equals(cs.SegmentC))
            {
                return SegmentB;
            }
            if (SegmentC.Equals(cs.SegmentA) || SegmentC.Equals(cs.SegmentB) || SegmentC.Equals(cs.SegmentC))
            {
                return SegmentC;
            }

            return null;
        }

        //
        // Acquire the particular angle which belongs to this triangle (of a congruence)
        //
        public Angle AngleBelongs(CongruentAngles ccas)
        {
            if (HasAngle(ccas.ca1)) return ccas.ca1;
            if (HasAngle(ccas.ca2)) return ccas.ca2;
            return null;
        }

        //
        // Acquire the particular angle which belongs to this triangle (of a congruence)
        //
        public Angle OtherAngle(Angle thatAngle1, Angle thatAngle2)
        {
            if (AngleA.Equates(thatAngle1) && AngleB.Equates(thatAngle2) || AngleA.Equates(thatAngle2) && AngleB.Equates(thatAngle1)) return AngleC;
            if (AngleB.Equates(thatAngle1) && AngleC.Equates(thatAngle2) || AngleB.Equates(thatAngle2) && AngleC.Equates(thatAngle1)) return AngleA;
            if (AngleA.Equates(thatAngle1) && AngleC.Equates(thatAngle2) || AngleA.Equates(thatAngle2) && AngleC.Equates(thatAngle1)) return AngleB;

            return null;
        }

        public bool IsIncludedAngle(Segment s1, Segment s2, Angle a)
        {
            if (!HasSegment(s1) || !HasSegment(s2) && !HasAngle(a)) return false;

            // If the shared vertex between the segments is the vertex of this given angle, then
            // the angle is the included angle as desired
            return s1.SharedVertex(s2).Equals(a.GetVertex());
        }

        // Of the congruent pair, return the segment that applies to this triangle
        public Segment GetSegment(CongruentSegments ccss)
        {
            if (HasSegment(ccss.cs1)) return ccss.cs1;
            if (HasSegment(ccss.cs2)) return ccss.cs2;

            return null;
        }

        // Of the propportional pair, return the segment that applies to this triangle
        public Segment GetSegment(SegmentRatio prop)
        {
            if (HasSegment(prop.smallerSegment)) return prop.smallerSegment;
            if (HasSegment(prop.largerSegment)) return prop.largerSegment;

            return null;
        }

        public bool HasPoint(Point p)
        {
            if (Point1.Equals(p)) return true;
            if (Point2.Equals(p)) return true;
            if (Point3.Equals(p)) return true;

            return false;
        }
        
        public Point OtherPoint(Segment cs)
        {
            if (!cs.HasPoint(Point1)) return Point1;
            if (!cs.HasPoint(Point2)) return Point2;
            if (!cs.HasPoint(Point3)) return Point3;

            return null;
        }

        public Point OtherPoint(Point p1, Point p2)
        {
            if (SegmentA.HasPoint(p1) && SegmentA.HasPoint(p2)) return OtherPoint(SegmentA);
            if (SegmentB.HasPoint(p1) && SegmentB.HasPoint(p2)) return OtherPoint(SegmentB);
            if (SegmentC.HasPoint(p1) && SegmentC.HasPoint(p2)) return OtherPoint(SegmentC);

            return null;
        }

        public KeyValuePair<Angle, Angle> GetAcuteAngles()
        {
            if (AngleA.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleB, AngleC);
            if (AngleB.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleA, AngleC);
            if (AngleC.measure >= 90) return new KeyValuePair<Angle,Angle>(AngleA, AngleB);

            return new KeyValuePair<Angle,Angle>(null, null);
        }

        //
        // Is this angle an 'extension' of the actual triangle angle? If so, acquire the normalized version of
        // the angle, using only the triangles vertices to represent the angle
        //
        public Angle NormalizeAngle(Angle extendedAngle)
        {
            return ExtendsSpecificAngle(extendedAngle);
        }

        //
        // Returns the longest side of the triangle; arbitary choice if equal and longest
        //
        public Segment GetLongestSide()
        {
            if (SegmentA.Length > SegmentB.Length)
            {
                if (SegmentA.Length > SegmentC.Length)
                {
                    return SegmentA;
                }
            }
            else if (SegmentB.Length > SegmentC.Length)
            {
                return SegmentB;
            }

            return SegmentC;
        }

        //
        // return the hypotenuse if we know we have a right triangle
        //
        public Segment GetHypotenuse()
        {
            if (!isRight) return null;

            return GetLongestSide();
        }

        //
        // Two sides known, return the third side
        //
        public Segment OtherSide(Segment s1, Segment s2)
        {
            if (!HasSegment(s1) || !HasSegment(s2)) return null;
            if (!SegmentA.Equals(s1) && !SegmentA.Equals(s2)) return SegmentA;
            if (!SegmentB.Equals(s1) && !SegmentB.Equals(s2)) return SegmentB;
            if (!SegmentC.Equals(s1) && !SegmentC.Equals(s2)) return SegmentC;
            return null;
        }

        // 
        // Triangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C),
        //                      Angle(A, B, C), Angle(B, C, A), Angle(C, A, B)
        //
        // RightTriangle(A, B, C) -> Segment(A, B), Segment(B, C), Segment(A, C), m\angle ABC = 90^o
        //
        private static readonly string INTRINSIC_NAME = "Intrinsic";
        private static Hypergraph.EdgeAnnotation intrinsicAnnotation = new Hypergraph.EdgeAnnotation(INTRINSIC_NAME, true);

        public static List<GenericInstantiator.EdgeAggregator> Instantiate(GroundedClause c)
        {
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            Triangle tri = c as Triangle;
            if (tri == null) return newGrounded;

            // Generate the FOL for segments
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(tri);
            //tri.SegmentA.SetJustification("Intrinsic");
            //tri.SegmentB.SetJustification("Intrinsic");
            //tri.SegmentC.SetJustification("Intrinsic");
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.SegmentA, intrinsicAnnotation));
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.SegmentB, intrinsicAnnotation));
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.SegmentC, intrinsicAnnotation));

            //tri.AngleA.SetJustification("Intrinsic");
            //tri.AngleB.SetJustification("Intrinsic");
            //tri.AngleC.SetJustification("Intrinsic");
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.AngleA, intrinsicAnnotation));
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.AngleB, intrinsicAnnotation));
            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, tri.AngleC, intrinsicAnnotation));

            // If this is a right triangle, generate the FOL equation
            if (tri.provenRight)
            {
                newGrounded.AddRange(Angle.Instantiate(tri, tri.rightAngle));
            }

            return newGrounded;
        }

        //
        // Is the given angle directly exterior to this triangle?
        //
        // The triangle must share a side with the angle, the non-shared side must extend the adjacent side
        public bool HasExteriorAngle(Angle extAngle)
        {
            // Disallow any angle in this triangle (since it technically satisfies the requirements)
            if (HasAngle(extAngle)) return false;

            // Determine which angle in the triangle has the same vertex as the input angle
            Angle triangleAngle = GetAngleWithVertex(extAngle.GetVertex());

            if (triangleAngle == null) return false;

            // Acquire the ray that is shared between the angle and the triangle
            Segment sharedSegment = triangleAngle.SharedRay(extAngle);

            if (sharedSegment == null) return false;

            // Acquire the other side of the triangle
            Segment otherTriangleSegment = triangleAngle.OtherRay(sharedSegment);

            if (otherTriangleSegment == null) return false;

            // Acquire the ray that is not shared
            Segment exteriorSegment = extAngle.OtherRay(sharedSegment);

            if (exteriorSegment == null) return false;

            //           DISALLOW                                     ALLOW
            //              /                                           /
            //             / \                                         / \
            //            /TRI\                                       /TRI\
            //           /-----\                                     /-----\
            //                 /                                            \
            //                /                                              \
            //               /                                                \
            return otherTriangleSegment.IsCollinearWith(exteriorSegment);
        }

        // Determine if the given segment is coinciding with one of the triangle sides; return that 
        public KeyValuePair<Segment, Segment> OtherSides(Segment candidate)
        {
            if (SegmentA.Equals(candidate)) return new KeyValuePair<Segment,Segment>(SegmentB, SegmentC);
            if (SegmentB.Equals(candidate)) return new KeyValuePair<Segment, Segment>(SegmentA, SegmentC);
            if (SegmentC.Equals(candidate)) return new KeyValuePair<Segment, Segment>(SegmentA, SegmentB);

            return new KeyValuePair<Segment, Segment>(null, null);
        }

        // Determine if the given segment is coinciding with one of the triangle sides; return that 
        public Segment CoincidesWithASide(Segment candidate)
        {
            if (SegmentA.IsCollinearWith(candidate)) return SegmentA;
            if (SegmentB.IsCollinearWith(candidate)) return SegmentB;
            if (SegmentC.IsCollinearWith(candidate)) return SegmentC;

            return null;
        }

        // Determine if the given segment is coinciding with one of the triangle sides; return that 
        public Segment DoesParallelCoincideWith(Parallel p)
        {
            if (CoincidesWithASide(p.segment1) != null) return p.segment1;
            if (CoincidesWithASide(p.segment2) != null) return p.segment2;

            return null;
        }

        //
        // Given a point on the triangle, return the two angles not at that vertex
        //
        public void AcquireRemoteAngles(Point givenVertex, out Angle remote1, out Angle remote2)
        {
            remote1 = null;
            remote2 = null;

            if (!HasPoint(givenVertex)) return;

            if (AngleA.GetVertex().Equals(givenVertex))
            {
                remote1 = AngleB;
                remote2 = AngleC;
            }
            else if (AngleB.GetVertex().Equals(givenVertex))
            {
                remote1 = AngleA;
                remote2 = AngleC;
            }
            else
            {
                remote1 = AngleA;
                remote2 = AngleB;
            }
        }

        //
        // Have we deduced a congrunence between this triangle and t already?
        //
        public bool HasEstablishedCongruence(Triangle t)
        {
            return congruencePairs.Contains(t);
        }

        // Add to the list of congruent triangles
        public void AddCongruentTriangle(Triangle t)
        {
            congruencePairs.Add(t);
        }

        //
        // Have we deduced a similarity between this triangle and t already?
        //
        public bool HasEstablishedSimilarity(Triangle t)
        {
            return similarPairs.Contains(t);
        }

        // Add to the list of similar triangles
        public void AddSimilarTriangle(Triangle t)
        {
            similarPairs.Add(t);
        }

        public Point GetVertexOn(Segment thatSegment)
        {
            if (Segment.IntersectAtSamePoint(this.SegmentA, this.SegmentB, thatSegment)) return SegmentA.SharedVertex(SegmentB);
            if (Segment.IntersectAtSamePoint(this.SegmentA, this.SegmentC, thatSegment)) return SegmentA.SharedVertex(SegmentC);
            if (Segment.IntersectAtSamePoint(this.SegmentB, this.SegmentC, thatSegment)) return SegmentB.SharedVertex(SegmentC);

            return null;
        }

        //
        // Is this triangle congruent to the given triangle in terms of the coordinatization from the UI?
        //
        public KeyValuePair<Triangle, Triangle> CoordinateCongruent(Triangle thatTriangle)
        {
            bool[] marked = new bool[3];
            List<Segment> thisSegments = this.orderedSides;
            List<Segment> thatSegments = thatTriangle.orderedSides;

            List<Segment> corrSegmentsThis = new List<Segment>();
            List<Segment> corrSegmentsThat = new List<Segment>();

            for (int thisS = 0; thisS < thisSegments.Count; thisS++)
            {
                bool found = false;
                for (int thatS = 0; thatS < thatSegments.Count; thatS++)
                {
                    if (!marked[thatS])
                    {
                        if (thisSegments[thisS].CoordinateCongruent(thatSegments[thatS]))
                        {
                            corrSegmentsThat.Add(thatSegments[thatS]);
                            corrSegmentsThis.Add(thisSegments[thisS]);
                            marked[thatS] = true;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) return new KeyValuePair<Triangle,Triangle>(null, null);
            }

            //
            // Find the exact corresponding points
            //
            List<Point> triThis = new List<Point>();
            List<Point> triThat = new List<Point>();

            for (int i = 0; i < 3; i++)
            {
                triThis.Add(corrSegmentsThis[i].SharedVertex(corrSegmentsThis[i + 1 < 3 ? i + 1 : 0]));
                triThat.Add(corrSegmentsThat[i].SharedVertex(corrSegmentsThat[i + 1 < 3 ? i + 1 : 0]));
            }

            return new KeyValuePair<Triangle, Triangle>(new Triangle(triThis), new Triangle(triThat));
        }

        //
        // Is this triangle similar to the given triangle in terms of the coordinatization from the UI?
        //
        public bool CoordinateSimilar(Triangle thatTriangle)
        {
            bool[] congruentAngle = new bool[3];
            List<Angle> thisAngles = this.angles;
            List<Angle> thatAngles = thatTriangle.angles;

            for (int thisS = 0; thisS < thisAngles.Count; thisS++)
            {
                int thatS = 0;
                for (; thatS < thatAngles.Count; thatS++)
                {
                    if (thisAngles[thisS].CoordinateCongruent(thatAngles[thatS]))
                    {
                        congruentAngle[thisS] = true;
                        break;
                    }
                }

                if (thatS == thatAngles.Count) return false;
            }

            KeyValuePair<Triangle, Triangle> corresponding = CoordinateCongruent(thatTriangle);
            return !congruentAngle.Contains(false) && (corresponding.Key == null && corresponding.Value == null); // CTA: Congruence is stronger than Similarity
        }

        //
        // Can this triangle be strengthened to Isosceles, Equilateral, Right, or Right Isosceles?
        //
        public static List<Strengthened> CanBeStrengthened(Triangle thatTriangle)
        {
            List<Strengthened> strengthened = new List<Strengthened>();

            //
            // Equilateral cannot be strengthened
            //
            if (thatTriangle is EquilateralTriangle) return strengthened;

            if (thatTriangle is IsoscelesTriangle)
            {
                //
                // Isosceles -> Equilateral
                //
                if (thatTriangle.isEquilateral)
                {
                    strengthened.Add(new Strengthened(thatTriangle, new EquilateralTriangle(thatTriangle)));
                }

                //
                // Isosceles -> Right Isosceles
                //
                if (thatTriangle.isRight)
                {
                    strengthened.Add(new Strengthened(thatTriangle, new RightTriangle(thatTriangle)));
                }

                return strengthened;
            }

            //
            // Scalene -> Isosceles
            //
            if (thatTriangle.isIsosceles)
            {
                strengthened.Add(new Strengthened(thatTriangle, new IsoscelesTriangle(thatTriangle)));
            }

            //
            // Scalene -> Equilateral
            //
            if (thatTriangle.isEquilateral)
            {
                strengthened.Add(new Strengthened(thatTriangle, new EquilateralTriangle(thatTriangle)));
            }

            //
            // Scalene -> Right
            //
            if (!(thatTriangle is RightTriangle))
            {
                if (thatTriangle.isRight)
                {
                    strengthened.Add(new Strengthened(thatTriangle, new RightTriangle(thatTriangle)));
                }
            }

            return strengthened;
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            Triangle tri = gc as Triangle;
            if (gc == null) return false;

            //if (gc is Strengthened)
            //{
            //    return this.StructurallyEquals((gc as Strengthened).original);
            //}

            // Handles isosceles, right, or equilateral
            if (!this.StructurallyEquals(gc)) return false;

            // Ensure we know the original has been 'proven' (given) to be a particular type of triangle
            if (tri.provenIsosceles) this.provenIsosceles = true;
            if (tri.provenEquilateral) this.provenEquilateral = true;
            if (tri.provenRight) this.provenRight = true;

            return true;
        }

        public bool CoordinateMedian(Segment thatSegment)
        {
            //
            // Two sides must intersect the median at a single point
            //
            Point midptIntersection = null;
            Point coincidingIntersection = null;
            Segment oppSide = null;
            if (Segment.IntersectAtSamePoint(SegmentA, SegmentB, thatSegment))
            {
                coincidingIntersection = SegmentA.FindIntersection(SegmentB);
                midptIntersection = SegmentC.FindIntersection(thatSegment);
                oppSide = SegmentC;
            }
            else if (Segment.IntersectAtSamePoint(SegmentA, SegmentC, thatSegment))
            {
                coincidingIntersection = SegmentA.FindIntersection(SegmentC);
                midptIntersection = SegmentB.FindIntersection(thatSegment);
                oppSide = SegmentB;
            }
            else if (Segment.IntersectAtSamePoint(SegmentB, SegmentC, thatSegment))
            {
                coincidingIntersection = SegmentB.FindIntersection(SegmentC);
                midptIntersection = SegmentA.FindIntersection(thatSegment);
                oppSide = SegmentA;
            }

            if (midptIntersection == null || oppSide == null) return false;

            // It is possible for the segment to be parallel to the opposite side; results in NaN.
            if (midptIntersection.X == double.NaN || midptIntersection.Y == double.NaN) return false;

            // The intersection must be on the potential median
            if (!thatSegment.PointLiesOnAndBetweenEndpoints(coincidingIntersection)) return false;

            // The midpoint intersection must be on the potential median
            if (!thatSegment.PointLiesOnAndBetweenEndpoints(midptIntersection)) return false;

            if (!Segment.Between(coincidingIntersection, thatSegment.Point1, thatSegment.Point2)) return false;

            if (!oppSide.PointLiesOnAndBetweenEndpoints(midptIntersection)) return false;

            // Midpoint of the remaining side needs to align
            return midptIntersection.Equals(oppSide.Midpoint());
        }

        //
        // Is this segment an altitude based on the coordinates (precomputation)
        //
        public bool CoordinateAltitude(Segment thatSegment)
        {
            //
            // Check to see if the altitude is actually one of the sides of the triangle
            //
            if (this.HasSegment(thatSegment) && this.isRight)
            {
                // Find the right angle; the altitude must be one of those segments
                if (Utilities.CompareValues(this.AngleA.measure, 90)) return AngleA.HasSegment(thatSegment);
                if (Utilities.CompareValues(this.AngleB.measure, 90)) return AngleB.HasSegment(thatSegment);
                if (Utilities.CompareValues(this.AngleC.measure, 90)) return AngleC.HasSegment(thatSegment);
            }

            //
            // Two sides must intersect the given segment at a single point
            //
            Point otherIntersection = null;
            Point thisIntersection = null;
            Segment oppSide = null;
            if (Segment.IntersectAtSamePoint(SegmentA, SegmentB, thatSegment))
            {
                thisIntersection = SegmentA.FindIntersection(SegmentB);
                otherIntersection = SegmentC.FindIntersection(thatSegment);
                oppSide = SegmentC;
            }
            if (Segment.IntersectAtSamePoint(SegmentA, SegmentC, thatSegment))
            {
                thisIntersection = SegmentA.FindIntersection(SegmentC);
                otherIntersection = SegmentB.FindIntersection(thatSegment);
                oppSide = SegmentB;
            }
            if (Segment.IntersectAtSamePoint(SegmentB, SegmentC, thatSegment))
            {
                thisIntersection = SegmentB.FindIntersection(SegmentC);
                otherIntersection = SegmentA.FindIntersection(thatSegment);
                oppSide = SegmentA;
            }

            if (otherIntersection == null || oppSide == null) return false;

            // Avoid a dangling altitude:
            //
            // |\
            // | \
            // |  \
            //     \
            // Need to make sure 'this' and the the 'other' intersection is actually on the potential altitude segment
            if (!thatSegment.PointLiesOnAndBetweenEndpoints(thisIntersection)) return false;
            if (!thatSegment.PointLiesOnAndBetweenEndpoints(otherIntersection)) return false;

            // We require a perpendicular intersection
            return Utilities.CompareValues((new Angle(thisIntersection, otherIntersection, oppSide.Point1)).measure, 90);
        }

        // Returns the exact correspondence between the triangles; <this, that>
        public Dictionary<Point, Point> PointsCorrespond(Triangle thatTriangle)
        {
            if (!this.StructurallyEquals(thatTriangle)) return null;

            List<Point> thatTrianglePts = thatTriangle.points;
            List<Point> thisTrianglePts = this.points;

            // Find the index of the first point (in this Triangle) 
            int i = 0;
            for (; i < 3; i++)
            {
                if (thisTrianglePts[0].StructurallyEquals(thatTrianglePts[i])) break;
            }

            // Sanity check; something bad happened
            if (i == 3) return null;

            Dictionary<Point, Point> correspondence = new Dictionary<Point, Point>();
            for (int j = 0; j < 3; j++)
            {
                if (!thisTrianglePts[j].StructurallyEquals(thatTrianglePts[(j + i) % 3])) return null;
                correspondence.Add(thisTrianglePts[j], thatTrianglePts[(j + i) % 3]);
            }

            return correspondence;
        }

        //
        // Area-Related Computations
        //
        // Hero's Formula.
        protected double HeroArea(double s1, double s2, double s3)
        {
            double semip = 0.5 * (s1 + s2 + s3);

            return Math.Sqrt(semip * (semip - s1) * (semip - s2) * (semip - s3));
        }
        // SAS: 1/2 a * b * sin C
        protected double TrigArea(double a, double b, double theta)
        {
            return 0.5 * a * b * Math.Sin(Angle.toRadians(theta));
        }
        // Classic: 1/2 base * height
        protected double Area(double b, double h)
        {
            return 0.5 * b * h;
        }
        protected double RationalArea(double b, double h) { return Area(b, h); }
        protected double RationalArea(double s1, double s2, double s3) { return HeroArea(s1, s2, s3); }
        public override bool IsComputableArea() { return true; }

        // Does Hero's Formula apply?
        private double HeroArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            List<double> lengths = new List<double>();

            foreach (Segment side in orderedSides)
            {
                double leng = known.GetSegmentLength(side);

                if (leng < 0) return -1;

                lengths.Add(leng);
            }

            return HeroArea(lengths[0], lengths[1], lengths[2]);
        }

        // Does the included Trig Formula apply?
        private double TrigArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            for (int s = 0; s < orderedSides.Count; s++)
            {
                double a = known.GetSegmentLength(orderedSides[s]);
                double b = known.GetSegmentLength(orderedSides[(s+1) % orderedSides.Count]);

                double theta = known.GetAngleMeasure(new Angle(orderedSides[s], orderedSides[(s+1) % orderedSides.Count]));

                if (a > 0 && b > 0 && theta > 0)
                {
                    return TrigArea(a, b, theta);
                }
            }

            return -1;
        }

        // Does the included Trig Formula apply?
        private double ClassicArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Right triangle with height being one of the sides.
            if (this is RightTriangle || this.provenRight)
            {
                Segment hypotenuse = GetHypotenuse();
                Segment otherSeg1, otherSeg2;
                GetOtherSides(hypotenuse, out otherSeg1, out otherSeg2);

                double b = known.GetSegmentLength(otherSeg1);
                double h = known.GetSegmentLength(otherSeg2);

                return b > 0 && h > 0 ? Area(b, h) : -1;
            }

            for (int s = 0; s < orderedSides.Count; s++)
            {
                double b = known.GetSegmentLength(orderedSides[s]);

                //
                // How to handle heights? Need to have altitudes for this triangle saved.
                //
                int h = 0;

                if (b > 0 && h > 0)
                {
                    return Area(b, h);
                }
            }

            return -1;
        }

        public override bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            if (ClassicArea(known) > 0) return true;

            if (TrigArea(known) > 0) return true;

            if (HeroArea(known) > 0) return true;

            return false;
        }

        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            double area = ClassicArea(known);
            if (area > 0) return area;

            area = TrigArea(known);
            if (area > 0) return area;

            return HeroArea(known);
        }


        public override bool StructurallyEquals(Object obj)
        {
            Triangle thatTriangle = obj as Triangle;
            if (thatTriangle == null) return false;

            return thatTriangle.HasPoint(this.Point1) && thatTriangle.HasPoint(this.Point2) && thatTriangle.HasPoint(this.Point3);
        }

        public override bool Equals(Object obj)
        {
            if (this == obj) return true;

            // An isosceles triangle is not the same as a scalene triangle object even if the points are the same.
            IsoscelesTriangle isoTriangle = obj as IsoscelesTriangle;
            if (isoTriangle != null && !(this is IsoscelesTriangle)) return false;

            // An isosceles triangle is not the same as a scalene triangle object even if the points are the same.
            EquilateralTriangle equiTriangle = obj as EquilateralTriangle;
            if (equiTriangle != null && !(this is IsoscelesTriangle)) return false;

            Triangle triangle = obj as Triangle;
            if (triangle == null) return false;

            // Is this a strengthened version?
            if (triangle.provenIsosceles != this.provenIsosceles) return false;

            // Is this a strenghtened version?
            if (triangle.provenRight != this.provenRight) return false;

            return StructurallyEquals(obj) && base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            if (provenEquilateral)
            {
                str.Append("Equilateral ");
            }
            else if (provenIsosceles)
            {
                str.Append("Isosceles ");
            }
            
            if (provenRight)
            {
                str.Append("Right ");
            }
            str.Append("Triangle(" + Point1.ToString() + ", " + Point2.ToString() + ", " + Point3.ToString() + ") ");
            return str.ToString();
        }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();

            if (provenEquilateral || this is EquilateralTriangle)
            {
                str.Append("Eq");
            }
            else if (provenIsosceles || this is IsoscelesTriangle)
            {
                str.Append("Iso");
            }

            if (provenRight || this is RightTriangle)
            {
                str.Append("Right");
            }

            str.Append("Tri(" + Point1.CheapPrettyString() + Point2.CheapPrettyString() + Point3.CheapPrettyString() + ")");

            return str.ToString();
        }
    }
}
