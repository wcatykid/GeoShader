using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentSidesInTriangleImplyCongruentAngles : Theorem
    {
        private readonly static string NAME = "If two segments of a triangle are congruent, then the angles opposite those segments are congruent.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES);

        private static List<CongruentSegments> candSegs = new List<CongruentSegments>();
        private static List<Triangle> candTris = new List<Triangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candSegs.Clear();
            candTris.Clear();
        }


        //
        //       A
        //      / \
        //     B---C
        //
        // Triangle(A, B, C), Congruent(Segment(A, B), Segment(A, C)) -> Congruent(\angle ABC, \angle ACB)
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentSegments)
            {
                CongruentSegments css = clause as CongruentSegments;

                // Only generate or add to possible congruent pairs if this is a non-reflexive relation
                if (css.IsReflexive()) return newGrounded;

                for (int t = 0; t < candTris.Count; t++)
                {
                    newGrounded.AddRange(InstantiateToCongruence(candTris[t], css));
                }

                candSegs.Add(css);
            }
            else if (clause is Triangle)
            {
                Triangle newTriangle = clause as Triangle;

                //
                // Do any of the congruent segment pairs merit calling this new triangle isosceles?
                //
                foreach (CongruentSegments css in candSegs)
                {
                    newGrounded.AddRange(InstantiateToCongruence(newTriangle, css));
                }

                // Add the the list of candidates if it was not determined isosceles now.
                candTris.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Just generate the new angle congruence
        //
        private static List<EdgeAggregator> InstantiateToCongruence(Triangle tri, CongruentSegments css)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!tri.HasSegment(css.cs1) || !tri.HasSegment(css.cs2)) return newGrounded;

            GeometricCongruentAngles newConAngs = new GeometricCongruentAngles(tri.GetOppositeAngle(css.cs1), tri.GetOppositeAngle(css.cs2));

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(css);
            antecedent.Add(tri);

            newGrounded.Add(new EdgeAggregator(antecedent, newConAngs, annotation));
            
            return newGrounded;
        }
    }
}