using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class IsoscelesTriangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Isosceles Triangle";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_DEFINITION);

        public static Boolean MayUnifyWith(GroundedClause c)
        {
            return (c is CongruentSegments) || (c is Triangle) || (c is Strengthened);
        }

        private static List<CongruentSegments> candSegs = new List<CongruentSegments>();
        private static List<Triangle> candTris = new List<Triangle>();
        private static List<IsoscelesTriangle> candIsoTris = new List<IsoscelesTriangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candSegs.Clear();
            candTris.Clear();
            candIsoTris.Clear();
        }

        //
        // In order for two triangles to be isosceles, we require the following:
        //
        //    Triangle(A, B, C), Congruent(Segment(A, B), Segment(B, C)) -> IsoscelesTriangle(A, B, C)
        //
        //  This does not generate a new clause explicitly; it simply strengthens the existent object
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_DEFINITION;

            if (c is IsoscelesTriangle || c is Strengthened) return InstantiateDefinition(c);

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is CongruentSegments) && !(c is Triangle)) return newGrounded;

            //
            // Unify
            //
            if (c is CongruentSegments)
            {
                CongruentSegments css = c as CongruentSegments;

                // Only generate or add to possible congruent pairs if this is a non-reflexive relation
                if (css.IsReflexive()) return newGrounded;

                for (int t = 0; t < candTris.Count; t++)
                {
                    if (candTris[t].HasSegment(css.cs1) && candTris[t].HasSegment(css.cs2))
                    {
                        newGrounded.Add(StrengthenToIsosceles(candTris[t], css));

                        // There should be only one possible Isosceles triangle from this congruent segments
                        // So we can remove this relationship and triangle from consideration
                        candTris.RemoveAt(t);

                        return newGrounded;
                    }
                }

                candSegs.Add(css);
            }

            else if (c is Triangle)
            {
                Triangle newTriangle = c as Triangle;

                //
                // Do any of the congruent segment pairs merit calling this new triangle isosceles?
                //
                for (int cs = 0; cs < candSegs.Count; cs++)
                {
                    if (newTriangle.HasSegment(candSegs[cs].cs1) && newTriangle.HasSegment(candSegs[cs].cs2))
                    {
                        newGrounded.Add(StrengthenToIsosceles(newTriangle, candSegs[cs]));

                        return newGrounded;
                    }
                }

                // Add to the list of candidates if it was not determined isosceles now.
                candTris.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // DO NOT generate a new clause, instead, report the result and generate all applicable
        // clauses attributed to this strengthening of a triangle from scalene to isosceles
        //
        private static EdgeAggregator StrengthenToIsosceles(Triangle tri, CongruentSegments ccss)
        {
            Strengthened newStrengthened = new Strengthened(tri, new IsoscelesTriangle(tri));

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(ccss);
            antecedent.Add(tri);

            return new EdgeAggregator(antecedent, newStrengthened, annotation);
        }

        //
        // IsoscelesTriangle(A, B, C) -> Congruent(Segment(A, B), Segment(A, C))
        //
        private static List<EdgeAggregator> InstantiateDefinition(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is EquilateralTriangle || (clause as Strengthened).strengthened is EquilateralTriangle) return newGrounded;

            if (clause is IsoscelesTriangle) return InstantiateDefinition(clause, clause as IsoscelesTriangle);

            if ((clause as Strengthened).strengthened is IsoscelesTriangle)
            {
                return InstantiateDefinition(clause, (clause as Strengthened).strengthened as IsoscelesTriangle);
            }

            return new List<EdgeAggregator>();
        }

        private static List<EdgeAggregator> InstantiateDefinition(GroundedClause original, IsoscelesTriangle isoTri)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            GeometricCongruentSegments gcs = new GeometricCongruentSegments(isoTri.leg1, isoTri.leg2);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gcs, annotation));

            return newGrounded;
        }
    }
}