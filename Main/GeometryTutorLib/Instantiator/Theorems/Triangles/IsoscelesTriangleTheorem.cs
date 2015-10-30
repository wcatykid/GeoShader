using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class IsoscelesTriangleTheorem : Theorem
    {
        private readonly static string NAME = "Isosceles Triangle Theorem: Base Angles are Congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_THEOREM);

        //
        // In order for two triangles to be isosceles, we require the following:
        //    IsoscelesTriangle(A, B, C) -> \angle BAC \cong \angle BCA
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_THEOREM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (c is EquilateralTriangle || (c as Strengthened).strengthened is EquilateralTriangle) return newGrounded;

            if (c is IsoscelesTriangle) return InstantiateTheorem(c, c as IsoscelesTriangle);

            Strengthened streng = c as Strengthened;
            if (streng != null)
            {
                if (streng.strengthened is IsoscelesTriangle)
                {
                    return InstantiateTheorem(c, streng.strengthened as IsoscelesTriangle);
                }
            }

            return newGrounded;
        }

        public static List<EdgeAggregator> InstantiateTheorem(GroundedClause original, IsoscelesTriangle isoTri)
        {
            GeometricCongruentAngles newCongSegs = new GeometricCongruentAngles(isoTri.baseAngleOppositeLeg1, isoTri.baseAngleOppositeLeg2);

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(original);

            return Utilities.MakeList<EdgeAggregator>(new EdgeAggregator(antecedent, newCongSegs, annotation));
        }
    }
}