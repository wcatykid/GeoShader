using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class KiteDefinition : Definition
    {
        private readonly static string NAME = "Definition of Kite";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.KITE_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateQuadrilateral.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.KITE_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral || clause is CongruentSegments)
            {
                newGrounded.AddRange(InstantiateToKite(clause));
            }

            else if (clause is Kite || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromKite(clause));
            }

            return newGrounded;
        }

        //       A   
        //      /|\ 
        //   D /_|_\ B
        //     \ | /
        //      \|/
        //       C
        // Kite(A, B, C, D) -> Congruent(Segment(A, B), Segment(A, D)), Congruent(Segment(C, D), Segment(C, B))
        //
        private static List<EdgeAggregator> InstantiateFromKite(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Kite)
            {
                Kite Kite  = clause as Kite;

                newGrounded.AddRange(InstantiateFromKite(Kite, Kite));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Kite)) return newGrounded;

                newGrounded.AddRange(InstantiateFromKite(streng.strengthened as Kite, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromKite(Kite kite, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine the CongruentSegments opposing sides and output that.
            //
            GeometricCongruentSegments gcs1 = new GeometricCongruentSegments(kite.pairASegment1, kite.pairASegment2);
            GeometricCongruentSegments gcs2 = new GeometricCongruentSegments(kite.pairBSegment1, kite.pairBSegment2);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gcs1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, gcs2, annotation));

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        //
        // Quadrilateral(A, B, C, D), Congruent(Segment(A, B), Segment(C, D)), Congruent(Segment(A, D), Segment(B, C)) -> Kite(A, B, C, D)
        //
        private static List<Quadrilateral> candidateQuadrilateral = new List<Quadrilateral>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        private static List<EdgeAggregator> InstantiateToKite(GroundedClause clause)
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
                        newGrounded.AddRange(InstantiateToKite(quad, candidateCongruent[i], candidateCongruent[j]));
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
                    foreach (CongruentSegments oldCs in candidateCongruent)
                    {
                        newGrounded.AddRange(InstantiateToKite(quad, oldCs, newCs));
                    }
                }

                candidateCongruent.Add(newCs);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToKite(Quadrilateral quad, CongruentSegments cs1, CongruentSegments cs2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The congruences should not share a side.
            if (cs1.SharedSegment(cs2) != null) return newGrounded;

            // The congruent pairs should not also be congruent to each other
            if (cs1.cs1.CoordinateCongruent(cs2.cs1)) return newGrounded;

            // Does both set of congruent segments apply to the quadrilateral?
            if (!quad.HasAdjacentCongruentSides(cs1)) return newGrounded;
            if (!quad.HasAdjacentCongruentSides(cs2)) return newGrounded;

            //
            // Create the new Kite object
            //
            Strengthened newKite = new Strengthened(quad, new Kite(quad));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(cs1);
            antecedent.Add(cs2);

            newGrounded.Add(new EdgeAggregator(antecedent, newKite, annotation));

            return newGrounded;
        }
    }
}