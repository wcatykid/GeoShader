using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a quadrilateral (defined by 4 segments)
    /// </summary>
    public partial class Quadrilateral : Polygon
    {
        public Point topLeft { get; private set; }
        public Point topRight { get; private set; }
        public Point bottomLeft { get; private set; }
        public Point bottomRight { get; private set; }

        public Segment left { get; private set; }
        public Segment right { get; private set; }
        public Segment top { get; private set; }
        public Segment bottom { get; private set; }

        public Angle topLeftAngle { get; private set; }
        public Angle topRightAngle { get; private set; }
        public Angle bottomLeftAngle { get; private set; }
        public Angle bottomRightAngle { get; private set; }

        //
        // Diagonals
        //
        public Intersection diagonalIntersection { get; private set; }
        public void SetIntersection(Intersection diag) { diagonalIntersection = diag; }

        public Segment topLeftBottomRightDiagonal { get; private set; }
        private bool tlbrDiagonalValid = true;
        public bool TopLeftDiagonalIsValid() { return tlbrDiagonalValid; }
        public void SetTopLeftDiagonalInValid() { tlbrDiagonalValid = false; }
        private KeyValuePair<Triangle, Triangle> triPairTLBR;

        public Segment bottomLeftTopRightDiagonal { get; private set; }
        private bool bltrDiagonalValid = true;
        public bool BottomRightDiagonalIsValid() { return bltrDiagonalValid; }
        public void SetBottomRightDiagonalInValid() { bltrDiagonalValid = false; }
        private KeyValuePair<Triangle, Triangle> triPairBLTR;

        public Quadrilateral(Segment left, Segment right, Segment top, Segment bottom) : base()
        {
            //
            // Segments
            //
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;

            orderedSides = new List<Segment>();
            orderedSides.Add(left);
            orderedSides.Add(top);
            orderedSides.Add(right);
            orderedSides.Add(bottom);

            //
            // Points
            //
            this.topLeft = left.SharedVertex(top);
            if (topLeft == null)
            {
                return;
                // throw new ArgumentException("Top left point is invalid: " + top + " " + left);
            }
            this.topRight = right.SharedVertex(top);
            if (topRight == null) throw new ArgumentException("Top left point is invalid: " + top + " " + right);

            this.bottomLeft = left.SharedVertex(bottom);
            if (bottomLeft == null) throw new ArgumentException("Bottom left point is invalid: " + bottom + " " + left);

            this.bottomRight = right.SharedVertex(bottom);
            if (bottomRight == null) throw new ArgumentException("Bottom right point is invalid: " + bottom + " " + right);

            points = new List<Point>();
            points.Add(topLeft);
            points.Add(topRight);
            points.Add(bottomRight);
            points.Add(bottomLeft);

            // Verify that we have 4 unique points
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (points[i].StructurallyEquals(points[j]))
                    {
                        throw new ArgumentException("Points of quadrilateral are not distinct: " + points[i] + " " + points[j]);
                    }
                }
            }

            //
            // Diagonals
            //
            this.topLeftBottomRightDiagonal = new Segment(topLeft, bottomRight);
            this.bottomLeftTopRightDiagonal = new Segment(bottomLeft, topRight);
            this.diagonalIntersection = null;
            triPairTLBR = new KeyValuePair<Triangle, Triangle>(new Triangle(topLeft, bottomLeft, bottomRight), new Triangle(topLeft, topRight, bottomRight));
            triPairBLTR = new KeyValuePair<Triangle, Triangle>(new Triangle(bottomLeft, topLeft, topRight), new Triangle(bottomLeft, bottomRight, topRight));

            //
            // Angles
            //
            this.topLeftAngle = new Angle(bottomLeft, topLeft, topRight);
            this.topRightAngle = new Angle(topLeft, topRight, bottomRight);
            this.bottomRightAngle = new Angle(topRight, bottomRight, bottomLeft);
            this.bottomLeftAngle = new Angle(bottomRight, bottomLeft, topLeft);

            angles = new List<Angle>();
            angles.Add(topLeftAngle);
            angles.Add(topRightAngle);
            angles.Add(bottomLeftAngle);
            angles.Add(bottomRightAngle);

            this.FigureSynthesizerConstructor();

            addSuperFigureToDependencies();
        }

        public Quadrilateral(List<Segment> segs) : this(segs[0], segs[1], segs[2], segs[3])
        {
            if (segs.Count != 4) throw new ArgumentException("Quadrilateral constructed with " + segs.Count + " segments.");
        }

        public static Quadrilateral MakeQuadrilateral(Point a, Point b, Point c, Point d)
        {
            Segment left = new Segment(a, d);
            Segment right = new Segment(b, c);
            Segment top = new Segment(a, b);
            Segment bottom = new Segment(c, d);

            Quadrilateral quad = null;
            try
            {
                quad = new Quadrilateral(left, right, top, bottom);
            }
            catch (Exception)
            {
                left = new Segment(a, d);
                right = new Segment(b, c);
                top = new Segment(a, c);
                bottom = new Segment(b, d);

                quad = new Quadrilateral(left, right, top, bottom);
            }

            return quad;
        }

        protected void addSuperFigureToDependencies()
        {
            Utilities.AddUniqueStructurally(topLeft.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(topRight.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomLeft.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomRight.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(left.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(right.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottom.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(top.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(topLeftAngle.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(topRightAngle.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomLeftAngle.getSuperFigures(), this);
            Utilities.AddUniqueStructurally(bottomRightAngle.getSuperFigures(), this);
        }

        public bool IsStrictQuadrilateral()
        {
            if (this is Parallelogram) return false;
            if (this is Trapezoid) return false;
            if (this is Kite) return false;

            return true;
        }

        //
        // Acquire one of the quadrilateral angles.
        //
        public Angle GetAngle(Angle thatAngle)
        {
            foreach (Angle angle in angles)
            {
                if (angle.Equates(thatAngle)) return angle;
            }

            return null;
        }

        //
        // Does this quadrilateral have the given side (exactly, no extension)?
        //
        public bool HasAngle(Angle thatAngle)
        {
            foreach (Angle angle in angles)
            {
                if (angle.Equates(thatAngle)) return true;
            }

            return false;
        }

        //
        // Does this quadrilateral have the given side (exactly, no extension)?
        //
        public bool HasSide(Segment segment)
        {
            foreach(Segment side in orderedSides)
            {
                if (side.StructurallyEquals(segment)) return true;
            }

            return false;
        }

        //
        // Does this quadrilateral contain the segment on a side?
        //
        public Segment HasSubsegmentSide(Segment segment)
        {
            foreach (Segment side in orderedSides)
            {
                if (side.HasSubSegment(segment)) return side;
            }

            return null;
        }

        //
        // Are the two given segments on the opposite sides of this quadrilateral?
        //
        public bool AreOppositeSides(Segment segment1, Segment segment2)
        {
            if (!this.HasSide(segment1) || !this.HasSide(segment2)) return false;

            if (top.StructurallyEquals(segment1) && bottom.StructurallyEquals(segment2)) return true;
            if (top.StructurallyEquals(segment2) && bottom.StructurallyEquals(segment1)) return true;

            if (left.StructurallyEquals(segment1) && right.StructurallyEquals(segment2)) return true;
            if (left.StructurallyEquals(segment2) && right.StructurallyEquals(segment1)) return true;

            return false;
        }

        //
        // Are the two given segments on the opposite sides of this quadrilateral?
        //
        public bool AreOppositeSubsegmentSides(Segment segment1, Segment segment2)
        {
            if (this.HasSubsegmentSide(segment1) == null || this.HasSubsegmentSide(segment2) == null) return false;

            if (top.HasSubSegment(segment1) && bottom.HasSubSegment(segment2)) return true;
            if (top.HasSubSegment(segment2) && bottom.HasSubSegment(segment1)) return true;

            if (left.HasSubSegment(segment1) && right.HasSubSegment(segment2)) return true;
            if (left.HasSubSegment(segment2) && right.HasSubSegment(segment1)) return true;

            return false;
        }

        //
        // Are the two given angles on the opposite sides of this quadrilateral?
        //
        public bool AreOppositeAngles(Angle angle1, Angle angle2)
        {
            if (!this.HasAngle(angle1) || !this.HasAngle(angle2)) return false;

            if (topLeftAngle.Equates(angle1) && bottomRightAngle.Equates(angle2)) return true;
            if (topLeftAngle.Equates(angle2) && bottomRightAngle.Equates(angle1)) return true;

            if (topRightAngle.Equates(angle1) && bottomLeftAngle.Equates(angle2)) return true;
            if (topRightAngle.Equates(angle2) && bottomLeftAngle.Equates(angle1)) return true;

            return false;
        }

        //
        // Are the two given segments adjacent with this quadrilateral?
        //
        public bool AreAdjacentSides(Segment segment1, Segment segment2)
        {
            if (!this.HasSide(segment1) || !this.HasSide(segment2)) return false;

            if (top.StructurallyEquals(segment1) && left.StructurallyEquals(segment2)) return true;
            if (top.StructurallyEquals(segment2) && left.StructurallyEquals(segment1)) return true;

            if (top.StructurallyEquals(segment2) && right.StructurallyEquals(segment1)) return true;
            if (top.StructurallyEquals(segment1) && right.StructurallyEquals(segment2)) return true;

            if (bottom.StructurallyEquals(segment1) && left.StructurallyEquals(segment2)) return true;
            if (bottom.StructurallyEquals(segment2) && left.StructurallyEquals(segment1)) return true;

            if (bottom.StructurallyEquals(segment2) && right.StructurallyEquals(segment1)) return true;
            if (bottom.StructurallyEquals(segment1) && right.StructurallyEquals(segment2)) return true;

            return false;
        }

        //
        // Does this parallel set apply to this quadrilateral?
        //
        public bool HasOppositeParallelSides(Parallel parallel)
        {
            return AreOppositeSides(parallel.segment1, parallel.segment2);
        }

        //
        // Does this parallel set apply to this quadrilateral?
        //
        public bool HasOppositeParallelSubsegmentSides(Parallel parallel)
        {
            return AreOppositeSubsegmentSides(parallel.segment1, parallel.segment2);
        }

        //
        // Does this congruent pair apply to this quadrilateral?
        //
        public bool HasOppositeCongruentSides(CongruentSegments cs)
        {
            return AreOppositeSides(cs.cs1, cs.cs2);
        }

        //
        // Does this congruent pair of angles apply to this quadrilateral?
        //
        public bool HasOppositeCongruentAngles(CongruentAngles cas)
        {
            return AreOppositeAngles(cas.ca1, cas.ca2);
        }

        //
        // Does this parallel set apply to this quadrilateral?
        //
        public bool HasAdjacentCongruentSides(CongruentSegments cs)
        {
            return AreAdjacentSides(cs.cs1, cs.cs2);
        }

        //
        // Acquire the other 2 sides not in this parallel relationship; works for a n-gon (polygon) as well.
        //
        public List<Segment> GetOtherSides(List<Segment> inSegments)
        {
            List<Segment> outSegments = new List<Segment>();

            // This quadrilateral must have these given segments to return valid data.
            foreach (Segment inSeg in inSegments)
            {
                if (!this.HasSide(inSeg)) return outSegments;
            }

            //
            // Traverse given segments partitioning this quad into in / out.
            //
            foreach (Segment side in orderedSides)
            {
                if (!inSegments.Contains(side))
                {
                    outSegments.Add(side);
                }
            }

            return outSegments;
        }

        //
        // Acquire the other 2 sides not in this parallel relationship; works for a n-gon (polygon) as well.
        //
        public List<Segment> GetOtherSubsegmentSides(List<Segment> inSegments)
        {
            // This quadrilateral must have these given segments to return valid data.
            List<Segment> inSegsMappedToQuad = new List<Segment>();
            foreach (Segment inSeg in inSegments)
            {
                Segment side = this.HasSubsegmentSide(inSeg);
                if (side == null) return new List<Segment>();
                inSegsMappedToQuad.Add(side);
            }

            //
            // Traverse given segments partitioning this quad into in / out.
            //
            List<Segment> outSegments = new List<Segment>();
            foreach (Segment side in orderedSides)
            {
                if (!inSegsMappedToQuad.Contains(side))
                {
                    outSegments.Add(side);
                }
            }

            return outSegments;
        }

        //
        // Acquire the other 2 sides not in this parallel relationship.
        //
        public List<Segment> GetOtherSides(Parallel parallel)
        {
            List<Segment> segs = new List<Segment>();
            segs.Add(parallel.segment1);
            segs.Add(parallel.segment2);

            return GetOtherSides(segs);
        }

        //
        // Acquire the other 2 sides not in this parallel relationship.
        //
        public List<Segment> GetOtherSubsegmentSides(Parallel parallel)
        {
            List<Segment> segs = new List<Segment>();
            segs.Add(parallel.segment1);
            segs.Add(parallel.segment2);

            return GetOtherSubsegmentSides(segs);
        }

        //
        // Coordinate-based determination if we have a parallelogram.
        //
        public bool VerifyParallelogram()
        {
            if (!left.IsParallelWith(right)) return false;
            if (!top.IsParallelWith(bottom)) return false;

            if (!Utilities.CompareValues(left.Length, right.Length)) return false;
            if (!Utilities.CompareValues(top.Length, bottom.Length)) return false;

            if (!Utilities.CompareValues(topLeftAngle.measure, bottomRightAngle.measure)) return false;
            if (!Utilities.CompareValues(topRightAngle.measure, bottomLeftAngle.measure)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a rhombus.
        //
        public bool VerifyRhombus()
        {
            if (!VerifyParallelogram()) return false;

            if (!Utilities.CompareValues(left.Length, top.Length)) return false;
            if (!Utilities.CompareValues(left.Length, bottom.Length)) return false;

            // Redundant
            if (!Utilities.CompareValues(right.Length, top.Length)) return false;
            if (!Utilities.CompareValues(right.Length, bottom.Length)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a Square
        //
        public bool VerifySquare()
        {
            if (!VerifyRhombus()) return false;

            if (!Utilities.CompareValues(topLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(topRightAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomRightAngle.measure, 90)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a Rectangle
        //
        public bool VerifyRectangle()
        {
            if (!VerifyParallelogram()) return false;

            if (!Utilities.CompareValues(topLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(topRightAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomLeftAngle.measure, 90)) return false;
            if (!Utilities.CompareValues(bottomRightAngle.measure, 90)) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have a Trapezoid
        //
        public bool VerifyTrapezoid()
        {
            bool lrParallel = left.IsParallelWith(right);
            bool tbParallel = top.IsParallelWith(bottom);

            // XOR of parallel opposing sides
            if (lrParallel && tbParallel) return false;
            if (!lrParallel && !tbParallel) return false;

            return true;
        }

        //
        // Coordinate-based determination if we have an Isosceles Trapezoid
        //
        public bool VerifyIsoscelesTrapezoid()
        {
            if (!VerifyTrapezoid()) return false;

            //
            // Determine parallel sides, then compare lengths of other sides
            //
            if (left.IsParallelWith(right))
            {
                if (!Utilities.CompareValues(top.Length, bottom.Length)) return false;
            }
            else if (top.IsParallelWith(bottom))
            {
                if (!Utilities.CompareValues(left.Length, right.Length)) return false;
            }

            return true;
        }

        //
        // Coordinate-based determination if we have an Isosceles Trapezoid
        //
        public bool VerifyKite()
        {
            //
            // Adjacent sides must equate in length
            //
            if (Utilities.CompareValues(left.Length, top.Length) && Utilities.CompareValues(right.Length, bottom.Length)) return true;

            if (Utilities.CompareValues(left.Length, bottom.Length) && Utilities.CompareValues(right.Length, top.Length)) return true;

            return false;
        }

        //
        // Can this Quadrilateral be strengthened to any of the specific quadrilaterals (parallelogram, kite, square, etc)?
        //
        public static List<Strengthened> CanBeStrengthened(Quadrilateral thatQuad)
        {
            List<Strengthened> strengthened = new List<Strengthened>();

            if (thatQuad.VerifyParallelogram())
            {
                strengthened.Add(new Strengthened(thatQuad, new Parallelogram(thatQuad)));
            }

            if (thatQuad.VerifyRectangle())
            {
                strengthened.Add(new Strengthened(thatQuad, new Rectangle(thatQuad)));
            }

            if (thatQuad.VerifySquare())
            {
                strengthened.Add(new Strengthened(thatQuad, new Square(thatQuad)));
            }

            if (thatQuad.VerifyRhombus())
            {
                strengthened.Add(new Strengthened(thatQuad, new Rhombus(thatQuad)));
            }

            if (thatQuad.VerifyKite())
            {
                strengthened.Add(new Strengthened(thatQuad, new Kite(thatQuad)));
            }

            if (thatQuad.VerifyTrapezoid())
            {
                Trapezoid newTrap = new Trapezoid(thatQuad);
                strengthened.Add(new Strengthened(thatQuad, newTrap));

                if (thatQuad.VerifyIsoscelesTrapezoid())
                {
                    strengthened.Add(new Strengthened(newTrap, new IsoscelesTrapezoid(thatQuad)));
                }
            }

            return strengthened;
        }

        protected bool HasSamePoints(Quadrilateral quad)
        {
            foreach (Point p in quad.points)
            {
                if (!this.points.Contains(p)) return false;
            }

            return true;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Quadrilateral thatQuad = obj as Quadrilateral;
            if (thatQuad == null) return false;

            if (!thatQuad.IsStrictQuadrilateral()) return false;

            return this.HasSamePoints(thatQuad);
        }

        public override bool Equals(Object obj)
        {
            Quadrilateral thatQuad = obj as Quadrilateral;
            if (thatQuad == null) return false;

            if (!thatQuad.IsStrictQuadrilateral()) return false;

            return this.HasSamePoints(thatQuad);
        }

        //
        // Maintain a public repository of all triangles objects in the figure.
        //
        public static void Clear()
        {
            figureQuadrilaterals.Clear();
        }
        public static List<Quadrilateral> figureQuadrilaterals = new List<Quadrilateral>();
        public static void Record(GroundedClause clause)
        {
            // Record uniquely? For right angles, etc?
            if (clause is Quadrilateral) figureQuadrilaterals.Add(clause as Quadrilateral);
        }
        public static Quadrilateral GetFigureQuadrilateral(Quadrilateral q)
        {
            // Search for exact segment first
            foreach (Quadrilateral quad in figureQuadrilaterals)
            {
                if (quad.StructurallyEquals(q)) return quad;
            }

            return null;
        }

        public override string ToString()
        {
            return "Quadrilateral(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                      bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }


        //
        // generate a Quadrilateral object, if the 4 segments construct a valid quadrilateral.
        //
        public static Quadrilateral GenerateQuadrilateral(List<Segment> segments)
        {
            if (segments.Count < 4) return null;

            return GenerateQuadrilateral(segments[0], segments[1], segments[2], segments[3]);
        }
        public static Quadrilateral GenerateQuadrilateral(Segment s1, Segment s2, Segment s3, Segment s4)
        {
            //    ____
            //   |
            //   |____
            // Check a C shape of 3 segments; the 4th needs to be opposite 
            Segment top;
            Segment bottom;
            Segment left = AcquireMiddleSegment(s1, s2, s3, out top, out bottom);

            // Check C for the top, bottom, and right sides
            if (left == null) return null;

            Segment right = s4;

            Segment tempOut1, tempOut2;
            Segment rightMid = AcquireMiddleSegment(top, bottom, right, out tempOut1, out tempOut2);

            // The middle segment we acquired must match the 4th segment
            if (!right.StructurallyEquals(rightMid)) return null;

            //
            // The top / bottom cannot cross; bowtie or hourglass shape
            // A valid quadrilateral will have the intersections outside of the quad, that is defined
            // by the order of the three points: intersection and two endpts of the side
            //
            Point intersection = top.FindIntersection(bottom);

            // Check for parallel lines, then in-betweenness
            if (intersection != null && !double.IsNaN(intersection.X) && !double.IsNaN(intersection.Y))
            {
                if (Segment.Between(intersection, top.Point1, top.Point2)) return null;
                if (Segment.Between(intersection, bottom.Point1, bottom.Point2)) return null;
            }

            // The left / right cannot cross; bowtie or hourglass shape
            intersection = left.FindIntersection(right);

            // Check for parallel lines, then in-betweenness
            if (intersection != null && !double.IsNaN(intersection.X) && !double.IsNaN(intersection.Y))
            {
                if (Segment.Between(intersection, left.Point1, left.Point2)) return null;
                if (Segment.Between(intersection, right.Point1, right.Point2)) return null;
            }

            //
            // Verify that we have 4 unique points; And not different shapes (like a star, or triangle with another segment)
            //
            List<Point> pts = new List<Point>();
            pts.Add(left.SharedVertex(top));
            pts.Add(left.SharedVertex(bottom));
            pts.Add(right.SharedVertex(bottom));
            pts.Add(right.SharedVertex(top));
            for (int i = 0; i < pts.Count - 1; i++)
            {
                for (int j = i + 1; j < pts.Count; j++)
                {
                    if (pts[i].StructurallyEquals(pts[j]))
                    {
                        return null;
                    }
                }
            }

            return new Quadrilateral(left, right, top, bottom);
        }

        //            top
        // shared1  _______   off1
        //         |
        //   mid   |
        //         |_________   off2
        //            bottom
        private static Segment AcquireMiddleSegment(Segment seg1, Segment seg2, Segment seg3, out Segment top, out Segment bottom)
        {
            if (seg1.SharedVertex(seg2) != null && seg1.SharedVertex(seg3) != null)
            {
                top = seg2;
                bottom = seg3;
                return seg1;
            }

            if (seg2.SharedVertex(seg1) != null && seg2.SharedVertex(seg3) != null)
            {
                top = seg1;
                bottom = seg3;
                return seg2;
            }

            if (seg3.SharedVertex(seg1) != null && seg3.SharedVertex(seg2) != null)
            {
                top = seg1;
                bottom = seg2;
                return seg3;
            }

            top = null;
            bottom = null;

            return null;
        }


        //
        // Area-Related Computations
        //
        public override bool IsComputableArea() { return true; }

        //
        // As a general mechanism, can we split up this quadrilateral into two triangles and find those individual areas?
        // We must try two combinations of triangle splitting.
        //
        protected double SplitTriangleArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            //
            // Check the areas of each pairs, only if a diagonal is evident.
            //
            if (TopLeftDiagonalIsValid())
            {
                double area1 = triPairTLBR.Key.GetArea(known);
                double area2 = triPairTLBR.Value.GetArea(known);

                if (area1 > 0 && area2 > 0) return area1 + area2;
            }

            if (BottomRightDiagonalIsValid())
            {
                double area1 = triPairBLTR.Key.GetArea(known);
                double area2 = triPairBLTR.Value.GetArea(known);

                if (area1 > 0 && area2 > 0) return area1 + area2;
            }

            return -1;
        }

        public override bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            return SplitTriangleArea(known) > 0;
        }

        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            return SplitTriangleArea(known);
        }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();
            foreach (Point pt in points) str.Append(pt.CheapPrettyString());
            return "Quad(" + str.ToString() + ")";
        }
    }
}
