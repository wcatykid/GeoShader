using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Point  : Figure
    {

        /// <summary>
        /// Returns the midpoint between this point and that point.
        /// </summary>
        /// <param name="that">Some point.</param>
        /// <returns>The midpoint of the two points.</returns>
        public Point Midpoint(Point that)
        {
            return new Point("", (this.X + that.X) / 2.0, (this.Y + that.Y) / 2.0);
        }

        public bool Collinear(Point pt1, Point pt2)
        {
            return Utilities.CompareValues(this.Slope(pt1), this.Slope(pt2));
        }

        public double Slope(Point pt)
        {
            return (pt.Y - this.Y) / (pt.X - this.X);
        }
    }
}
