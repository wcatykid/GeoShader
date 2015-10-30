using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ParallelogramDefinition : Definition
    {
        private readonly static string NAME = "Definition of Parallelogram";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PARALLELOGRAM_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateParallel.Clear();
            candidateQuadrilateral.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.PARALLELOGRAM_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral || clause is Parallel)
            {
                newGrounded.AddRange(InstantiateToParallelogram(clause));
            }

            else if (clause is Parallelogram || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromParallelogram(clause));
            }

            return newGrounded;
        }

        //     A _________________ B
        //      /                /
        //     /                /
        //    /                /
        // D /________________/ C
        //
        // Parallelogram(A, B, C, D) -> Parallel(Segment(A, B), Segment(C, D)), Parallel(Segment(A, D), Segment(B, C))
        //
        private static List<EdgeAggregator> InstantiateFromParallelogram(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallelogram)
            {
                Parallelogram parallelogram  = clause as Parallelogram;

                newGrounded.AddRange(InstantiateFromParallelogram(parallelogram, parallelogram));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Parallelogram)) return newGrounded;

                newGrounded.AddRange(InstantiateFromParallelogram(streng.strengthened as Parallelogram, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromParallelogram(Parallelogram Parallelogram, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine the parallel opposing sides and output that.
            //
            Parallel newParallel1 = new Parallel(Parallelogram.top, Parallelogram.bottom);
            Parallel newParallel2 = new Parallel(Parallelogram.left, Parallelogram.right);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallel1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, newParallel2, annotation));

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        //
        // Quadrilateral(A, B, C, D), Parallel(Segment(A, B), Segment(C, D)) -> Parallelogram(A, B, C, D)
        //
        private static List<Quadrilateral> candidateQuadrilateral = new List<Quadrilateral>();
        private static List<Parallel> candidateParallel = new List<Parallel>();
        private static List<EdgeAggregator> InstantiateToParallelogram(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral)
            {
                Quadrilateral quad = clause as Quadrilateral;

                if (!quad.IsStrictQuadrilateral()) return newGrounded;

                for (int i = 0; i < candidateParallel.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateParallel.Count; j++)
                    {
                        newGrounded.AddRange(InstantiateToParallelogram(quad, candidateParallel[i], candidateParallel[j]));
                    }
                }

                candidateQuadrilateral.Add(quad);
            }
            else if (clause is Parallel)
            {
                Parallel newParallel = clause as Parallel;

                foreach (Quadrilateral quad in candidateQuadrilateral)
                {
                    foreach (Parallel oldParallel in candidateParallel)
                    {
                        newGrounded.AddRange(InstantiateToParallelogram(quad, oldParallel, newParallel));
                    }
                }

                candidateParallel.Add(newParallel);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToParallelogram(Quadrilateral quad, Parallel parallel1, Parallel parallel2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this paralle set apply to this triangle?
            if (!quad.HasOppositeParallelSides(parallel1)) return newGrounded;
            if (!quad.HasOppositeParallelSides(parallel2)) return newGrounded;

            //
            // Create the new Parallelogram object
            //
            Strengthened newParallelogram = new Strengthened(quad, new Parallelogram(quad));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(parallel1);
            antecedent.Add(parallel2);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallelogram, annotation));

            return newGrounded;
        }
    }
}