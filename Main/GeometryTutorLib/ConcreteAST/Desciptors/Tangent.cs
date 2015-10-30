using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Tangent : Descriptor
    {
        public CircleIntersection intersection { get; protected set; }

        public Tangent(CircleIntersection that) : base()
        {
            if (!that.IsTangent())
            {
                throw new ArgumentException(that + " deduced tangent; it is NOT numerically.");
            }

            intersection = that;
        }

        public override bool StructurallyEquals(Object obj)
        {
            Tangent tangent = obj as Tangent;
            if (tangent == null) return false;
            return this.intersection.StructurallyEquals(tangent.intersection);
        }

        public override bool Equals(Object obj)
        {
            Tangent tangent = obj as Tangent;
            if (tangent == null) return false;
            return this.intersection.Equals(tangent.intersection);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "Tangent(" + intersection.ToString() + ") " + justification;
        }
    }
}