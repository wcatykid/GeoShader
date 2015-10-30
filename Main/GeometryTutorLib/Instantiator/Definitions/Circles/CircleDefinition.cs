using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CircleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Circle: Radii Congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CIRCLE_DEFINITION);

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CIRCLE_DEFINITION;

            if (clause is Circle)
            {
                return InstantiateFromDefinition(clause as Circle);
            }

            return new List<EdgeAggregator>();
        }

        //
        // All radii of a circle are congruent.
        //
        private static List<GenericInstantiator.EdgeAggregator> InstantiateFromDefinition(Circle circle)
        {
            List<EdgeAggregator> congRadii = new List<EdgeAggregator>();

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(circle);

            for (int r1 = 0; r1 < circle.radii.Count; r1++)
            {
                for (int r2 = r1 + 1; r2 < circle.radii.Count; r2++)
                {
                    GeometricCongruentSegments gcs = new GeometricCongruentSegments(circle.radii[r1], circle.radii[r2]);
                    congRadii.Add(new GenericInstantiator.EdgeAggregator(antecedent, gcs, annotation));
                }
            }

            return congRadii;
        }
    }
}