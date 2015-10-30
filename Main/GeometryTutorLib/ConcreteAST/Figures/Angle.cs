using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public partial class Angle : Figure
    {
        public Point A { get; private set; }
        public Point B { get; private set; }
        public Point C { get; private set; }
        public Segment ray1 { get; private set; }
        public Segment ray2 { get; private set; }
        public double measure { get; private set; }

        public Point GetVertex() { return B; }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            Angle other = (Angle)(this.MemberwiseClone());
            other.A = (Point)this.A.DeepCopy();
            other.B = (Point)this.B.DeepCopy();
            other.C = (Point)this.C.DeepCopy();
            other.ray1 = (Segment)this.ray1.DeepCopy();
            other.ray2 = (Segment)this.ray2.DeepCopy();

            return other;
        }

        /// <summary>
        /// Create a new angle.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        public Angle(Point a, Point b, Point c) : base()
        {
            if (a.StructurallyEquals(b) || b.StructurallyEquals(c) || a.StructurallyEquals(c))
            {
                return;
                // throw new ArgumentException("Angle constructed with redundant vertices.");
            }
            this.A = a;
            this.B = b;
            this.C = c;
            ray1 = new Segment(a, b);
            ray2 = new Segment(b, c);
            this.measure = toDegrees(findAngle(A, B, C));

            if (measure <= 0)
            {
                //System.Diagnostics.Debug.WriteLine("NO-OP");
                // throw new ArgumentException("Measure of " + this.ToString() + " is ZERO");
            }
        }

        public Angle(Segment ray1, Segment ray2) : base()
        {
            Point vertex = ray1.SharedVertex(ray2);

            if (vertex == null) throw new ArgumentException("Rays do not share a vertex: " + ray1 + " " + ray2);

            this.A = ray1.OtherPoint(vertex);
            this.B = vertex;
            this.C = ray2.OtherPoint(vertex);
            this.ray1 = ray1;
            this.ray2 = ray2;
            this.measure = toDegrees(findAngle(A, B, C));

            if (measure <= 0)
            {
                //System.Diagnostics.Debug.WriteLine("NO-OP");
//                throw new ArgumentException("Measure of " + this.ToString() + " is ZERO");
            }
        }

        public Angle(List<Point> pts) : base()
        {
            if (pts.Count != 3)
            {
                throw new ArgumentException("Angle constructed with only " + pts.Count + " vertices.");
            }

            this.A = pts[0];
            this.B = pts[1];
            this.C = pts[2];

            if (A.StructurallyEquals(B) || B.StructurallyEquals(C) || A.StructurallyEquals(C))
            {
                throw new ArgumentException("Angle constructed with redundant vertices.");
            }

            ray1 = new Segment(A, B);
            ray2 = new Segment(B, C);
            this.measure = toDegrees(findAngle(A, B, C));

            if (measure <= 0)
            {
                //throw new ArgumentException("Measure of " + this.ToString() + " is ZERO");
            }
        }

        /// <summary>
        /// Find the measure of the angle (in radians) specified by the three points.
        /// </summary>
        /// <param name="a">A point defining the angle.</param>
        /// <param name="b">A point defining the angle. This is the point the angle is actually at.</param>
        /// <param name="c">A point defining the angle.</param>
        /// <returns>The measure of the angle (in radians) specified by the three points.</returns>
        public static double findAngle(Point a, Point b, Point c)
        {
            double v1x = a.X - b.X;
            double v1y = a.Y - b.Y;
            double v2x = c.X - b.X;
            double v2y = c.Y - b.Y;
            double dotProd = v1x * v2x + v1y * v2y;
            double cosAngle = dotProd / (Point.calcDistance(a, b) * Point.calcDistance(b, c));

            // Avoid minor calculation issues and retarget the given value to specific angles. 
            // 0 or 180 degrees
            if (Utilities.CompareValues(Math.Abs(cosAngle), 1))
            {
                cosAngle = cosAngle < 0 ? -1 : 1;
            }

            // 90 degrees
            if (Utilities.CompareValues(cosAngle, 0)) cosAngle = 0;

            return Math.Acos(cosAngle);
        }

        /// <summary>
        /// Converts radians into degrees.
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>An angle in degrees</returns>
        public static double toDegrees(double radians)
        {
            return radians * 180 / System.Math.PI;
        }

        /// <summary>
        /// Converts degrees into radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>An angle in radians</returns>
        public static double toRadians(double degrees)
        {
            return degrees * System.Math.PI / 180;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public Point SameVertex(Angle ang)
        {
            return GetVertex().Equals(ang.GetVertex()) ? GetVertex() : null;
        }

        //
        // Looks for a single shared ray
        //
        public Segment SharedRay(Angle ang)
        {
            //if (ray1.Equals(ang.ray1) || ray1.Equals(ang.ray2)) return ray1;

            //if (ray2.Equals(ang.ray1) || ray2.Equals(ang.ray2)) return ray2;

            if (ray1.RayOverlays(ang.ray1) || ray1.RayOverlays(ang.ray2)) return ray1;

            if (ray2.RayOverlays(ang.ray1) || ray2.RayOverlays(ang.ray2)) return ray2;

            return null;
        }

        public Segment SharesOneRayAndHasSameVertex(Angle ang)
        {
            if (SameVertex(ang) == null) return null;

            return SharedRay(ang);
        }


        // Return the shared angle in both congruences
        public Segment IsAdjacentTo(Angle thatAngle)
        {
            if (thatAngle.IsOnInterior(this)) return null;
            if (this.IsOnInterior(thatAngle)) return null;

            //Segment shared =  SharesOneRayAndHasSameVertex(thatAngle);
            //if (shared == null) return null;

            //// Is this a scenario where one angle encompasses completely the other angle?
            //Segment otherThat = thatAngle.OtherRayEquates(shared);
            //Angle tempAngle = new Angle(shared.OtherPoint(GetVertex()), GetVertex(), this.OtherRayEquates(shared).OtherPoint(GetVertex()));

            //if (tempAngle.IsOnInterior(otherThat.OtherPoint(GetVertex())) return null; 

            return SharesOneRayAndHasSameVertex(thatAngle);
        }

        //
        // Is this point in the interior of the angle?
        //
        public bool IsOnInterior(Point pt)
        {
            //     |
            //     |
            //  x  |_____
            // Is the point on either ray such that it is outside the angle? (x in the image above)
            if (ray1.PointLiesOn(pt))
            {
                // Point between the endpoints of the ray.
                if (Segment.Between(pt, GetVertex(), ray1.OtherPoint(GetVertex()))) return true;
                // Point is on the ray, but extended in the right direction.
                return Segment.Between(ray1.OtherPoint(GetVertex()), GetVertex(), pt);
            }
            if (ray2.PointLiesOn(pt))
            {
                // Point between the endpoints of the ray.
                if (Segment.Between(pt, GetVertex(), ray2.OtherPoint(GetVertex()))) return true;
                // Point is on the ray, but extended in the right direction.
                return Segment.Between(ray2.OtherPoint(GetVertex()), GetVertex(), pt);
            }

            Angle newAngle1 = new Angle(A, GetVertex(), pt);
            Angle newAngle2 = new Angle(C, GetVertex(), pt);

            // This is an angle addition scenario, BUT not with these two angles; that is, one is contained in the other.
            if (Utilities.CompareValues(newAngle1.measure + newAngle2.measure, this.measure)) return true;

            return newAngle1.measure + newAngle2.measure <= this.measure;
        }

        //
        // Is this point in the interior of the angle?
        //
        public bool IsOnInteriorExplicitly(Point pt)
        {
            if (ray1.PointLiesOn(pt)) return false;
            if (ray2.PointLiesOn(pt)) return false;

            Angle newAngle1 = new Angle(A, GetVertex(), pt);
            Angle newAngle2 = new Angle(C, GetVertex(), pt);

            // This is an angle addition scenario, BUT not with these two angles; that is, one is contained in the other.
            if (Utilities.CompareValues(newAngle1.measure + newAngle2.measure, this.measure)) return true;

            return newAngle1.measure + newAngle2.measure <= this.measure;
        }

        //
        // Is this angle on the interior of the other?
        //
        public bool IsOnInterior(Angle thatAngle)
        {
            if (this.measure < thatAngle.measure) return false;

            return this.IsOnInterior(thatAngle.A) && this.IsOnInterior(thatAngle.B) && this.IsOnInterior(thatAngle.C);
        }

        public Point OtherPoint(Segment seg)
        {
            if (seg.HasPoint(A) && seg.HasPoint(B)) return C;
            if (seg.HasPoint(A) && seg.HasPoint(C)) return B;
            if (seg.HasPoint(B) && seg.HasPoint(C)) return A;

            if (seg.PointLiesOn(A) && seg.PointLiesOn(B)) return C;
            if (seg.PointLiesOn(A) && seg.PointLiesOn(C)) return B;
            if (seg.PointLiesOn(B) && seg.PointLiesOn(C)) return A;

            return null;
        }

        //
        // Given one ray of the angle, return the other ray
        //
        public Segment OtherRay(Segment seg)
        {
            if (ray1.Equals(seg)) return ray2;
            if (ray2.Equals(seg)) return ray1;

            return null;
        }

        //
        // Given one ray of the angle, return the other ray
        //
        public Segment OtherRayEquates(Segment seg)
        {
            if (ray1.RayOverlays(seg)) return ray2;
            if (ray2.RayOverlays(seg)) return ray1;

            if (ray1.IsCollinearWith(seg)) return ray2;
            if (ray2.IsCollinearWith(seg)) return ray1;

            return null;
        }

        //
        // Do these segments overlay this angle?
        //
        public bool IsIncludedAngle(Segment seg1, Segment seg2)
        {
            // Do not allow the same segment.
            if (seg1.StructurallyEquals(seg2)) return false;

            // Check direct inclusion
            if (seg1.Equals(ray1) && seg2.Equals(ray2) || seg1.Equals(ray2) && seg2.Equals(ray1)) return true;

            // Check overlaying angle
            Point shared = seg1.SharedVertex(seg2);

            if (shared == null) return false;

            Angle thatAngle = new Angle(seg1.OtherPoint(shared), shared, seg2.OtherPoint(shared));

            return this.Equates(thatAngle);
        }

        private static readonly int[] VALID_CONCRETE_SPECIAL_ANGLES = { 30, 45 }; // 0 , 60, 90, 120, 135, 150, 180, 210, 225, 240, 270, 300, 315, 330 }; // 15, 22.5, ...


        private static bool IsSpecialAngle(double measure)
        {
            foreach (int d in VALID_CONCRETE_SPECIAL_ANGLES)
            {
                if (Utilities.GCD((int)measure, d) == d) return true;
            }

            return false;
        }

        public override bool ContainsClause(GroundedClause target)
        {
            return this.Equals(target);
        }

        //
        // Is the given angle the same as this angle? that is, the vertex is the same and the rays coincide
        // (not necessarily with the same endpoints)
        // Can't just be collinear, must be collinear and on same side of an angle
        //
        public bool Equates(Angle thatAngle)
        {
            //if (this.Equals(thatAngle)) return true;

            // Vertices must equate
            if (!this.GetVertex().Equals(thatAngle.GetVertex())) return false;

            // Rays must originate at the vertex and emanate outward
            return (ray1.RayOverlays(thatAngle.ray1) && ray2.RayOverlays(thatAngle.ray2)) ||
                   (ray2.RayOverlays(thatAngle.ray1) && ray1.RayOverlays(thatAngle.ray2));
        }

        // Does this angle lie between the two lines? This is mainly for a parallelism check
        public bool OnInteriorOf(Intersection inter1, Intersection inter2)
        {
            Intersection angleBelongs = null;
            Intersection angleDoesNotBelong = null;

            // Determine the intersection to which the angle belongs
            if (inter1.InducesNonStraightAngle(this))
            {
                angleBelongs = inter1;
                angleDoesNotBelong = inter2;
            }
            else if (inter2.InducesNonStraightAngle(this))
            {
                angleBelongs = inter2;
                angleDoesNotBelong = inter1;
            }

            if (angleBelongs == null || angleDoesNotBelong == null) return false;

            // Make the transversal out of the points of intersection
            Segment transversal = new Segment(angleBelongs.intersect, angleDoesNotBelong.intersect);
            Segment angleRayOnTraversal = this.ray1.IsCollinearWith(transversal) ? ray1 : ray2;

            // Is the endpoint of the angle (on the transversal) between the two intersection points?
            // Or, is that same endpoint on the far end beyond the other line: the other intersection point lies between the other points
            return Segment.Between(angleRayOnTraversal.OtherPoint(this.GetVertex()), angleBelongs.intersect, angleDoesNotBelong.intersect) ||
                   Segment.Between(angleDoesNotBelong.intersect, angleBelongs.intersect, angleRayOnTraversal.OtherPoint(this.GetVertex())); 
        }

        public static List<GenericInstantiator.EdgeAggregator> Instantiate(GroundedClause pred, GroundedClause c)
        {
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            if (c is Angle) return newGrounded;

            //Angle angle = c as Angle;

            //if (IsSpecialAngle(angle.measure))
            //{
            //    GeometricAngleEquation angEq = new GeometricAngleEquation(angle, new NumericValue((int)angle.measure), "Given:tbd");
            //    List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(pred);
            //    newClauses.Add(new EdgeAggregator(antecedent, angEq));
            //}

            return newGrounded;
        }

        //
        // Maintain a public repository of all angle objects in the figure
        //
        public static void Clear()
        {
            figureAngles.Clear();
            candidateTriangles.Clear();
            knownSharedAngles.Clear();
        }
        public static List<Angle> figureAngles = new List<Angle>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Angle) figureAngles.Add(clause as Angle);
        }
        public static Angle AcquireFigureAngle(Angle thatAngle)
        {
            foreach (Angle angle in figureAngles)
            {
                if (angle.Equates(thatAngle)) return angle;
            }
            return null;
        }

        //
        // Each angle is congruent to itself; only generate if both rays are shared
        //
        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<Angle> knownSharedAngles = new List<Angle>();
        public static List<GenericInstantiator.EdgeAggregator> InstantiateReflexiveAngles(GroundedClause clause)
        {
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            Triangle newTriangle = clause as Triangle;
            if (newTriangle == null) return newGrounded;

            //
            // Compare all angles in this new triangle to all the angles in the old triangles
            //
            foreach (Triangle oldTriangle in candidateTriangles)
            {
                if (newTriangle.HasAngle(oldTriangle.AngleA))
                {
                    GenericInstantiator.EdgeAggregator newClause = GenerateAngleCongruence(newTriangle, oldTriangle.AngleA);
                    if (newClause != null) newGrounded.Add(newClause);
                }

                if (newTriangle.HasAngle(oldTriangle.AngleB))
                {
                    GenericInstantiator.EdgeAggregator newClause = GenerateAngleCongruence(newTriangle, oldTriangle.AngleB);
                    if (newClause != null) newGrounded.Add(newClause);
                }

                if (newTriangle.HasAngle(oldTriangle.AngleC))
                {
                    GenericInstantiator.EdgeAggregator newClause = GenerateAngleCongruence(newTriangle, oldTriangle.AngleC);
                    if (newClause != null) newGrounded.Add(newClause);
                }
            }

            candidateTriangles.Add(newTriangle);

            return newGrounded;
        }

        //
        // Generate the actual angle congruence
        //
        private static readonly string REFLEXIVE_ANGLE_NAME = "Reflexive Angles";
        private static Hypergraph.EdgeAnnotation reflexAnnotation = new Hypergraph.EdgeAnnotation(REFLEXIVE_ANGLE_NAME, EngineUIBridge.JustificationSwitch.REFLEXIVE);

        public static GenericInstantiator.EdgeAggregator GenerateAngleCongruence(Triangle tri, Angle angle)
        {
            //
            // If we have already generated a reflexive congruence, avoid regenerating
            //
            foreach (Angle oldSharedAngle in knownSharedAngles)
            {
                if (oldSharedAngle.Equates(angle)) return null;
            }

            // Generate
            GeometricCongruentAngles gcas = new GeometricCongruentAngles(angle, angle);

            // This is an 'obvious' notion so it should be intrinsic to any figure
            gcas.MakeIntrinsic();

            return new GenericInstantiator.EdgeAggregator(Utilities.MakeList<GroundedClause>(Angle.AcquireFigureAngle(angle)), gcas, reflexAnnotation);
        }

        public bool IsComplementaryTo(Angle thatAngle)
        {
            return Utilities.CompareValues(this.measure + thatAngle.measure, 90);
        }

        public bool IsSupplementaryTo(Angle thatAngle)
        {
            return Utilities.CompareValues(this.measure + thatAngle.measure, 180);
        }

        public bool IsStraightAngle()
        {
            return ray1.IsCollinearWith(ray2);
        }

        // Checking for structural equality (is it the same segment) excluding the multiplier
        // This is either a direct comparison of the angle based on vertices or 
        public override bool StructurallyEquals(object obj)
        {
            Angle angle = obj as Angle;
            if (angle == null) return false;

            // Measures better be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            if (Equates(angle)) return true;

            return (angle.A.StructurallyEquals(A) && angle.B.StructurallyEquals(B) && angle.C.StructurallyEquals(C)) ||
                   (angle.A.StructurallyEquals(C) && angle.B.StructurallyEquals(B) && angle.C.StructurallyEquals(A));
        }

        //Checking for equality of angles WITHOUT considering possible overlay of rays (i.e. two angles will be considered NOT equal
        //if they contain rays that coincide but are not equivalent)
        public bool EqualRays(object obj)
        {
            Angle angle = obj as Angle;
            if (angle == null) return false;

            // Measures better be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            return (angle.A.StructurallyEquals(A) && angle.B.StructurallyEquals(B) && angle.C.StructurallyEquals(C)) ||
                   (angle.A.StructurallyEquals(C) && angle.B.StructurallyEquals(B) && angle.C.StructurallyEquals(A));
        }

        //
        // Is this angle congruent to the given angle in terms of the coordinatization from the UI?
        //
        public bool CoordinateCongruent(Angle a)
        {
            return Utilities.CompareValues(a.measure, this.measure);
        }

        public bool CoordinateAngleBisector(Segment thatSegment)
        {
            if (!thatSegment.PointLiesOnAndBetweenEndpoints(this.GetVertex())) return false;

            if (thatSegment.IsCollinearWith(this.ray1) || thatSegment.IsCollinearWith(this.ray2)) return false;

            Point interiorPoint = this.IsOnInteriorExplicitly(thatSegment.Point1) ? thatSegment.Point1 : thatSegment.Point2;
            if (!this.IsOnInteriorExplicitly(interiorPoint)) return false;

            Angle angle1 = new Angle(A, GetVertex(), interiorPoint);
            Angle angle2 = new Angle(C, GetVertex(), interiorPoint);

            return Utilities.CompareValues(angle1.measure, angle2.measure);
        }

        //
        // Is this angle proportional to the given segment in terms of the coordinatization from the UI?
        // We should not report proportional if the ratio between segments is 1
        //
        public KeyValuePair<int, int> CoordinateProportional(Angle a)
        {
            return Utilities.RationalRatio(a.measure, this.measure);
        }

        public bool HasPoint(Point p)
        {
            if (A.Equals(p)) return true;
            if (B.Equals(p)) return true;
            if (C.Equals(p)) return true;

            return false;
        }

        public bool HasSegment(Segment seg)
        {
            return ray1.RayOverlays(seg) || ray2.RayOverlays(seg);
        }

        // CTA: Be careful with equality; this is object-based equality
        // If we check for angle measure equality that is distinct.
        // If we check to see that a different set of remote vertices describes this angle, that is distinct.
        public override bool Equals(Object obj)
        {
            Angle angle = obj as Angle;
            if (angle == null) return false;

            // Measures must be the same.
            if (!Utilities.CompareValues(this.measure, angle.measure)) return false;

            return base.Equals(obj) && StructurallyEquals(obj);
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            RightAngle ra = gc as RightAngle;

            if (ra == null) return false;

            return this.StructurallyEquals(ra);
        }

        public override string ToString()
        {
            return "Angle( m" + A.name + B.name + C.name + " = " + string.Format("{0:N3}", measure) + ")";
        }

        public override string ToPrettyString()
        {
            return "Angle " + A.name + B.name + C.name;
        }
    }
}
