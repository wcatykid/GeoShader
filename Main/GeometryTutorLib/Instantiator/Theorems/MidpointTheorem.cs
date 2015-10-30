using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class MidpointTheorem : Theorem
    {
        private readonly static string NAME = "Midpoint Theorem";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.MIDPOINT_THEOREM);

        //
        // Midpoint(M, Segment(A, B)) -> 2AM = AB, 2BM = AB          A ------------- M ------------- B
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.MIDPOINT_THEOREM;

            if (clause is Midpoint) return InstantiateMidpointTheorem(clause, clause as Midpoint);

            if ((clause as Strengthened).strengthened is Midpoint) return InstantiateMidpointTheorem(clause, (clause as Strengthened).strengthened as Midpoint);

            return new List<EdgeAggregator>();
        }

        public static List<EdgeAggregator> InstantiateMidpointTheorem(GroundedClause original, Midpoint midpt)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Construct 2AM
            Multiplication product1 = new Multiplication(new NumericValue(2), new Segment(midpt.point, midpt.segment.Point1));
            // Construct 2BM
            Multiplication product2 = new Multiplication(new NumericValue(2), new Segment(midpt.point, midpt.segment.Point2));

            // 2X = AB
            GeometricSegmentEquation newEq1 = new GeometricSegmentEquation(product1, midpt.segment);
            GeometricSegmentEquation newEq2 = new GeometricSegmentEquation(product2, midpt.segment);

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newEq1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, newEq2, annotation));

            return newGrounded;
        }
    }
}