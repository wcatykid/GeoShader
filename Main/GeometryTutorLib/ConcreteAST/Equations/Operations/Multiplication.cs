using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Multiplication : ArithmeticOperation
    {
        public Multiplication() : base() { }

        public Multiplication(GroundedClause l, GroundedClause r) : base(l, r) { }

        public override string ToString()
        {
            return "(" + leftExp.ToString() + " * " + rightExp.ToString() + ")";
        }

        //
        // In an attempt to avoid issues, all terms collected are copies of the originals.
        //
        public override List<GroundedClause> CollectTerms()
        {
            List<GroundedClause> list = new List<GroundedClause>();

            if (leftExp is NumericValue && rightExp is NumericValue)
            {
                list.Add(new NumericValue((leftExp as NumericValue).DoubleValue * (rightExp as NumericValue).DoubleValue));
                return list;
            }
            
            if (leftExp is NumericValue)
            {
                foreach (GroundedClause gc in rightExp.CollectTerms())
                {
                    GroundedClause copyGC = gc.DeepCopy();

                    copyGC.multiplier *= ((NumericValue)leftExp).IntValue;
                    list.Add(copyGC);
                }
            }

            if (rightExp is NumericValue)
            {
                foreach (GroundedClause gc in leftExp.CollectTerms())
                {
                    GroundedClause copyGC = gc.DeepCopy();

                    copyGC.multiplier *= ((NumericValue)rightExp).IntValue;
                    list.Add(copyGC);
                }
            }

            return list;
        }

        public override bool Equals(Object obj)
        {
            Multiplication m = obj as Multiplication;
            if (m == null) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}