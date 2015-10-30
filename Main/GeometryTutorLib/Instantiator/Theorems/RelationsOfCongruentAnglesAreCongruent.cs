using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RelationsOfCongruentAnglesAreCongruent : Theorem
    {
        private readonly static string COMPLEMENT_NAME = "If two angles are complements of congruent angles (or of the same angle), then the two are congruent and the other combinations are complements.";
        private readonly static string SUPPLEMENT_NAME = "If two angles are supplements of congruent angles (or of the same angle), then the two are congruent and the other combinations are supplements.";
        private static Hypergraph.EdgeAnnotation compAnnotation = new Hypergraph.EdgeAnnotation(COMPLEMENT_NAME, EngineUIBridge.JustificationSwitch.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT);
        private static Hypergraph.EdgeAnnotation suppAnnotation = new Hypergraph.EdgeAnnotation(SUPPLEMENT_NAME, EngineUIBridge.JustificationSwitch.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT);

        private static List<AnglePairRelation> candRelation = new List<AnglePairRelation>();
        private static List<CongruentAngles> candCongruentAngles = new List<CongruentAngles>();

        // Resets all saved data.
        public static void Clear()
        {
            candRelation.Clear();
            candCongruentAngles.Clear();
        }

        //
        // AnglePairRelation(Angle(1), Angle(2)),
        // AnglePairRelation(Angle(3), Angle(4)),
        // Congruent(Angle(2), Angle(4)) -> Congruent(Angle(1), Angle(3))
        //                                  AnglePairRelation(Angle(1), Angle(4)),
        //                                  AnglePairRelation(Angle(2), Angle(3)),
        //
        //         |   /
        //         |1 /
        //         | / 2
        //         |/____________________
        //         |   /
        //         |3 /
        //         | / 4
        //         |/____________________
        //
        //
        // AnglePairRelation(Angle(1), Angle(2)),
        // AnglePairRelation(Angle(3), Angle(4)),
        // Congruent(Angle(2), Angle(4)) -> Congruent(Angle(1), Angle(3))
        //                                  AnglePairRelation(Angle(1), Angle(4)),
        //                                  AnglePairRelation(Angle(2), Angle(3)),
        //
        //                      /              /
        //                     /              /
        //                 1  /              /\ 2
        //       ____________/              /__\__________________
        //                      /              /
        //                     /              /
        //                 3  /              /\ 4
        //       ____________/              /__\__________________
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            compAnnotation.active = EngineUIBridge.JustificationSwitch.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT;
            suppAnnotation.active = EngineUIBridge.JustificationSwitch.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is CongruentAngles) && !(c is AnglePairRelation)) return newGrounded;

            if (c is CongruentAngles)
            {
                CongruentAngles cas = c as CongruentAngles;

                // We are not interested in reflexive relationships implying congruences or any complementary or supplementary relations.
                if (cas.IsReflexive()) return newGrounded;

                for (int i = 0; i < candRelation.Count - 1; i++)
                {
                    for (int j = i + 1; j < candRelation.Count; j++)
                    {
                        newGrounded.AddRange(IndirectRelations(cas, candRelation[i], candRelation[j]));
                    }
                }

                candCongruentAngles.Add(cas);
            }
            else if (c is AnglePairRelation)
            {
                AnglePairRelation newRelation = c as AnglePairRelation;

                foreach (AnglePairRelation relation in candRelation)
                {
                    newGrounded.AddRange(DirectRelations(relation, newRelation));

                    foreach (CongruentAngles cas in candCongruentAngles)
                    {
                        newGrounded.AddRange(IndirectRelations(cas, relation, newRelation));
                    }
                }

                candRelation.Add(newRelation);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> DirectRelations(AnglePairRelation relation1, AnglePairRelation relation2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do we have the same type of relation?
            if (relation1.GetType() != relation2.GetType()) return newGrounded;

            // Acquire the shared angle
            Angle shared = relation1.AngleShared(relation2);
            if (shared == null) return newGrounded;

            Angle otherAngle1 = relation1.OtherAngle(shared);
            Angle otherAngle2 = relation2.OtherAngle(shared);

            // Avoid generating a reflexive relationship
            if (otherAngle1.Equates(otherAngle2)) return newGrounded;

            // The other two angles are then congruent
            GeometricCongruentAngles gcas = new GeometricCongruentAngles(otherAngle1, otherAngle2);

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(relation1);
            antecedent.Add(relation2);

            newGrounded.Add(new EdgeAggregator(antecedent, gcas, relation1 is Complementary ? compAnnotation : suppAnnotation));

            return newGrounded;
        }

        private static List<EdgeAggregator> IndirectRelations(CongruentAngles cas, AnglePairRelation relation1, AnglePairRelation relation2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do we have the same type of relation?
            if (relation1.GetType() != relation2.GetType()) return newGrounded;

            //
            // Determine the shared values amongst the relations
            //
            Angle shared1 = relation1.AngleShared(cas);
            if (shared1 == null) return newGrounded;

            Angle shared2 = cas.OtherAngle(shared1);
            if (!relation2.HasAngle(shared2)) return newGrounded;

            Angle otherAngle1 = relation1.OtherAngle(shared1);
            Angle otherAngle2 = relation2.OtherAngle(shared2);

            // Avoid generating a reflexive relationship
            if (otherAngle1.Equates(otherAngle2)) return newGrounded;

            //
            // Congruent(Angle(1), Angle(3))
            //
            // The other two angles from the relation pairs are then congruent
            GeometricCongruentAngles gcas = new GeometricCongruentAngles(otherAngle1, otherAngle2);

            // Avoid direct cyclic congruent angle generation
            if (cas.StructurallyEquals(gcas)) return newGrounded;

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(cas);
            antecedent.Add(relation1);
            antecedent.Add(relation2);

            //
            // AnglePairRelation(Angle(1), Angle(4)),
            // AnglePairRelation(Angle(2), Angle(3)),
            //
            if (relation1 is Complementary && relation2 is Complementary)
            {
                Complementary comp1 = new Complementary(shared1, otherAngle2);
                Complementary comp2 = new Complementary(shared2, otherAngle1);

                newGrounded.Add(new EdgeAggregator(antecedent, comp1, compAnnotation));
                newGrounded.Add(new EdgeAggregator(antecedent, comp2, compAnnotation));
            }
            else if (relation1 is Supplementary && relation2 is Supplementary)
            {
                Supplementary supp1 = new Supplementary(shared1, otherAngle2);
                Supplementary supp2 = new Supplementary(shared2, otherAngle1);

                newGrounded.Add(new EdgeAggregator(antecedent, supp1, suppAnnotation));
                newGrounded.Add(new EdgeAggregator(antecedent, supp2, suppAnnotation));
            }
            else
            {
                throw new ArgumentException("RelationsOfCongruent:: Expected a supplementary or complementary angle, not " + relation1.GetType());
            }

            newGrounded.Add(new EdgeAggregator(antecedent, gcas, relation1 is Complementary ? compAnnotation : suppAnnotation));

            return newGrounded;
        }
    }
}