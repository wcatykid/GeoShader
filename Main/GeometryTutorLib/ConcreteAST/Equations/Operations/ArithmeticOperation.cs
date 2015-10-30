using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class ArithmeticOperation : ArithmeticNode
    {
        protected GroundedClause leftExp;
        protected GroundedClause rightExp;

        public ArithmeticOperation() : base() { }

        public ArithmeticOperation(GroundedClause l, GroundedClause r) : base()
        {
            leftExp = l;
            rightExp = r;
        }

        public override List<GroundedClause> CollectTerms()
        {
            List<GroundedClause> list = new List<GroundedClause>();

            list.AddRange(leftExp.CollectTerms());

            foreach (GroundedClause gc in rightExp.CollectTerms())
            {
                GroundedClause copyGC = gc.DeepCopy();

                list.Add(copyGC);
            }

            return list;
        }

        public override bool ContainsClause(GroundedClause newG)
        {
            return leftExp.ContainsClause(newG) || rightExp.ContainsClause(newG);
        }

        public override void Substitute(GroundedClause toFind, GroundedClause toSub)
        {
            if (leftExp.Equals(toFind))
            {
                leftExp = toSub;
            }
            else
            {
                leftExp.Substitute(toFind, toSub);
            }

            if (rightExp.Equals(toFind))
            {
                rightExp = toSub;
            }
            else
            {
                rightExp.Substitute(toFind, toSub);
            }
        }

        // Make a deep copy of this object
        public override GroundedClause DeepCopy()
        {
            ArithmeticOperation other = (ArithmeticOperation)(this.MemberwiseClone());
            other.leftExp = leftExp.DeepCopy();
            other.rightExp = rightExp.DeepCopy();

            return other;
        }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " + " + rightExp.ToString() + ")";
        }

        public override bool Equals(Object obj)
        {
            ArithmeticOperation ao = obj as ArithmeticOperation;
            if (ao == null) return false;
            return leftExp.Equals(ao.leftExp) && rightExp.Equals(ao.rightExp) ||
                   leftExp.Equals(ao.rightExp) && rightExp.Equals(ao.leftExp) && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }
    }
}