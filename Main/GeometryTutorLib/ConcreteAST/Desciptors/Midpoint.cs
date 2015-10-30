using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents an angle (degrees), defined by 3 points.
    /// </summary>
    public class Midpoint : InMiddle
    {
        public Midpoint(InMiddle im) : base(im.point, im.segment) { }

        public override bool StructurallyEquals(Object obj)
        {
            Midpoint midptObj = obj as Midpoint;
            if (midptObj == null) return false;

            return point.StructurallyEquals(midptObj.point) && segment.StructurallyEquals(midptObj.segment);
        }

        public override bool Equals(Object obj)
        {
            Midpoint midptObj = obj as Midpoint;
            if (midptObj == null) return false;
            return point.Equals(midptObj.point) && segment.Equals(midptObj.segment) && base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "Midpoint(" + point.ToString() + ", " + segment.ToString() + ") " + justification;
        }
    }
}
