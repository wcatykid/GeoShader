using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class NumericValue : ArithmeticNode
    {
        protected double value;

        public NumericValue() : base() { value = 0; }
        public NumericValue(int v) : base() { value = v; }
        public NumericValue(double v) : base() { value = v; }

        public override string ToString() { return value.ToString(); }

        public override bool ContainsClause(GroundedClause clause) 
        {
            NumericValue clauseValue = clause as NumericValue;
            if (clauseValue == null) return false;

            return Utilities.CompareValues(value, clauseValue.value);
        }

        // Make a deep copy of this object; this is really a shallow copy since it
        // is only an integer wrapper
        public override GroundedClause DeepCopy()
        {
            return (NumericValue)this.MemberwiseClone();
        }

        public int IntValue
        {
            get
            {
                return (int)value;
            }
        }
        public double DoubleValue
        {
            get
            {
                return value;
            }
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(object obj)
        {
            NumericValue thatVal = obj as NumericValue;
            if (thatVal == null) return false;

            return Utilities.CompareValues(value, thatVal.value);
        }
    }
}