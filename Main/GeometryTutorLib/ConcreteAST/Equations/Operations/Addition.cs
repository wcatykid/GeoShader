using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Addition : ArithmeticOperation
    {
        public Addition() : base() { }

        public Addition(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " + " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            Addition aa = obj as Addition;
            if (aa == null) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode() {  return base.GetHashCode(); }
    }
}