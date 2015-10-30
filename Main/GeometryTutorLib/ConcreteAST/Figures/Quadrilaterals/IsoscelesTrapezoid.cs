using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class IsoscelesTrapezoid : Trapezoid
    {
        public IsoscelesTrapezoid(Quadrilateral quad) : this(quad.left, quad.right, quad.top, quad.bottom) { }

        public IsoscelesTrapezoid(Segment left, Segment right, Segment top, Segment bottom) : base(left, right, top, bottom)
        {
            if (!Utilities.CompareValues(leftLeg.Length, rightLeg.Length))
            {
                throw new ArgumentException("Trapezoid does not define an isosceles trapezoid; sides are not equal length: " + this);
            }
        }

        public override bool IsStrongerThan(Polygon that)
        {
            if (that is Kite) return false;
            if (that is Parallelogram) return false;
            if (that is Trapezoid) return true;
            if (that is Quadrilateral) return true;

            return false;
        }

        public override bool StructurallyEquals(Object obj)
        {
            IsoscelesTrapezoid thatTrap = obj as IsoscelesTrapezoid;
            if (thatTrap == null) return false;

            //return base.StructurallyEquals(obj);
            return this.HasSamePoints(obj as Quadrilateral);
        }

        public override bool Equals(Object obj)
        {
            return StructurallyEquals(obj);
        }

        public override string ToString()
        {
            return "IsoscelesTrapezoid(" + topLeft.ToString() + ", " + topRight.ToString() + ", " +
                                           bottomRight.ToString() + ", " + bottomLeft.ToString() + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();
            foreach (Point pt in points) str.Append(pt.CheapPrettyString());
            return "IsoTrap(" + str.ToString() + ")";
        }

        //
        // Attempt trapezoidal formulas; if they fail, call the base method: splitting into triangles.
        //
        public override double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            if (calculatedHeight > 0)
            {
                double area = GetBaseBasedArea(calculatedHeight, known);
            }

            return base.GetArea(known);
        }
    }
}
