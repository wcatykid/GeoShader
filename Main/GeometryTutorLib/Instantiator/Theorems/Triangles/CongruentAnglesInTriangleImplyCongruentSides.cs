using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CongruentAnglesInTriangleImplyCongruentSides : Theorem
    {
        private readonly static string NAME = "If two angles of a triangle are congruent, then the sides opposite those angles are congruent.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES);

        public static Boolean MayUnifyWith(GroundedClause c)
        {
            return (c is CongruentAngles) || (c is Triangle);
        }

        private static List<CongruentAngles> candAngs = new List<CongruentAngles>();
        private static List<Triangle> candTris = new List<Triangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candAngs.Clear();
            candTris.Clear();
        }


        //
        //       A
        //      / \
        //     B---C
        //
        // Triangle(A, B, C), Congruent(\angle ABC, \angle ACB) -> Congruent(Segment(A, B), Segment(A, C))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!MayUnifyWith(c)) return newGrounded;

            //
            // Unify
            //
            if (c is CongruentAngles)
            {
                CongruentAngles cas = c as CongruentAngles;

                // Only generate or add to possible congruent pairs if this is a non-reflexive relation
                if (!cas.IsReflexive())
                {
                    for (int t = 0; t < candTris.Count; t++)
                    {
                        if (candTris[t].HasAngle(cas.ca1) && candTris[t].HasAngle(cas.ca2))
                        {
                            newGrounded.Add(GenerateCongruentSides(candTris[t], cas));

                            // There should be only one possible Isosceles triangle from this congruent angles
                            // So we can remove this relationship and triangle from consideration
                            candTris.RemoveAt(t);

                            return newGrounded;
                        }
                    }

                    candAngs.Add(cas);
                }

                return new List<EdgeAggregator>();
            }

            else if (c is Triangle)
            {
                Triangle newTriangle = c as Triangle;

                //
                // Do any of the congruent segment pairs merit calling this new triangle isosceles?
                //
                for (int ca = 0; ca < candAngs.Count; ca++)
                {
                    // No need to check for this, in theory, since we never add any reflexive expressions to the list
                    if (!candAngs[ca].IsReflexive())
                    {
                        if (newTriangle.HasAngle(candAngs[ca].ca1) && newTriangle.HasAngle(candAngs[ca].ca2))
                        {
                            newGrounded.Add(GenerateCongruentSides(newTriangle, candAngs[ca]));

                            return newGrounded;
                        }
                    }
                }

                // Add the the list of candidates if it was not determined isosceles now.
                candTris.Add(newTriangle);
            }

            return newGrounded;
        }

        //
        // Just generate the new triangle
        //
        private static EdgeAggregator GenerateCongruentSides(Triangle tri, CongruentAngles cas)
        {
            GeometricCongruentSegments newConSegs = new GeometricCongruentSegments(tri.OtherSide(cas.ca1), tri.OtherSide(cas.ca2));

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(cas);
            antecedent.Add(tri);

            return new EdgeAggregator(antecedent, newConSegs, annotation);
        }
    }
}