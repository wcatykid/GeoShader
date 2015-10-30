using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public abstract class AreaArithmeticNode
    {
        public AreaArithmeticNode() : base() { }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}