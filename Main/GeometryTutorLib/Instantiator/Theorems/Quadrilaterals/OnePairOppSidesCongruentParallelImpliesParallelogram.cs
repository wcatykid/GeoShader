using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class OnePairOppSidesCongruentParallelImpliesParallelogram : Theorem
    {
        private readonly static string NAME = "If One Pair of Opposite Sides of a Quadrilateral are Congruent and Parallel, then the quadrilateral is a Parallelogram";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateParallel.Clear();
            candidateQuadrilateral.Clear();
        }

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral || clause is CongruentSegments || clause is Parallel)
            {
                newGrounded.AddRange(InstantiateToRhombus(clause));
            }

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        //
        // Quadrilateral(A, B, C, D), Congruent(Segment(A, B), Segment(C, D)), Congruent(Segment(A, D), Segment(B, C)) -> Parallelogram(A, B, C, D)
        //
        private static List<Quadrilateral> candidateQuadrilateral = new List<Quadrilateral>();
        private static List<Parallel> candidateParallel = new List<Parallel>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        private static List<EdgeAggregator> InstantiateToRhombus(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral)
            {
                Quadrilateral quad = clause as Quadrilateral;

                if (!quad.IsStrictQuadrilateral()) return newGrounded;

                foreach (CongruentSegments oldCs in candidateCongruent)
                {
                    foreach (Parallel parallel in candidateParallel)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, oldCs, parallel));
                    }
                }

                candidateQuadrilateral.Add(quad);
            }
            else if (clause is CongruentSegments)
            {
                CongruentSegments newCs = clause as CongruentSegments;

                if (newCs.IsReflexive()) return newGrounded;

                foreach (Quadrilateral quad in candidateQuadrilateral)
                {
                    foreach (Parallel parallel in candidateParallel)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, newCs, parallel));
                    }
                }

                candidateCongruent.Add(newCs);
            }
            else if (clause is Parallel)
            {
                Parallel newParallel = clause as Parallel;

                foreach (Quadrilateral quad in candidateQuadrilateral)
                {
                    foreach (CongruentSegments oldCs in candidateCongruent)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, oldCs, newParallel));
                    }
                }

                candidateParallel.Add(newParallel);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Quadrilateral quad, CongruentSegments cs, Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Are the pairs on the opposite side of this quadrilateral?
            if (!quad.HasOppositeCongruentSides(cs)) return newGrounded;
            if (!quad.HasOppositeParallelSides(parallel)) return newGrounded;

            // Do the congruent segments coincide with these parallel segments?
            if (!parallel.segment1.HasSubSegment(cs.cs1) && !parallel.segment2.HasSubSegment(cs.cs1)) return newGrounded;
            if (!parallel.segment1.HasSubSegment(cs.cs2) && !parallel.segment2.HasSubSegment(cs.cs2)) return newGrounded;

            //
            // Create the new Rhombus object
            //
            Strengthened newParallelogram = new Strengthened(quad, new Parallelogram(quad));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(cs);
            antecedent.Add(parallel);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallelogram, annotation));

            return newGrounded;
        }
    }
}