using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaAddition : AreaArithmeticOperation
    {
        public AreaAddition(Region l, Region r) : base(l, r) { }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " + " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            AreaAddition aa = obj as AreaAddition;
            if (aa == null) return false;

            return base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}