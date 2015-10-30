using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Altitude : Descriptor
    {
        public Triangle triangle { get; private set; }
        public Segment segment { get; private set; }

        public Altitude(Triangle tri, Segment alt) : base()
        {
            triangle = tri;
            segment = alt;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            Altitude alt = obj as Altitude;
            if (alt == null) return false;
            return triangle.StructurallyEquals(alt.triangle) && segment.StructurallyEquals(alt.segment);
        }

        public override bool Equals(Object obj)
        {
            Altitude alt = obj as Altitude;
            if (alt == null) return false;
            return triangle.Equals(alt.triangle) && segment.Equals(alt.segment) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "Altitude(" + segment.ToString() + ", " + triangle.ToString() + ")";
        }
    }
}
