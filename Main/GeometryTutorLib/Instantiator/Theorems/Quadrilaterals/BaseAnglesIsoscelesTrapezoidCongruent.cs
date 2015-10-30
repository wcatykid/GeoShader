using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class BaseAnglesIsoscelesTrapezoidCongruent : Theorem
    {
        private readonly static string NAME = "Base Angles of Isosceles Trapezoid are Congruent";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT);

        //  A    _______  B
        //      /       \
        //     /         \
        //    /           \
        // D /_____________\ C
        //
        // IsoscelesTrapezoid(A, B, C, D) -> Congruent(Angle(A, D, C), Angle(B, C, D)), Congruent(Angle(D, A, B), Angle(C, B, A))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is IsoscelesTrapezoid)
            {
                IsoscelesTrapezoid trapezoid = clause as IsoscelesTrapezoid;

                newGrounded.AddRange(InstantiateToTheorem(trapezoid, trapezoid));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is IsoscelesTrapezoid)) return newGrounded;

                newGrounded.AddRange(InstantiateToTheorem(streng.strengthened as IsoscelesTrapezoid, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(IsoscelesTrapezoid trapezoid, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            GeometricCongruentAngles gcas1 = new GeometricCongruentAngles(trapezoid.bottomLeftBaseAngle, trapezoid.bottomRightBaseAngle);
            GeometricCongruentAngles gcas2 = new GeometricCongruentAngles(trapezoid.topLeftBaseAngle, trapezoid.topRightBaseAngle);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gcas1, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, gcas2, annotation));

            return newGrounded;
        }
    }
}