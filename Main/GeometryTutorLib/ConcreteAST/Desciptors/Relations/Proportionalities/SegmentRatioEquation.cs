using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class SegmentRatioEquation : Descriptor
    {
        public SegmentRatio lhs { get; protected set; }
        public SegmentRatio rhs { get; protected set; }
        private List<Segment> segments;
        //public KeyValuePair<int, int> proportion { get; protected set; }
        //public double dictatedProportion { get; protected set; }

        public SegmentRatioEquation(SegmentRatio ell, SegmentRatio r) : base()
        {
            if (!ell.ProportionallyEquals(r))
            {
                throw new ArgumentException("Ratios of segments are not proportionally equal: " + ell + " " + r);
            }

            lhs = ell;
            rhs = r;

            segments = new List<Segment>();
            segments.Add(lhs.smallerSegment);
            segments.Add(lhs.largerSegment);
            segments.Add(rhs.smallerSegment);
            segments.Add(rhs.largerSegment);
        }

        //// Return the number of shared segments in both congruences
        //public int SharesNumClauses(CongruentSegments thatCS)
        //{
        //    //CongruentSegments css = thatPS as CongruentSegments;

        //    //if (css == null) return 0;

        //    int numShared = smallerSegment.Equals(thatCS.cs1) || smallerSegment.Equals(thatCS.cs2) ? 1 : 0;
        //    numShared += largerSegment.Equals(thatCS.cs1) || largerSegment.Equals(thatCS.cs2) ? 1 : 0;

        //    return numShared;
        //}

        //
        // This equation must be able to relate two segments from triangle 1 and two from triangle 2
        //
        public bool LinksTriangles(Triangle ct1, Triangle ct2)
        {
            int count1 = 0;
            int count2 = 0;
            bool[] marked = new bool[segments.Count];
            for (int s = 0; s < segments.Count; s++)
            {
                if (ct1.HasSegment(segments[s]))
                {
                    marked[s] = true;
                    count1++;
                }
                if (ct2.HasSegment(segments[s]))
                {
                    marked[s] = true;
                    count2++;
                }
            }

            if (marked.Contains(false)) return false;

            return count1 == 2 && count2 == 2;
        }

        public KeyValuePair<Segment, Segment> GetSegments(Triangle tri)
        {
            // Collect the applicable segments.
            List<Segment> theseSegments = new List<Segment>();
            foreach (Segment segment in segments)
            {
                if (tri.HasSegment(segment)) theseSegments.Add(segment);
            }

            // Check for error condition
            if (theseSegments.Count != 2) return new KeyValuePair<Segment, Segment>(null, null);

            // Place the larger segment first
            KeyValuePair<Segment, Segment> pair;
            if (theseSegments[0].Length > theseSegments[1].Length)
            {
                pair = new KeyValuePair<Segment, Segment>(theseSegments[0], theseSegments[1]);
            }
            else
            {
                pair = new KeyValuePair<Segment, Segment>(theseSegments[1], theseSegments[0]);
            }

            return pair;
        }

        //
        //  if x / y = z / w   and we are checking for  x / z OR y / w
        //
        private bool HasImpliedRatio(SegmentRatio thatRatio)
        {
            if (thatRatio.HasSegment(lhs.largerSegment) && thatRatio.HasSegment(rhs.largerSegment)) return true;
            if (thatRatio.HasSegment(lhs.smallerSegment) && thatRatio.HasSegment(rhs.smallerSegment)) return true;

            return false;
        }

        private SegmentRatio GetOtherImpliedRatio(SegmentRatio thatRatio)
        {
            if (thatRatio.HasSegment(lhs.largerSegment) && thatRatio.HasSegment(rhs.largerSegment))
            {
                return new SegmentRatio(lhs.smallerSegment, rhs.smallerSegment);
            }

            if (thatRatio.HasSegment(lhs.smallerSegment) && thatRatio.HasSegment(rhs.smallerSegment))
            {
                return new SegmentRatio(lhs.largerSegment, rhs.largerSegment);
            }

            return null;
        }

        public bool SharesRatio(SegmentRatioEquation that)
        {
            return GetSharedRatio(that) != null;
        }

        public SegmentRatio GetSharedRatio(SegmentRatioEquation that)
        {
            // Check for the obvious shared ratio
            if (lhs.StructurallyEquals(that.lhs)) return that.lhs;
            if (lhs.StructurallyEquals(that.rhs)) return that.rhs;
            if (rhs.StructurallyEquals(that.lhs)) return that.lhs;
            if (rhs.StructurallyEquals(that.rhs)) return that.rhs;

            if (HasImpliedRatio(that.lhs)) return that.lhs;
            if (HasImpliedRatio(that.rhs)) return that.rhs;

            return null;
        }

        public SegmentRatio GetOtherRatio(SegmentRatio that)
        {
            // Check for the obvious shared ratio
            if (lhs.StructurallyEquals(that)) return this.rhs;
            if (rhs.StructurallyEquals(that)) return this.lhs;

            return GetOtherImpliedRatio(that);
        }

        public override bool StructurallyEquals(Object obj)
        {
            SegmentRatioEquation that = obj as SegmentRatioEquation;
            if (that == null) return false;

            return lhs.StructurallyEquals(that.lhs) && rhs.StructurallyEquals(that.rhs) ||
                   lhs.StructurallyEquals(that.rhs) && rhs.StructurallyEquals(that.lhs);
        }

        public override bool Equals(Object obj)
        {
            SegmentRatioEquation that = obj as SegmentRatioEquation;
            if (that == null) return false;

            return lhs.Equals(that.lhs) && rhs.Equals(that.rhs) ||
                   lhs.Equals(that.rhs) && rhs.Equals(that.lhs);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            return "ProportionalEquation(" + lhs.ToString() + " = " + rhs.ToString() + ") ";
        }
    }
}
