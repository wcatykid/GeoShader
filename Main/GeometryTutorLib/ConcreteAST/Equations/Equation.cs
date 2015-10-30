using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Equation : ArithmeticNode
    {
        public GroundedClause lhs { get; private set; }
        public GroundedClause rhs { get; private set; }

        public Equation() : base() { }

        public Equation(GroundedClause l, GroundedClause r) : base()
        {
            lhs = l;
            rhs = r;
        }

        //public Equation(GroundedClause l, GroundedClause r, string just) : base()
        //{
        //    lhs = l;
        //    rhs = r;
        //    justification = just;
        //}

        public override void Substitute(GroundedClause toFind, GroundedClause toSub)
        {
            if (lhs.Equals(toFind))
            {
                lhs = toSub.DeepCopy();
            }
            else
            {
                lhs.Substitute(toFind, toSub);
            }

            if (rhs.Equals(toFind))
            {
                rhs = toSub.DeepCopy();
            }
            else
            {
                rhs.Substitute(toFind, toSub);
            }
        }

        public override bool ContainsClause(GroundedClause target)
        {
            // If a composite node, check accordingly; this will return false if they are atomic
            return lhs.ContainsClause(target) || rhs.ContainsClause(target);
        }

        //
        // Determines if the equation has one side being atomic; no compound expressions
        // returns -1 (left is atomic), 0 (neither atomic), 1 (right is atomic)
        // both atomic: 2
        //
        public const int LEFT_ATOMIC = -1;
        public const int NONE_ATOMIC = 0;
        public const int RIGHT_ATOMIC = 1;
        public const int BOTH_ATOMIC = 2;
        public int GetAtomicity()
        {
            bool leftIs = lhs is Angle || lhs is Segment || lhs is Arc || lhs is NumericValue;
            bool rightIs = rhs is Angle || rhs is Segment || rhs is Arc || rhs is NumericValue;

            if (leftIs && rightIs) return BOTH_ATOMIC;
            if (!leftIs && !rightIs) return NONE_ATOMIC;
            if (leftIs) return LEFT_ATOMIC;
            if (rightIs) return RIGHT_ATOMIC;

            return NONE_ATOMIC;
        }

        // Collect all the terms and return a count for both sides <left, right>
        public KeyValuePair<int, int> GetCardinalities()
        {
            List<GroundedClause> left = lhs.CollectTerms();
            List<GroundedClause> right = rhs.CollectTerms();
            return new KeyValuePair<int, int>(left.Count, right.Count);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override bool StructurallyEquals(object obj)
        {
            Equation thatEquation = obj as Equation;
            if (thatEquation == null) return false;

            return Equals(obj);
        }

        //
        // Equals checks that for both sides of this equation is the same as one entire side of the other equation
        //
        public override bool Equals(object target)
        {
            Equation thatEquation = target as Equation;
            if (thatEquation == null) return false;

            //
            // Collect all basic terms on the left and right hand sides of both equations.
            //
            List<GroundedClause> thisLHS = lhs.CollectTerms();
            List<GroundedClause> thisRHS = rhs.CollectTerms();

            List<GroundedClause> thatLHS = thatEquation.lhs.CollectTerms();
            List<GroundedClause> thatRHS = thatEquation.rhs.CollectTerms();

            // Check side length counts as a first step.
            if (!(thisLHS.Count == thatLHS.Count && thisRHS.Count == thatRHS.Count) && !(thisLHS.Count == thatRHS.Count && thisRHS.Count == thatLHS.Count)) return false;

            // Seek one side equal to one side and then the other equals the other.
            // Cannot do this easily with a union / set interection set theoretic approach since an equation may have multiple instances of a value
            // In theory, since we always deal with simplified equations, there should not be multiple instances of a particular value.
            // So, this should work.

            // Note operations like multiplication and substraction have been taken into account.
            List<GroundedClause> unionLHS = new List<GroundedClause>(thisLHS);
            Utilities.AddUniqueList(unionLHS, thatLHS);

            List<GroundedClause> unionRHS = new List<GroundedClause>(thisRHS);
            Utilities.AddUniqueList(unionRHS, thatRHS);

            // Exact same sides means the union is the same as each list itself
            if (unionLHS.Count == thisLHS.Count && unionRHS.Count == thisRHS.Count) return true;

            // Check the other combination of sides
            unionLHS = new List<GroundedClause>(thisLHS);
            Utilities.AddUniqueList(unionLHS, thatRHS);

            if (unionLHS.Count != thisLHS.Count) return false;

            unionRHS = new List<GroundedClause>(thisRHS);
            Utilities.AddUniqueList(unionRHS, thatLHS);

            // Exact same sides means the union is the same as each list itself
            return unionRHS.Count == thisRHS.Count;
        }

        //
        // Returns if this equation represents a congruence relationship between two segments or angles
        //
        // Note: we need something of the form: 1 * m\angle ABC = 1* m\angle DEF  for congruence
        public bool IsProperCongruence()
        {
            return !(lhs is NumericValue) && !(rhs is NumericValue) && lhs.multiplier == 1 && rhs.multiplier == 1;
        }

        //private static readonly string SEGMENT_NAME = "Congruent Segments Imply Equal Lengths";
        //private static readonly string ANGLE_NAME = "Congruent Angles Imply Equal Measure";

        //
        // Congruent(Segment(A, B), Segment(C, D)) -> AB = CD
        // Congruent(Angle(A, B, C), Angle(D, E, F)) -> m\angle ABC = m\angle DEF
        //
        //public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        //{
        //    List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

        //    if (!(clause is GeometricCongruentSegments || clause is GeometricCongruentAngles)) return newGrounded;

        //    // For hyperedge construction
        //    List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(clause);

        //    // Congruent(Segment(A, B), Segment(C, D)) -> AB = CD
        //    if (clause is GeometricCongruentAngles)
        //    {
        //        GeometricCongruentAngles ccas = clause as GeometricCongruentAngles;

        //        AlgebraicAngleEquation angEq = new AlgebraicAngleEquation(ccas.ca1.DeepCopy(), ccas.ca2.DeepCopy(), ANGLE_NAME);

        //        newGrounded.Add(new EdgeAggregator(antecedent, angEq));
        //    }
        //    // Congruent(Angle(A, B, C), Angle(D, E, F)) -> m\angle ABC = m\angle DEF
        //    else if (clause is GeometricCongruentSegments)
        //    {
        //        GeometricCongruentSegments ccss = clause as GeometricCongruentSegments;

        //        AlgebraicSegmentEquation segEq = new AlgebraicSegmentEquation(ccss.cs1.DeepCopy(), ccss.cs2.DeepCopy(), SEGMENT_NAME);

        //        newGrounded.Add(new EdgeAggregator(antecedent, segEq));
        //    }

        //    return newGrounded;
        //}
    }
}