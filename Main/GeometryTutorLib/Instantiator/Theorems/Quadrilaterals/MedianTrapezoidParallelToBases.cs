using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MedianTrapezoidParallelToBases : Theorem
    {
        private readonly static string NAME = "The Median of a Trapezoid is Parallel to the Bases";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_PARALLEL_TO_BASE);

        //  A    _______  B
        //      /       \
        //   Y /_________\ Z
        //    /           \
        // D /_____________\ C
        //
        // Trapezoid(A, B, C, D), Median(Y, Z) -> Parallel(Segment(A, B), Segment(Y, Z)), Parallel(Segment(C, D), Segment(Y, Z))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_PARALLEL_TO_BASE;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Trapezoid)
            {
                Trapezoid trapezoid = clause as Trapezoid;

                newGrounded.AddRange(InstantiateToTheorem(trapezoid, trapezoid));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Trapezoid)) return newGrounded;

                newGrounded.AddRange(InstantiateToTheorem(streng.strengthened as Trapezoid, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Trapezoid trapezoid, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If median has not been checked, check now
            if (!trapezoid.IsMedianChecked()) trapezoid.FindMedian();
            // Generate only if the median is valid (exists in the original figure)
            if (!trapezoid.IsMedianValid()) return newGrounded;

            GeometricParallel newParallel1 = new GeometricParallel(trapezoid.median, trapezoid.oppBaseSegment);
            GeometricParallel newParallel2 = new GeometricParallel(trapezoid.median, trapezoid.baseSegment);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallel1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, newParallel2, annotation));

            return newGrounded;
        }
    }
}