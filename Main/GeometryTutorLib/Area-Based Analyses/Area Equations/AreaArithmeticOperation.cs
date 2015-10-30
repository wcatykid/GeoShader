using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    public class AreaArithmeticOperation : AreaArithmeticNode
    {
        protected Region leftExp;
        protected Region rightExp;

        public AreaArithmeticOperation() : base() { }

        public AreaArithmeticOperation(Region l, Region r) : base()
        {
            leftExp = l;
            rightExp = r;
        }

        public override bool Equals(Object obj)
        {
            AreaArithmeticOperation aao = obj as AreaArithmeticOperation;
            if (aao == null) return false;

            return leftExp.Equals(aao.leftExp) && rightExp.Equals(aao.rightExp) ||
                   leftExp.Equals(aao.rightExp) && rightExp.Equals(aao.leftExp) && base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}