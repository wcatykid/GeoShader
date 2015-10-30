using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class FlatEquation : Equation
    {
        public List<GroundedClause> lhsExps { get; private set; }
        public List<GroundedClause> rhsExps { get; private set; }

        public FlatEquation() : base() { }

        public FlatEquation(List<GroundedClause> l, List<GroundedClause> r) : base()
        {
            lhsExps = l;
            rhsExps = r;
        }

        public override GroundedClause DeepCopy()
        {
            return new AlgebraicSegmentEquation(this.lhs.DeepCopy(), this.rhs.DeepCopy());
        }

        public override bool Equals(Object obj)
        {
            AlgebraicSegmentEquation eq = obj as AlgebraicSegmentEquation;
            if (eq == null) return false;
            return (lhs.Equals(eq.lhs) && rhs.Equals(eq.rhs)) || (lhs.Equals(eq.rhs) && rhs.Equals(eq.lhs));
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            string retS = "";
            foreach (GroundedClause lc in lhsExps)
            {
                retS += lc.multiplier + " * " + lc.ToString() + " + "; 
            }
            retS = retS.Substring(0, retS.Length - 3) + " = ";
            foreach (GroundedClause rc in rhsExps)
            {
                retS += rc.multiplier + " * " + rc.ToString() + " + ";
            }

            return retS.Substring(0, retS.Length - 3);
        }

    }
}