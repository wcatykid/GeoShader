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
    public class CongruentSegments : Congruent
    {
        public Segment cs1 { get; private set; }
        public Segment cs2 { get; private set; }

        public CongruentSegments(Segment s1, Segment s2) : base()
        {
            if (!Utilities.CompareValues(s1.Length, s2.Length))
            {
                throw new ArgumentException("Two congruent Segments deduced congruent although segment lengths differ: " + s1 + " " + s2);
            }

            cs1 = s1;
            cs2 = s2;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public Boolean HasSegment(Segment cs)
        {
            return cs1.Equals(cs) || cs2.Equals(cs);
        }

        // Return the number of shared segments in both congruences
        public override int SharesNumClauses(Congruent thatCC)
        {
            CongruentSegments ccss = thatCC as CongruentSegments;

            if (ccss == null) return 0;

            int numShared = cs1.Equals(ccss.cs1) || cs1.Equals(ccss.cs2) ? 1 : 0;
            numShared += cs2.Equals(ccss.cs1) || cs2.Equals(ccss.cs2) ? 1 : 0;

            return numShared;
        }

        // Return the shared segment in both congruences
        public Segment SegmentShared(CongruentSegments thatCC)
        {
            if (SharesNumClauses(thatCC) != 1) return null;

            return cs1.Equals(thatCC.cs1) || cs1.Equals(thatCC.cs2) ? cs1 : cs2;
        }

        // Given one of the segments in the pair, return the other
        public Segment OtherSegment(Segment cs)
        {
            if (cs.Equals(cs1)) return cs2;
            if (cs.Equals(cs2)) return cs1;
            return null;
        }

        public Segment GetFirstSegment() { return cs1; }
        public Segment GetSecondSegment() { return cs2; }

        public override bool IsReflexive()
        {
            return cs1.Equals(cs2);
        }

        public bool LinksTriangles(Triangle ct1, Triangle ct2)
        {
            return ct1.HasSegment(cs1) && ct2.HasSegment(cs2) || ct1.HasSegment(cs2) && ct2.HasSegment(cs1);
        }

        public bool SharedVertex(CongruentSegments ccs)
        {
            return ccs.cs1.SharedVertex(cs1) != null || ccs.cs1.SharedVertex(cs2) != null ||
                   ccs.cs2.SharedVertex(cs1) != null || ccs.cs2.SharedVertex(cs2) != null;
        }

        public Segment SharedSegment(CongruentSegments ccs)
        {
            if (ccs.cs1.StructurallyEquals(this.cs1) || ccs.cs2.StructurallyEquals(this.cs1)) return this.cs1;
            if (ccs.cs1.StructurallyEquals(this.cs2) || ccs.cs2.StructurallyEquals(this.cs2)) return this.cs2;

            return null;
        }

        public override bool StructurallyEquals(Object obj)
        {
            CongruentSegments ccs = obj as CongruentSegments;

            if (ccs == null) return false;

            return (cs1.StructurallyEquals(ccs.cs1) && cs2.StructurallyEquals(ccs.cs2)) ||
                   (cs1.StructurallyEquals(ccs.cs2) && cs2.StructurallyEquals(ccs.cs1));
        }

        public override bool Equals(Object obj)
        {
            CongruentSegments ccs = obj as CongruentSegments;

            if (ccs == null) return false;

            return (cs1.Equals(ccs.cs1) && cs2.Equals(ccs.cs2)) || (cs1.Equals(ccs.cs2) && cs2.Equals(ccs.cs1)) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "Congruent(" + cs1.ToString() + ", " + cs2.ToString() + ") " + justification;
        }

        public override string ToPrettyString()
        {
            return cs1.ToPrettyString() + " is congruent to " + cs2.ToPrettyString() + ".";
        }
    }
}
