using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Congruent : Descriptor
    {
        public Congruent() : base() {}

        public virtual int SharesNumClauses(Congruent thatCC) { return 0; }

        private static string NAME = "Transitivity";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRANSITIVE_SUBSTITUTION);

        public static List<GenericInstantiator.EdgeAggregator> CreateTransitiveCongruence(Congruent congruent1, Congruent congruent2)
        {
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            //
            // Create the antecedent clauses
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(congruent1);
            antecedent.Add(congruent2);

            //
            // Create the consequent clause
            //
            Congruent newCC = null;
            if (congruent1 is CongruentSegments)
            {
                CongruentSegments css1 = congruent1 as CongruentSegments;
                CongruentSegments css2 = congruent2 as CongruentSegments;

                Segment shared = css1.SegmentShared(css2);

                newCC = new AlgebraicCongruentSegments(css1.OtherSegment(shared), css2.OtherSegment(shared));
            }
            else if (congruent1 is CongruentAngles)
            {
                CongruentAngles cas1 = congruent1 as CongruentAngles;
                CongruentAngles cas2 = congruent2 as CongruentAngles;

                Angle shared = cas1.AngleShared(cas2);

                newCC = new AlgebraicCongruentAngles(cas1.OtherAngle(shared), cas2.OtherAngle(shared));
            }

            if (newCC == null)
            {
                System.Diagnostics.Debug.WriteLine("");
                throw new NullReferenceException("Unexpected Problem in Atomic substitution...");
            }

            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, newCC, annotation));

            return newGrounded;
        }
    }
}
