using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SegmentAdditionAxiom : Axiom
    {
        private static readonly string NAME = "Segment Addition Axiom";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.SEGMENT_ADDITION_AXIOM);

        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.SEGMENT_ADDITION_AXIOM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            InMiddle im = c as InMiddle;

            if (im == null) return newGrounded;

            Segment s1 = new Segment(im.segment.Point1, im.point);
            Segment s2 = new Segment(im.point, im.segment.Point2);
            Addition sum = new Addition(s1, s2);
            GeometricSegmentEquation eq = new GeometricSegmentEquation(sum, im.segment);
            eq.MakeAxiomatic();

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(im);
            newGrounded.Add(new EdgeAggregator(antecedent, eq, annotation));
           

            return newGrounded;
        }
    }
}