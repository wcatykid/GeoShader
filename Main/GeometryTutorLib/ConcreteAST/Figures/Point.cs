using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// A 2D point
    /// </summary>
    public partial class Point : Figure
    {
        private static int CURRENT_ID = 0;

        public double X { get; private set; }
        public double Y { get; private set; }
        public static readonly double EPSILON = 0.0001;

        /// <summary> A unique identifier for this point. </summary>
        public int ID { get; private set; }

        public string name { get; private set; }

        /// <summary>
        /// Create a new ConcretePoint with the specified coordinates.
        /// </summary>
        /// <param name="name">The name of the point. (Assigned by the UI)</param>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        public Point(string n, double x, double y) : base()
        {
            this.ID = CURRENT_ID++;
            name = n != null ? n : "";
            this.X = x;
            this.Y = y;
        }

        //
        // Expects a radian angle measurement.
        //
        public static Point GetPointFromAngle(Point center, double radius, double angle)
        {
            return new Point("", center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
        }

        //
        // Assumes our points represent vectors in std position
        //
        public static double CrossProduct(Point thisPoint, Point thatPoint)
        {
            return thisPoint.X * thatPoint.Y - thisPoint.Y * thatPoint.X;
        }

        //
        // Assumes our points represent vectors in std position
        //
        public static bool OppositeVectors(Point first, Point second)
        {
            Point origin = new Point("", 0, 0);

            return Segment.Between(origin, first, second);
        }

        //
        // Angle measure (in degrees) between two vectors in standard position.
        //
        public static double AngleBetween(Point thisPoint, Point thatPoint)
        {
            Point origin = new Point("", 0, 0);

            if (Segment.Between(origin, thisPoint, thatPoint)) return 180;
            if (Segment.Between(thisPoint, origin, thatPoint)) return 0;
            if (Segment.Between(thatPoint, origin, thisPoint)) return 0;
            
            return new Angle(thisPoint, origin, thatPoint).measure;
        }

        public static bool CounterClockwise(Point A, Point B, Point C)
        {
            // Define two vectors: vect1: A----->B
            //                     vect2: B----->C
            // Cross product vect1 and vect2. 
            // If the result is negative, the sequence A-->B-->C is Counter-clockwise. 
            // If the result is positive, the sequence A-->B-->C is clockwise.
            Point vect1 = Point.MakeVector(A, B);
            Point vect2 = Point.MakeVector(B, C);

            return Point.CrossProduct(vect1, vect2) < 0;
        }

        public static double Magnitude(Point vector) { return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2)); }
        public static Point MakeVector(Point tail, Point head) { return new Point("", head.X - tail.X, head.Y - tail.Y); }
        public static Point GetOppositeVector(Point v) { return new Point("", -v.X, -v.Y); }
        public static Point Normalize(Point vector)
        {
            double magnitude = Point.Magnitude(vector);
            return new Point("", vector.X / magnitude, vector.Y / magnitude);
        }
        public static Point ScalarMultiply(Point vector, double scalar) { return new Point("", scalar * vector.X, scalar * vector.Y); }

        public int Quadrant()
        {
            if (Utilities.CompareValues(X, 0) && Utilities.CompareValues(Y, 0)) return 0;
            if (Utilities.GreaterThan(X, 0) && Utilities.GreaterThan(Y, 0)) return 1;
            if (Utilities.CompareValues(X, 0) && Utilities.GreaterThan(Y, 0)) return 12;
            if (Utilities.LessThan(X, 0) && Utilities.GreaterThan(Y, 0)) return 2;
            if (Utilities.LessThan(X, 0) && Utilities.CompareValues(Y, 0)) return 23;
            if (Utilities.LessThan(X, 0) && Utilities.CompareValues(Y, 0)) return 3;
            if (Utilities.CompareValues(X, 0) && Utilities.LessThan(Y, 0)) return 34;
            if (Utilities.GreaterThan(X, 0) && Utilities.LessThan(Y, 0)) return 4;
            if (Utilities.GreaterThan(X, 0) && Utilities.CompareValues(Y, 0)) return 41;

            return -1;
        }

        //
        // Returns a degree angle measurement between [0, 360]. 
        //
        public static double GetDegreeStandardAngleWithCenter(Point center, Point other)
        {
            return GetRadianStandardAngleWithCenter(center, other) / Math.PI * 180;
        }

        //
        // Returns a radian angle measurement between [0, 2PI]. 
        //
        public static double GetRadianStandardAngleWithCenter(Point center, Point other)
        {
            Point stdVector = new Point("", other.X - center.X, other.Y - center.Y);

            double angle = System.Math.Atan2(stdVector.Y, stdVector.X);

            return angle < 0 ? angle + 2 * Math.PI : angle;
        }

        //
        // Maintain a public repository of all segment objects in the figure.
        //
        public static void Clear()
        {
            figurePoints.Clear();
        }
        public static List<Point> figurePoints = new List<Point>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Point) figurePoints.Add(clause as Point);
        }
        public static Point GetFigurePoint(Point candPoint)
        {
            foreach (Point p in figurePoints)
            {
                if (p.StructurallyEquals(candPoint)) return p;
            }

            return null;
        }

        /// <summary>
        /// Find the distance between two points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">Tjhe second point</param>
        /// <returns>The distance between the two points</returns>
        public static double calcDistance(Point p1, Point p2)
        {
            return System.Math.Sqrt(System.Math.Pow(p2.X - p1.X, 2) + System.Math.Pow(p2.Y - p1.Y, 2));
        }

        //
        // One-dimensional betweeness
        //
        public static bool Between(double val, double a, double b)
        {
            if (a >= val && val <= b) return true;
            if (b >= val && val <= a) return true;

            return false;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Point pt = obj as Point;

            if (pt == null) return false;
            return Utilities.CompareValues(pt.X, X) && Utilities.CompareValues(pt.Y, Y);
        }

        public override bool Equals(Object obj)
        {
            Point pt = obj as Point;

            if (pt == null) return false;

            return StructurallyEquals(obj); // && name.Equals(pt.name);
        }

        public override int GetHashCode() { return (int)(X * Y * 100);  }

        // Make a deep copy of this object; this is actually shallow, but is all that is required.
        public override GroundedClause DeepCopy() { return (Point)(this.MemberwiseClone()); }

        public override string ToString() { return name + "(" + string.Format("{0:N3}", X) + ", " + string.Format("{0:N3}", Y) + ")"; }

        public override string ToPrettyString() { return name; }
        
        public string SimpleToString()
        {
            if (name == "") return "(" + string.Format("{0:N1}", X) + ", " + string.Format("{0:N1}", Y) + ")";
            else return name;
        }
        public override string CheapPrettyString() { return SimpleToString(); }

        /// <summary>
        /// p1 < p2 : -1
        /// p1 == p2 : 0
        /// p1 > p2 : 1
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int LexicographicOrdering(Point p1, Point p2)
        {
            if (!Utilities.CompareValues(p1.X, p2.X))
            {
                // X's first
                if (p1.X < p2.X) return -1;

                if (p1.X > p2.X) return 1;
            }

            if (Utilities.CompareValues(p1.Y, p2.Y)) return 0;

            // Y's second
            if (p1.Y < p2.Y) return -1;

            if (p1.Y > p2.Y) return 1;

            // Equal points
            return 0;
        }
    }
}
