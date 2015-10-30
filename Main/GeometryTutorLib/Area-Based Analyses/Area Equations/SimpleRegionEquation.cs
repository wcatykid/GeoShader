using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{

    public enum OperationT { ADDITION, SUBTRACTION };

    //
    // An equation of the form: target = bigger +- smaller
    //
    public class SimpleRegionEquation
    {
        public Region target { get; private set; }
        public Region bigger { get; private set; }
        public Region smaller { get; private set; }
        public OperationT op { get; private set; }

        public SimpleRegionEquation(Region t,  Region big, OperationT opType, Region small) : base()
        {
            target = t;
            bigger = big;
            op = opType;
            smaller = small;
        }

        // Copy constructor
        public SimpleRegionEquation(SimpleRegionEquation simple) : this(simple.target, simple.bigger, simple.op, simple.smaller) {}

        public override int GetHashCode() { return base.GetHashCode(); }

        //
        // Equals checks that for both sides of this equation is the same as one entire side of the other equation
        //
        public override bool Equals(object obj)
        {
            SimpleRegionEquation thatEquation = obj as SimpleRegionEquation;
            if (thatEquation == null) return false;

            return this.op == thatEquation.op &&
                target.Equals(thatEquation.target) &&
                bigger.Equals(thatEquation.bigger) &&
                smaller.Equals(thatEquation.smaller);
        }
    }
}