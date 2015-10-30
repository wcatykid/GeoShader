using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    //
    // This class has two roles:
    // 1) Congruent Pair of Angles
    // 2) To avoid redundancy and bloat in the hypergraph, it also mimics a basic Algebraic equation
    // So \angle ABC \cong \angle DEF also means m\angle ABC = m\angle DEF (as a Algebraic equation)
    //
    public class AlgebraicCongruentAngles : CongruentAngles
    {
        public AlgebraicCongruentAngles(Angle a1, Angle a2) : base(a1, a2) { }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override string ToString()
        {
            return "AlgebraicCongruent(" + ca1.ToString() + ", " + ca2.ToString() + ") " + justification;
        }
    }
}
