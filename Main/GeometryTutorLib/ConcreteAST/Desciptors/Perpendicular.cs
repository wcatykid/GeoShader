using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Perpendicular : Intersection
    {
        public Intersection originalInter { get; private set; }

        public Perpendicular(Intersection inter) : base(inter.intersect, inter.lhs, inter.rhs)
        {
            // Check if truly perpendicular
            if (lhs.CoordinatePerpendicular(rhs) == null)
            {
                throw new ArgumentException("Intersection is not perpendicular: " + inter.ToString());
            }

            originalInter = inter;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            Perpendicular perp = obj as Perpendicular;
            if (perp == null) return false;
            return intersect.Equals(perp.intersect) && ((lhs.StructurallyEquals(perp.lhs) && rhs.StructurallyEquals(perp.rhs)) ||
                                                        (lhs.StructurallyEquals(perp.rhs) && rhs.StructurallyEquals(perp.lhs)));
        }

        public override bool Equals(Object obj)
        {
            if (obj is PerpendicularBisector) return (obj as PerpendicularBisector).Equals(this);

            Perpendicular p = obj as Perpendicular;
            if (p == null) return false;

            return intersect.Equals(p.intersect) && lhs.Equals(p.lhs) && rhs.Equals(p.rhs);
        }

        public override string ToString()
        {
            return "Perpendicular(" + intersect.ToString() + ", " + lhs.ToString() + ", " + rhs.ToString() + ") " + justification;
        }
    }
}
