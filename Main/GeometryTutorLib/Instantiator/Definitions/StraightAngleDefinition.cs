using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class StraightAngleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Straight Angle";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.STRAIGHT_ANGLE_DEFINITION);

        public StraightAngleDefinition() { }

        //
        // Collinear(A, B, C, D, ...) -> Angle(A, B, C), Angle(A, B, D), Angle(A, C, D), Angle(B, C, D),...
        // All angles will have measure 180^o
        // There will be nC3 resulting clauses.
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.STRAIGHT_ANGLE_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Collinear cc = clause as Collinear;
            if (cc == null) return newGrounded;

            for (int i = 0; i < cc.points.Count - 2; i++)
            {
                for (int j = i + 1; j < cc.points.Count - 1; j++)
                {
                    for (int k = j + 1; k < cc.points.Count; k++)
                    {
                        Angle newAngle = new Angle(cc.points[i], cc.points[j], cc.points[k]);
                        List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(cc);
                        newGrounded.Add(new EdgeAggregator(antecedent, newAngle, annotation));
                    }
                }
            }

            return newGrounded;
        }
    }
}