using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AcuteAnglesInRightTriangleComplementary : Theorem
    {
        private readonly static string NAME = "The two acute angles in a right triangle are complementary.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY);

        //
        // In order for two triangles to be congruent, we require the following:
        //    RightTriangle(A, B, C) -> Complementary(Angle(B, A, C), Angle(A, C, B))
        //     A 
        //    | \
        //    |  \
        //    |   \
        //    |    \
        //    |_____\
        //    B      C
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Triangle) && !(c is Strengthened)) return newGrounded;

            if (c is Triangle)
            {
                Triangle tri = c as Triangle;

                if (!(tri is RightTriangle))
                {
                    if (!tri.provenRight) return newGrounded;
                }
                newGrounded.AddRange(InstantiateRightTriangle(tri, c));
            }

            if (c is Strengthened)
            {
                Strengthened streng = c as Strengthened;

                if (!(streng.strengthened is RightTriangle)) return newGrounded;

                newGrounded.AddRange(InstantiateRightTriangle(streng.strengthened as RightTriangle, c));
            }

            return newGrounded;
        }

        //
        // We know at this point that we have a right triangle
        //
        private static List<EdgeAggregator> InstantiateRightTriangle(Triangle tri, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            KeyValuePair<Angle, Angle> acuteAngles = tri.GetAcuteAngles();

            Complementary newComp = new Complementary(acuteAngles.Key, acuteAngles.Value);

            // Hypergraph
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newComp, annotation));

            return newGrounded;
        }
    }
}