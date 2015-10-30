using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Rectangle : Parallelogram
    {
        public Rectangle(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom,
                            quad.TopLeftDiagonalIsValid(), quad.BottomRightDiagonalIsValid(), quad.diagonalIntersection) { }

        public Rectangle(Segment a, Segment b, Segment c, Segment d, 
            bool tlDiag = false, bool brDiag = false, Intersection inter = null) : base(a, b, c, d)
        {
            foreach (Angle angle in angles)
            {
                if (!Utilities.CompareValues(angle.measure, 90))
                {
                    throw new ArgumentException("Quadrilateral is not a Rectangle; angle does not measure 90^o: " + angle);
                }
            }

            //Set the diagonal and intersection values
            if (!tlDiag) this.SetTopLeftDiagonalInValid();
            if (!brDiag) this.SetBottomRightDiagonalInValid();
            this.SetIntersection(inter);
        }

        public override bool IsStrongerThan(Polygon that)
        {
            if (that is Trapezoid) return false;
            if (that is Kite) return false;
            if (that is Rectangle) return false;
            if (that is Rhombus) return false;
            if (that is Parallelogram) return true;
            if (that is Quadrilateral) return true;

            return false;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Rectangle thatRect = obj as Rectangle;
            if (thatRect == null) return false;

            //return base.StructurallyEquals(obj);
            return base.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Rectangle(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                  bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();
            foreach (Point pt in points) str.Append(pt.CheapPrettyString());
            return "Rect(" + str.ToString() + ")";
        }

        //
        // Calculate base * height ; defer to splitting triangles from there.
        //
        public double Area(double b, double h) { return b * h; }

        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            double[] sideVals = new double[orderedSides.Count];

            for (int s = 0; s < orderedSides.Count; s++)
            {
                sideVals[s] = known.GetSegmentLength(orderedSides[s]);
            }

            // One pair of adjacent sides is required for the area computation.
            for (int s = 0; s < sideVals.Length; s++)
            {
                double baseVal = sideVals[s];
                double heightVal = sideVals[(s+1) % sideVals.Length];

                if (baseVal > 0 && heightVal > 0) return Area(baseVal, heightVal);
            }

            return SplitTriangleArea(known);
        }
    }
}
