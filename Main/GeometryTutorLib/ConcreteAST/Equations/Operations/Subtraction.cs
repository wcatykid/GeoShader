using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Subtraction : ArithmeticOperation
    {
        public Subtraction() : base() { }

        public Subtraction(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override List<GroundedClause> CollectTerms()
        {
            List<GroundedClause> list = new List<GroundedClause>();

            list.AddRange(leftExp.CollectTerms());

            foreach (GroundedClause gc in rightExp.CollectTerms())
            {
                GroundedClause copyGC = gc.DeepCopy();

                copyGC.multiplier *= -1;
                list.Add(copyGC);
            }

            return list;
        }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " - " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            Subtraction s = obj as Subtraction;
            if (s == null) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}