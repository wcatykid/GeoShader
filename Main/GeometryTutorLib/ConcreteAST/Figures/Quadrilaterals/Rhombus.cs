using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Rhombus : Parallelogram
    {
        public Rhombus(Quadrilateral quad)
            : this(quad.left, quad.right, quad.top, quad.bottom,
                quad.TopLeftDiagonalIsValid(), quad.BottomRightDiagonalIsValid(), quad.diagonalIntersection)
        {
        }

        //TODO: Need to find a way to determine the validity of diagonals, and to find the intersection if both diagonals are valid
        //These values are determined for base quadrilaterals by the Implied Component Calculator in the UI parser, but are never
        //computed for the specialized quads

        public Rhombus(Segment left, Segment right, Segment top, Segment bottom,
            bool tlDiag = false, bool brDiag = false, Intersection inter = null)
            : base(left, right, top, bottom)
        {
            if (!Utilities.CompareValues(top.Length, left.Length))
            {
                throw new ArgumentException("Quadrilateral is not a Rhombus; sides are not equal length: " + top + " " + left);
            }
            if (!Utilities.CompareValues(top.Length, right.Length))
            {
                throw new ArgumentException("Quadrilateral is not a Rhombus; sides are not equal length: " + top + " " + right);
            }
            if (!Utilities.CompareValues(top.Length, bottom.Length))
            {
                throw new ArgumentException("Quadrilateral is not a Rhombus; sides are not equal length: " + top + " " + bottom);
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
            Rhombus thatRhom = obj as Rhombus;
            if (thatRhom == null) return false;

            if (thatRhom is Square) return false;

            //return base.StructurallyEquals(obj);
            return base.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            //Rhombus thatRhom = obj as Rhombus;
            //if (thatRhom == null) return false;

            //if (thatRhom is Square) return false;

            //return this.StructurallyEquals(obj);
            return this.StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "Rhombus(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();
            foreach (Point pt in points) str.Append(pt.CheapPrettyString());
            return "Rhombus(" + str.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
