using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class SupplementaryAndCongruentImplyRightAngles :Theorem
    {
        private readonly static string NAME = "Congruent Supplementary Angles Are Right Angles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES);

        private static List<Supplementary> candSupplementary = new List<Supplementary>();
        private static List<CongruentAngles> candCongruent = new List<CongruentAngles>();

        // Resets all saved data.
        public static void Clear()
        {
            candSupplementary.Clear();
            candCongruent.Clear();
        }

        //
        // Supplementary(Angle(A, B, D), Angle(B, A, C))
        // Congruent(Angle(A, B, D), Angle(B, A, C)) -> RightAngle(Angle(A, B, D))
        //                                           -> RightAngle(Angle(B, A, C))
        //                            
        //                            C            D
        //                            |            |
        //                            |____________|
        //                            A            B
        //                            
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (c is CongruentAngles)
            {
                CongruentAngles conAngles = c as CongruentAngles;

                // We are interested in adjacent angles, not reflexive
                if (conAngles.IsReflexive()) return newGrounded;

                foreach (Supplementary suppAngles in candSupplementary)
                {
                    newGrounded.AddRange(CheckAndGenerateSupplementaryCongruentImplyRightAngles(suppAngles, conAngles));
                }

                candCongruent.Add(conAngles);
            }
            else if (c is Supplementary)
            {
                Supplementary supplementary = c as Supplementary;

                foreach (CongruentAngles cc in candCongruent)
                {
                    newGrounded.AddRange(CheckAndGenerateSupplementaryCongruentImplyRightAngles(supplementary, cc));
                }

                candSupplementary.Add(supplementary);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateSupplementaryCongruentImplyRightAngles(Supplementary supplementary, CongruentAngles conAngles)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //The given pairs must contain the same angles (i.e., the angles must be both supplementary AND congruent)
            if (!((supplementary.angle1.Equals(conAngles.ca1) && supplementary.angle2.Equals(conAngles.ca2)) ||
               (supplementary.angle2.Equals(conAngles.ca1) && supplementary.angle1.Equals(conAngles.ca2)))) return newGrounded;
            //if (!(supplementary.StructurallyEquals(conAngles))) return newGrounded;

            //
            // Now we have two supplementary and congruent angles, which must be right angles
            //
            Strengthened streng = new Strengthened(supplementary.angle1, new RightAngle(supplementary.angle1));
            Strengthened streng2 = new Strengthened(supplementary.angle2, new RightAngle(supplementary.angle2));

            // Construct hyperedges
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(supplementary);
            antecedent.Add(conAngles);

            newGrounded.Add(new EdgeAggregator(antecedent, streng, annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, streng2, annotation));

            return newGrounded;
        }
    }
}
