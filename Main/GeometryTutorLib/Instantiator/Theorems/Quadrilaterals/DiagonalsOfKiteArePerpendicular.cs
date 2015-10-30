using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiagonalsOfKiteArePerpendicular : Theorem
    {
        private readonly static string NAME = "Diagonals Of Kite are Perpendicular";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.DIAGONALS_OF_KITE_ARE_PERPENDICULAR);

        //
        // Kite(A, B, C, D) -> Perpendicular(Intersection(Segment(A, C), Segment(B, D))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.DIAGONALS_OF_KITE_ARE_PERPENDICULAR;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Kite)
            {
                Kite kite = clause as Kite;

                newGrounded.AddRange(InstantiateToTheorem(kite, kite));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Kite)) return newGrounded;

                newGrounded.AddRange(InstantiateToTheorem(streng.strengthened as Kite, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTheorem(Kite kite, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Instantiate this kite diagonals ONLY if the original figure has the diagonals drawn.
            if (kite.diagonalIntersection == null) return newGrounded;

            // Determine the CongruentSegments opposing sides and output that.
            Strengthened newPerpendicular = new Strengthened(kite.diagonalIntersection, new Perpendicular(kite.diagonalIntersection));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newPerpendicular, annotation));

            return newGrounded;
        }
    }
}