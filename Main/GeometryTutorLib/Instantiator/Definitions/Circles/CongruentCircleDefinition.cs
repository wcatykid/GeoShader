using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentCircleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Congruent Circles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CIRCLE_CONGRUENCE_DEFINITION);

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CIRCLE_CONGRUENCE_DEFINITION;

            if (clause is CongruentCircles)
            {
                return InstantiateFromDefinition(clause as CongruentCircles);
            }

            if (clause is CongruentSegments)
            {
                return InstantiateToDefinition(clause as CongruentSegments);
            }

            return new List<EdgeAggregator>();
        }

        private static List<EdgeAggregator> InstantiateToDefinition(CongruentSegments css)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Find all the circles for which the two segments are radii.
            //
            List<Circle> circlesCSS1 = Circle.GetFigureCirclesByRadius(css.cs1);
            List<Circle> circlesCSS2 = Circle.GetFigureCirclesByRadius(css.cs2);

            //
            // Since the radii are congruent, the circles are therefore congruent.
            //
            foreach (Circle cc1 in circlesCSS1)
            {
                foreach (Circle cc2 in circlesCSS2)
                {
                    // Avoid generating refexive relationships(?)
                    if (!cc1.StructurallyEquals(cc2))
                    {
                        // For hypergraph
                        List<GroundedClause> antecedent = new List<GroundedClause>();
                        antecedent.Add(css);
                        antecedent.Add(cc1);
                        antecedent.Add(cc2);

                        GeometricCongruentCircles gccs = new GeometricCongruentCircles(cc1, cc2);
                        newGrounded.Add(new EdgeAggregator(antecedent, gccs, annotation));
                    }
                }
            }

            return newGrounded;
        }

        //
        // All radii of the congruent circles are congruent.
        //
        private static List<GenericInstantiator.EdgeAggregator> InstantiateFromDefinition(CongruentCircles cs)
        {
            List<EdgeAggregator> congRadii = new List<EdgeAggregator>();

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(cs);

            for (int r1 = 0; r1 < cs.cc1.radii.Count; r1++)
            {
                for (int r2 = 0; r2 < cs.cc2.radii.Count; r2++)
                {
                    GeometricCongruentSegments gcs = new GeometricCongruentSegments(cs.cc1.radii[r1], cs.cc2.radii[r2]);
                    congRadii.Add(new GenericInstantiator.EdgeAggregator(antecedent, gcs, annotation));
                }
            }

            return congRadii;
        }
    }
}