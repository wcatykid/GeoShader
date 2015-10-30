using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class OppositeSidesOfParallelogramAreCongruent : Theorem
    {
        private readonly static string NAME = "Opposite Sides of a Parallelogram are Congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT);

        //     A _________________ B
        //      /                /
        //     /                /
        //    /                /
        // D /________________/ C
        //
        // Parallelogram(A, B, C, D) -> Congruent(Segment(A, B), Segment(C, D)), Congruent(Segment(A, D), Segment(B, C))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallelogram)
            {
                Parallelogram para = clause as Parallelogram;

                newGrounded.AddRange(InstantiateTheorem(para, para));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Parallelogram)) return newGrounded;

                newGrounded.AddRange(InstantiateTheorem(streng.strengthened as Parallelogram, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateTheorem(Parallelogram parallelogram, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Determine the CongruentSegments opposing sides and output that.
            GeometricCongruentSegments gcs1 = new GeometricCongruentSegments(parallelogram.top, parallelogram.bottom);
            GeometricCongruentSegments gcs2 = new GeometricCongruentSegments(parallelogram.left, parallelogram.right);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gcs1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, gcs2, annotation));

            return newGrounded;
        }
    }
}