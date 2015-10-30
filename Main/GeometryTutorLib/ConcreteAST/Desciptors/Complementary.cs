using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Complementary : AnglePairRelation
    {
        public Complementary(Angle ang1, Angle ang2) : base(ang1, ang2)
        {
            if (!Utilities.CompareValues(ang1.measure + ang2.measure, 90))
            {
                throw new ArgumentException("Complementary Angles must sum to 90: " + ang1 + " " + ang2);
            }

        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            Complementary supp = obj as Complementary;
            if (supp == null) return false;
            return base.StructurallyEquals(supp);
        }

        public override bool Equals(Object obj)
        {
            Complementary supp = obj as Complementary;
            if (supp == null) return false;
            return base.Equals(supp);
        }

        public override string ToString()
        {
            return "Complementary(" + angle1 + ", " + angle2 + "): " + justification;
        }
    }
}
