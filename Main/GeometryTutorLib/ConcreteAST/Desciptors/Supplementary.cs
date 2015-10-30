using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Supplementary : AnglePairRelation
    {
        public Supplementary(Angle ang1, Angle ang2) : base(ang1, ang2)
        {
            if (!Utilities.CompareValues(ang1.measure + ang2.measure, 180))
            {
                throw new ArgumentException("Supplementary Angles must sum to 180: " + ang1 + " " + ang2);
            }
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(Object obj)
        {
            Supplementary supp = obj as Supplementary;
            if (supp == null) return false;
            return base.StructurallyEquals(supp);
        }

        public override bool Equals(Object obj)
        {
            Supplementary supp = obj as Supplementary;
            if (supp == null) return false;
            return base.Equals(supp);
        }

        public override string ToString()
        {
            return "Supplementary(" + angle1 + ", " + angle2 + "): " + justification;
        }
    }
}
