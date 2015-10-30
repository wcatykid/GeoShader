using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Square : Rhombus
    {
        public Square(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom,
            quad.TopLeftDiagonalIsValid(), quad.BottomRightDiagonalIsValid(), quad.diagonalIntersection) { }

        public Square(Segment left, Segment right, Segment top, Segment bottom, 
            bool tlDiag = false, bool brDiag = false, Intersection inter = null) : base(left, right, top, bottom)
        {
            if (!Utilities.CompareValues(topLeftAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + topLeftAngle);
            }
            if (!Utilities.CompareValues(topRightAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + topRightAngle);
            }

            if (!Utilities.CompareValues(bottomLeftAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + bottomLeftAngle);
            }

            if (!Utilities.CompareValues(bottomRightAngle.measure, 90))
            {
                throw new ArgumentException("Quadrilateral is not a Square; angle does not measure 90^o: " + bottomRightAngle);
            }

            //Set the diagonal and intersection values
            if (!tlDiag) this.SetTopLeftDiagonalInValid();
            if (!brDiag) this.SetBottomRightDiagonalInValid();
            this.SetIntersection(inter);
        }

        //
        // Area-Related Computations
        //
        // Side-squared
        protected double Area(double s)
        {
            return s * s;
        }
        protected double RationalArea(double s) { return Area(s); }
        public override bool IsComputableArea() { return true; }

        private double ClassicArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            foreach (Segment side in orderedSides)
            {
                double sideLength = known.GetSegmentLength(side);

                if (sideLength > 0) return Area(sideLength);
            }

            return -1;
        }

        public override bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Check side-squared.
            if (ClassicArea(known) > 0) return true;

            // If not side-squared, check the general quadrilateral split into triangles.
            return base.CanAreaBeComputed(known);
        }

        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Check side-squared.
            double area = ClassicArea(known);
            
            if (area > 0) return area;

            // If not side-squared, check the general quadrilateral split into triangles.
            return base.GetArea(known);
        }

        public override bool IsStrongerThan(Polygon that)
        {
            if (that is Trapezoid) return false;
            if (that is Kite) return false;
            if (that is Rectangle) return true;
            if (that is Rhombus) return true;
            if (that is Parallelogram) return true;
            if (that is Quadrilateral) return true;

            return false;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Square thatSquare = obj as Square;
            if (thatSquare == null) return false;

            //return base.StructurallyEquals(obj);
            return this.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Square(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                               bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();
            foreach (Point pt in points) str.Append(pt.CheapPrettyString());
            return "Sq(" + str.ToString() + ")";
        }
    }
}
