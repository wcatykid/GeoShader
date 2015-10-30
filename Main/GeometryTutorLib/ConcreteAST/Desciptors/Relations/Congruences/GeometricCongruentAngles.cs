using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    //
    // This class has two roles:
    // 1) Congruent Pair of Angles
    // 2) To avoid redundancy and bloat in the hypergraph, it also mimics a basic geometric equation
    // So \angle ABC \cong \angle DEF also means m\angle ABC = m\angle DEF (as a GEOMETRIC equation)
    //
    public class GeometricCongruentAngles : CongruentAngles
    {
        public GeometricCongruentAngles(Angle a1, Angle a2) : base(a1, a2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "GeometricCongruent(" + ca1.ToString() + ", " + ca2.ToString() + ") " + justification;
        }
    }
}
