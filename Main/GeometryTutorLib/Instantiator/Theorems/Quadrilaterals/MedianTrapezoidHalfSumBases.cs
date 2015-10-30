using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MedianTrapezoidHalfSumBases : Theorem
    {
        private readonly static string NAME = "The Median of a Trapezoid is Half the Length of the Sum of the Bases";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES);

        //  A    _______  B
        //      /       \
        //   Y /_________\ Z
        //    /           \
        // D /_____________\ C
        //
        // Trapezoid(A, B, C, D), Median(Y, Z) -> 2 * Segment(Y, Z) = Segment(A, B) + Segment(C, D)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES;

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

            Addition sum = new Addition(trapezoid.baseSegment, trapezoid.oppBaseSegment);
            Multiplication product = new Multiplication(new NumericValue(2), trapezoid.median);

            GeometricSegmentEquation gseq = new GeometricSegmentEquation(product, sum);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gseq, annotation));

            return newGrounded;
        }
    }
}