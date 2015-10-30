using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    //
    // This class has two roles:
    // 1) Congruent Pair of segments
    // 2) To avoid redundancy and bloat in the hypergraph, it also mimics a basic Algebraic equation
    // So AB \cong CD also means AB = CD (as a Algebraic equation)
    //
    public class AlgebraicCongruentSegments : CongruentSegments
    {
        public AlgebraicCongruentSegments(Segment s1, Segment s2) : base(s1, s2) { }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override string ToString()
        {
            return "AlgebraicCongruent(" + cs1.ToString() + ", " + cs2.ToString() + ") " + justification;
        }
    }
}
