using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class BothPairsOppAnglesCongruentImpliesParallelogram : Theorem
    {
        private readonly static string NAME = "If Both Pairs of Opposite Angles of a Quadrilateral are Congruent, then the quadrilateral is a Parallelogram";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateQuadrilateral.Clear();
        }

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral || clause is CongruentAngles)
            {
                newGrounded.AddRange(InstantiateToTheorem(clause));
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
        private static List<CongruentAngles> candidateCongruent = new List<CongruentAngles>();
        private static List<EdgeAggregator> InstantiateToTheorem(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral)
            {
                Quadrilateral quad = clause as Quadrilateral;

                if (!quad.IsStrictQuadrilateral()) return newGrounded;

                for (int i = 0; i < candidateCongruent.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateCongruent.Count; j++)
                    {
                            newGrounded.AddRange(InstantiateToTheorem(quad, candidateCongruent[i], candidateCongruent[j]));
                    }
                }

                candidateQuadrilateral.Add(quad);
            }
            else if (clause is CongruentAngles)
            {
                CongruentAngles newCas = clause as CongruentAngles;

                if (newCas.IsReflexive()) return newGrounded;

                foreach (Quadrilateral quad in candidateQuadrilateral)
                {
                    foreach (CongruentAngles oldCas in candidateCongruent)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, newCas, oldCas));
                    }
                }

                candidateCongruent.Add(newCas);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Quadrilateral quad, CongruentAngles cas1, CongruentAngles cas2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Are the pairs on the opposite side of this quadrilateral?
            if (!quad.HasOppositeCongruentAngles(cas1)) return newGrounded;
            if (!quad.HasOppositeCongruentAngles(cas2)) return newGrounded;

            //
            // Create the new Rhombus object
            //
            Strengthened newParallelogram = new Strengthened(quad, new Parallelogram(quad));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(cas1);
            antecedent.Add(cas2);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallelogram, annotation));

            return newGrounded;
        }
    }
}