using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    //
    // This class has two roles:
    // 1) Congruent Pair of segments
    // 2) To avoid redundancy and bloat in the hypergraph, it also mimics a basic geometric equation
    // So AB \cong CD also means AB = CD (as a GEOMETRIC equation)
    //
    public class GeometricCongruentSegments : CongruentSegments
    {
        public GeometricCongruentSegments(Segment s1, Segment s2) : base(s1, s2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "GeometricCongruent(" + cs1.ToString() + ", " + cs2.ToString() + ") " + justification;
        }
    }
}
