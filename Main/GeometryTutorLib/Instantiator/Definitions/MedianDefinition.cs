using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MedianDefinition : Definition
    {
        private readonly static string NAME = "Definition of Median";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.MEDIAN_DEFINITION);

        private static List<Triangle> candidateTriangle = new List<Triangle>();
        private static List<SegmentBisector> candidateBisector = new List<SegmentBisector>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        private static List<Median> candidateMedian = new List<Median>();
        private static List<InMiddle> candidateInMiddle = new List<InMiddle>();

        // Reset saved data for next problem
        public static void Clear()
        {
            candidateBisector.Clear();
            candidateTriangle.Clear();
            candidateStrengthened.Clear();
            candidateMedian.Clear();
            candidateInMiddle.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.MEDIAN_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Median || clause is InMiddle) newGrounded.AddRange(InstantiateFromMedian(clause));

            if (clause is SegmentBisector || clause is Triangle || clause is Strengthened) newGrounded.AddRange(InstantiateToMedian(clause));

            return newGrounded;
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // Median(Segment(V, C), Triangle(C, A, B)) -> Midpoint(V, Segment(B, A))
        //
        private static List<EdgeAggregator> InstantiateFromMedian(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is InMiddle && !(clause is Midpoint))
            {
                InMiddle im = clause as InMiddle;

                foreach (Median median in candidateMedian)
                {
                    newGrounded.AddRange(InstantiateFromMedian(im, median));
                }

                candidateInMiddle.Add(im);
            }
            else if (clause is Median)
            {
                Median median = clause as Median;

                foreach (InMiddle im in candidateInMiddle)
                {
                    newGrounded.AddRange(InstantiateFromMedian(im, median));
                }

                candidateMedian.Add(median);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromMedian(InMiddle im, Median median)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Which point is on the side of the triangle?
            Point vertexOnTriangle = median.theTriangle.GetVertexOn(median.medianSegment);
            Segment segmentCutByMedian = median.theTriangle.GetOppositeSide(vertexOnTriangle);
            Point midpt = segmentCutByMedian.FindIntersection(median.medianSegment);

            // This is to acquire the name of the midpoint, nothing more.
            if (midpt.Equals(median.medianSegment.Point1)) midpt = median.medianSegment.Point1;
            else if (midpt.Equals(median.medianSegment.Point2)) midpt = median.medianSegment.Point2;

            // Does this median apply to this InMiddle? Point check ...
            if (!im.point.StructurallyEquals(midpt)) return newGrounded;

            // Segment check
            if (!im.segment.StructurallyEquals(segmentCutByMedian)) return newGrounded;

            // Create the midpoint
            Strengthened newMidpoint = new Strengthened(im, new Midpoint(im));

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(median);
            antecedent.Add(im);

            newGrounded.Add(new EdgeAggregator(antecedent, newMidpoint, annotation));

            return newGrounded;
        }

        //     B ---------V---------A
        //                 \
        //                  \
        //                   \
        //                    C
        //
        // SegmentBisector(Segment(V, C), Segment(B, A)), Triangle(A, B, C) -> Median(Segment(V, C), Triangle(A, B, C))
        //        
        private static List<EdgeAggregator> InstantiateToMedian(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Triangle)
            {
                Triangle tri = clause as Triangle;

                foreach (SegmentBisector sb in candidateBisector)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, sb, sb));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, streng.strengthened as SegmentBisector, streng));
                }

                candidateTriangle.Add(tri);
            }
            else if (clause is SegmentBisector)
            {
                SegmentBisector sb = clause as SegmentBisector;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, sb, sb));
                }

                candidateBisector.Add(sb);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is SegmentBisector)) return newGrounded;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToMedian(tri, streng.strengthened as SegmentBisector, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // Take the angle congruence and bisector and create the AngleBisector relation
        //
        private static List<EdgeAggregator> InstantiateToMedian(Triangle tri, SegmentBisector sb, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The Bisector cannot be a side of the triangle.
            if (tri.CoincidesWithASide(sb.bisector) != null) return newGrounded;

            // Acquire the intersection segment that coincides with the base of the triangle
            Segment triangleBaseCandidate = sb.bisected.OtherSegment(sb.bisector);
            Segment triangleBase = tri.CoincidesWithASide(triangleBaseCandidate);
            if (triangleBase == null) return newGrounded;

            // The candidate base and the actual triangle side must equate exactly
            if (!triangleBase.HasSubSegment(triangleBaseCandidate) || !triangleBaseCandidate.HasSubSegment(triangleBase)) return newGrounded;

            // The point opposite the base of the triangle must be within the endpoints of the bisector
            Point oppPoint = tri.OtherPoint(triangleBase);
            if (!sb.bisector.PointLiesOnAndBetweenEndpoints(oppPoint)) return newGrounded;

            // -> Median(Segment(V, C), Triangle(A, B, C))
            Median newMedian = new Median(sb.bisector, tri);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newMedian, annotation));

            return newGrounded;
        }
    }
}