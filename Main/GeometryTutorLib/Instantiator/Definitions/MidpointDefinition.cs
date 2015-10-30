using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MidpointDefinition : Definition
    {
        private readonly static string NAME = "Definition of Midpoint";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.MIDPOINT_DEFINITION);

        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateSegments.Clear();
            candidateInMiddle.Clear();
            candidateMidpoint.Clear();
            candidateStrengthened.Clear();
        }

        private static List<Segment> candidateSegments = new List<Segment>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        private static List<InMiddle> candidateInMiddle = new List<InMiddle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<Midpoint> candidateMidpoint = new List<Midpoint>();

        //
        // This implements forward and Backward instantiation
        // Forward is Midpoint -> Congruent Clause
        // Backward is Congruent -> Midpoint Clause
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.MIDPOINT_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Midpoint || clause is Strengthened || clause is InMiddle)
            {
                newGrounded.AddRange(InstantiateFromMidpoint(clause));
            }

            if (clause is CongruentSegments || (clause is InMiddle && !(clause is Midpoint)))
            {
                newGrounded.AddRange(InstantiateToMidpoint(clause));
            }

            return newGrounded;
        }

        //
        // Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
        // Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B)); This implies: AM = MB
        //
        private static List<EdgeAggregator> InstantiateFromMidpoint(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is InMiddle && !(clause is Midpoint))
            {
                InMiddle inMid = clause as InMiddle;

                foreach (Midpoint midpt in candidateMidpoint)
                {
                    newGrounded.AddRange(InstantiateFromMidpoint(inMid, midpt, midpt));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateFromMidpoint(inMid, streng.strengthened as Midpoint, streng));
                }

                candidateInMiddle.Add(inMid);
            }
            else if (clause is Midpoint)
            {
                Midpoint midpt = clause as Midpoint;

                foreach (InMiddle im in candidateInMiddle)
                {
                    newGrounded.AddRange(InstantiateFromMidpoint(im, midpt, midpt));
                }

                candidateMidpoint.Add(midpt);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Midpoint)) return newGrounded;

                foreach (InMiddle im in candidateInMiddle)
                {
                    newGrounded.AddRange(InstantiateFromMidpoint(im, streng.strengthened as Midpoint, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }
        private static List<EdgeAggregator> InstantiateFromMidpoint(InMiddle im, Midpoint midpt, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this ImMiddle apply to this midpoint?
            if (!im.point.StructurallyEquals(midpt.point)) return newGrounded;
            if (!im.segment.StructurallyEquals(midpt.segment)) return newGrounded;

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            // Backward: Midpoint(M, Segment(A, B)) -> InMiddle(A, M, B)
            newGrounded.Add(new EdgeAggregator(antecedent, im, annotation));

            //
            // Forward: Midpoint(M, Segment(A, B)) -> Congruent(Segment(A,M), Segment(M,B))
            //
            Segment left = new Segment(midpt.segment.Point1, midpt.point);
            Segment right = new Segment(midpt.point, midpt.segment.Point2);
            GeometricCongruentSegments ccss = new GeometricCongruentSegments(left, right);
            newGrounded.Add(new EdgeAggregator(antecedent, ccss, annotation));

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToMidpoint(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is InMiddle && !(clause is Midpoint))
            {
                InMiddle inMid = clause as InMiddle;

                foreach (CongruentSegments css in candidateCongruent)
                {
                    newGrounded.AddRange(InstantiateToMidpoint(inMid, css));
                }

                // No need to add this InMiddle object to the list since it was added previously in InstantiateFrom
            }
            else if (clause is CongruentSegments)
            {
                CongruentSegments css = clause as CongruentSegments;

                // A reflexive relationship cannot possibly create a midpoint situation
                if (css.IsReflexive()) return newGrounded;

                // The congruence must relate two collinear segments...
                if (!css.cs1.IsCollinearWith(css.cs2)) return newGrounded;

                // ...that share a vertex
                if (css.cs1.SharedVertex(css.cs2) == null) return newGrounded;

                foreach (InMiddle im in candidateInMiddle)
                {
                    newGrounded.AddRange(InstantiateToMidpoint(im, css));
                }

                candidateCongruent.Add(css);
            }

            return newGrounded;
        }

        //
        // Congruent(Segment(A, M), Segment(M, B)) -> Midpoint(M, Segment(A, B))
        //
        private static List<EdgeAggregator> InstantiateToMidpoint(InMiddle im, CongruentSegments css)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Point midpoint = css.cs1.SharedVertex(css.cs2);

            // Does this InMiddle relate to the congruent segments?
            if (!im.point.StructurallyEquals(midpoint)) return newGrounded;

            // Do the congruent segments combine into a single segment equating to the InMiddle?
            Segment overallSegment = new Segment(css.cs1.OtherPoint(midpoint), css.cs2.OtherPoint(midpoint));
            if (!im.segment.StructurallyEquals(overallSegment)) return newGrounded;

            Strengthened newMidpoint = new Strengthened(im, new Midpoint(im)); 

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(im);
            antecedent.Add(css);

            newGrounded.Add(new EdgeAggregator(antecedent, newMidpoint, annotation));

            return newGrounded;
        }
    }
}