using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaSubtraction : AreaArithmeticOperation
    {
        public AreaSubtraction(Region l, Region r) : base(l, r) { }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " - " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            AreaSubtraction ase = obj as AreaSubtraction;
            if (ase == null) return false;

            return base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}