using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ArcAdditionAxiom
    {
        private static readonly string NAME = "Arc Addition Axiom";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ARC_ADDITION_AXIOM);

        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ARC_ADDITION_AXIOM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            ArcInMiddle im = c as ArcInMiddle;

            if (im == null) return newGrounded;

            Addition sum = null;

            // Temporarily assume that the two arcs formed by the intersection are both minor arcs
            MinorArc a1 = new MinorArc(im.arc.theCircle, im.arc.endpoint1, im.point);
            MinorArc a2 = new MinorArc(im.arc.theCircle, im.point, im.arc.endpoint2);

            // If the intersected arc is a minor arc, this will always be true.
            // If the intersected arc is a major arc, this might be true. Other case is one minor arc and one major arc.
            if (Arc.BetweenMajor(im.point, im.arc))
            {
                // Check if both arcs are genuinely minor arcs. 
                // If the other endpoint falls in the new arc, then the major arc should be used instead
                if (Arc.BetweenMinor(im.arc.endpoint2, a1))
                {
                    MajorArc majorArc = new MajorArc(im.arc.theCircle, im.arc.endpoint1, im.point);
                    sum = new Addition(majorArc, a2);
                }
                else if (Arc.BetweenMinor(im.arc.endpoint1, a2))
                {
                    MajorArc majorArc = new MajorArc(im.arc.theCircle, im.point, im.arc.endpoint2);
                    sum = new Addition(a1, majorArc);
                }
            }

            if (sum == null) sum = new Addition(a1, a2);

            GeometricArcEquation eq = new GeometricArcEquation(sum, im.arc);
            eq.MakeAxiomatic();

            // For hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(im);
            newGrounded.Add(new EdgeAggregator(antecedent, eq, annotation));


            return newGrounded;
        }
    }
}
