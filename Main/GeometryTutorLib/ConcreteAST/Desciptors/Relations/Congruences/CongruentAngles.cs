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
    public class CongruentAngles : Congruent
    {
        public Angle ca1 { get; private set; }
        public Angle ca2 { get; private set; }

        public Angle GetFirstAngle() { return ca1; }
        public Angle GetSecondAngle() { return ca2; }

        //
        // Deduced Node Information
        //
        public CongruentAngles(Angle a1, Angle a2) : base()
        {
            if (!Utilities.CompareValues(a1.measure, a2.measure))
            {
                throw new ArgumentException("Two congruent angles deduced congruent although angle measures differ: " + a1 + " " + a2);
            }

            ca1 = a1;
            ca2 = a2;
        }

        public override bool StructurallyEquals(Object obj)
        {
            CongruentAngles cas = obj as CongruentAngles;
            if (cas == null) return false;

            if (!Utilities.CompareValues(this.ca1.measure, cas.ca1.measure)) return false;

            return (ca1.StructurallyEquals(cas.ca1) && ca2.StructurallyEquals(cas.ca2)) ||
                   (ca1.StructurallyEquals(cas.ca2) && ca2.StructurallyEquals(cas.ca1));
        }

        public override bool Equals(Object obj)
        {
            return this.StructurallyEquals(obj);
        }

        public override bool IsReflexive()
        {
            return ca1.Equals(ca2);
        }

        // Return the number of shared angles in both congruences
        public override int SharesNumClauses(Congruent thatCC)
        {
            CongruentAngles ccas = thatCC as CongruentAngles;

            if (ccas == null) return 0;

            int numShared = ca1.Equates(ccas.ca1) || ca1.Equates(ccas.ca2) ? 1 : 0;
            numShared += ca2.Equates(ccas.ca1) || ca2.Equates(ccas.ca2) ? 1 : 0;

            return numShared;
        }

        // Return the shared angle in both congruences
        public Segment SegmentShared()
        {
            if (ca1.ray1.IsCollinearWith(ca2.ray1) || ca1.ray1.IsCollinearWith(ca2.ray2)) return ca1.ray1;
            if (ca1.ray2.IsCollinearWith(ca2.ray1) || ca1.ray2.IsCollinearWith(ca2.ray2)) return ca1.ray2;

            return null;
        }

        // Return the shared angle in both congruences
        public Angle AngleShared(CongruentAngles cas)
        {
            if (ca1.Equates(cas.ca1) || ca1.Equates(cas.ca2)) return ca1;
            if (ca2.Equates(cas.ca1) || ca2.Equates(cas.ca2)) return ca2;

            return null;
        }

        // Return the shared angle in both congruences
        public Segment AreAdjacent()
        {
            return ca1.IsAdjacentTo(ca2);
        }


        // Given one of the angles in the pair, return the other
        public Angle OtherAngle(Angle cs)
        {
            if (cs.Equates(ca1)) return ca2;
            if (cs.Equates(ca2)) return ca1;
            return null;
        }

        // Given one of the angles in the pair, return the other
        public bool HasAngle(Angle cs)
        {
            if (cs.Equates(ca1)) return true;
            if (cs.Equates(ca2)) return true;
            return false;
        }

        public bool LinksTriangles(Triangle ct1, Triangle ct2)
        {
            return ct1.HasAngle(ca1) && ct2.HasAngle(ca2) || ct1.HasAngle(ca2) && ct2.HasAngle(ca1);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "Congruent(" + ca1.ToString() + ", " + ca2.ToString() + ") " + justification;
        }

        public override string ToPrettyString()
        {
            return ca1.ToPrettyString() + " is congruent to " + ca2.ToPrettyString() + " .";
        }
    }
}
