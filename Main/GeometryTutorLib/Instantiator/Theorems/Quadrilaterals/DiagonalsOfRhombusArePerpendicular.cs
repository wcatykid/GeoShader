using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiagonalsOfRhombusArePerpendicular : Theorem
    {
        private readonly static string NAME = "Diagonals Of Rhombi are Perpendicular";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR);

        //  A __________  B
        //   |          |
        //   |          |
        //   |          |
        // D |__________| C
        //
        // Rhombus(A, B, C, D) -> Perpendicular(Intersection(Segment(A, C), Segment(B, D))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rhombus)
            {
                Rhombus rhombus = clause as Rhombus;

                newGrounded.AddRange(InstantiateToTheorem(rhombus, rhombus));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Rhombus)) return newGrounded;

                newGrounded.AddRange(InstantiateToTheorem(streng.strengthened as Rhombus, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Rhombus rhombus, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Instantiate this rhombus diagonals ONLY if the original figure has the diagonals drawn.
            if (rhombus.diagonalIntersection == null) return newGrounded;

            // Determine the CongruentSegments opposing sides and output that.
            Strengthened newPerpendicular = new Strengthened(rhombus.diagonalIntersection, new Perpendicular(rhombus.diagonalIntersection));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newPerpendicular, annotation));

            return newGrounded;
        }
    }
}