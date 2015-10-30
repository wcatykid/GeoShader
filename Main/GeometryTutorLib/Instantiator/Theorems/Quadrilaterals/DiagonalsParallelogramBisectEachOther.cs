using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiagonalsParallelogramBisectEachOther : Theorem
    {
        private readonly static string NAME = "Diagonals of a Parallelogram Bisect Each Other";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER);

        //     A _________________ B
        //      /                /
        //     /                /
        //    /                /
        // D /________________/ C
        //
        // Parallelogram(A, B, C, D) -> SegmentBisector(Segment(A, C), Segment(B, D)), SegmentBisector(Segment(B, D), Segment(A, C)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallelogram)
            {
                Parallelogram newPara = clause as Parallelogram;

                newGrounded.AddRange(InstantiateTheorem(newPara, newPara));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Parallelogram)) return newGrounded;

                newGrounded.AddRange(InstantiateTheorem(streng.strengthened as Parallelogram, streng));
            }

            return newGrounded;
        }

        //     A _________________ B
        //      /                /
        //     /     \/         /
        //    /      /\        /
        // D /________________/ C
        //
        // Parallelogram(A, B, C, D) -> SegmentBisector(Segment(A, C), Segment(B, D)), SegmentBisector(Segment(B, D), Segment(A, C)),
        //
        private static List<EdgeAggregator> InstantiateTheorem(Parallelogram parallelogram, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Generate only if the diagonals are a part of the original figure.
            if (parallelogram.diagonalIntersection == null) return newGrounded;

            // Determine the CongruentSegments opposing sides and output that.
            Intersection diagInter = parallelogram.diagonalIntersection;
            Strengthened sb1 = new Strengthened(diagInter, new SegmentBisector(diagInter, diagInter.lhs));
            Strengthened sb2 = new Strengthened(diagInter, new SegmentBisector(diagInter, diagInter.rhs));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, sb1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, sb2, annotation));

            return newGrounded;
        }
    }
}