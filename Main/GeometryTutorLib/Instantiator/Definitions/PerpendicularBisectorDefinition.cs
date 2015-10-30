using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularBisectorDefinition : Definition
    {
        private readonly static string NAME = "Definition of Perpendicular Bisector";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PERPENDICULAR_BISECTOR_DEFINITION);

        //
        // PerpendicularBisector(Intersection(X, Segment(A, B), Segment(C, D)), Bisector(Segment(C, D))) ->
        //                         Perpendicular(Intersection(X, Segment(A, B), Segment(C, D)), Bisector(Segment(C, D))),
        //                         SegmentBisector(Intersection(X, Segment(A, B), Segment(C, D)), Bisector(Segment(C, D)))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.PERPENDICULAR_BISECTOR_DEFINITION;

            if (clause is PerpendicularBisector) return InstantiateFromPerpendicularBisector(clause, clause as PerpendicularBisector);

            if ((clause as Strengthened).strengthened is PerpendicularBisector)
            {
                return InstantiateFromPerpendicularBisector(clause, (clause as Strengthened).strengthened as PerpendicularBisector);
            }

            return new List<EdgeAggregator>();
        }

        public static List<EdgeAggregator> InstantiateFromPerpendicularBisector(GroundedClause original, PerpendicularBisector pb)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            Strengthened streng1 = new Strengthened(pb.originalInter, new Perpendicular(pb.originalInter));
            Strengthened streng2 = new Strengthened(pb.originalInter, new SegmentBisector(pb.originalInter, pb.bisector));

            newGrounded.Add(new EdgeAggregator(antecedent, streng1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, streng2, annotation));

            return newGrounded;
        }
    }
}