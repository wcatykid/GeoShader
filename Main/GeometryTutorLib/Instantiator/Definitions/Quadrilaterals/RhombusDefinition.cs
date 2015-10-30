using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RhombusDefinition : Definition
    {
        private readonly static string NAME = "Definition of Rhombus";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.RHOMBUS_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateQuadrilateral.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.RHOMBUS_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral || clause is CongruentSegments)
            {
                newGrounded.AddRange(InstantiateToRhombus(clause));
            }

            else if (clause is Rhombus || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromRhombus(clause));
            }

            return newGrounded;
        }

        //     A _________________ B
        //      /                /
        //     /                /
        //    /                /
        // D /________________/ C
        //
        // Quadrilateral(A, B, C, D), Congruent(Segment(A, B), Segment(C, D)),
        // Congruent(Segment(C, D), Segment(A, D)), Congruent(Segment(A, D), Segment(B, C)) -> Rhombus(A, B, C, D)
        //
        private static List<EdgeAggregator> InstantiateFromRhombus(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Rhombus)
            {
                Rhombus rhombus  = clause as Rhombus;

                newGrounded.AddRange(InstantiateFromRhombus(rhombus, rhombus));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Rhombus)) return newGrounded;

                newGrounded.AddRange(InstantiateFromRhombus(streng.strengthened as Rhombus, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromRhombus(Rhombus rhombus, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            // Determine the CongruentSegments : 4 Choose 2
            newGrounded.Add(new EdgeAggregator(antecedent, new GeometricCongruentSegments(rhombus.top, rhombus.bottom), annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, new GeometricCongruentSegments(rhombus.top, rhombus.left), annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, new GeometricCongruentSegments(rhombus.top, rhombus.right), annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, new GeometricCongruentSegments(rhombus.bottom, rhombus.left), annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, new GeometricCongruentSegments(rhombus.bottom, rhombus.right), annotation));
            newGrounded.Add(new EdgeAggregator(antecedent, new GeometricCongruentSegments(rhombus.left, rhombus.right), annotation));

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        //
        // Quadrilateral(A, B, C, D), Congruent(Segment(A, B), Segment(C, D)),
        // Congruent(Segment(C, D), Segment(A, D)), Congruent(Segment(A, D), Segment(B, C)) -> Rhombus(A, B, C, D)
        //
        private static List<Quadrilateral> candidateQuadrilateral = new List<Quadrilateral>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        private static List<EdgeAggregator> InstantiateToRhombus(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral)
            {
                Quadrilateral quad = clause as Quadrilateral;

                if (!quad.IsStrictQuadrilateral()) return newGrounded;

                for (int i = 0; i < candidateCongruent.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateCongruent.Count; j++)
                    {
                        for (int k = j + 1; k < candidateCongruent.Count; k++)
                        {
                            newGrounded.AddRange(InstantiateToRhombus(quad, candidateCongruent[i], candidateCongruent[j], candidateCongruent[k]));
                        }
                    }
                }

                candidateQuadrilateral.Add(quad);
            }
            else if (clause is CongruentSegments)
            {
                CongruentSegments newCs = clause as CongruentSegments;

                if (newCs.IsReflexive()) return newGrounded;

                foreach (Quadrilateral quad in candidateQuadrilateral)
                {
                    for (int i = 0; i < candidateCongruent.Count - 1; i++)
                    {
                        for (int j = i + 1; j < candidateCongruent.Count; j++)
                        {
                            newGrounded.AddRange(InstantiateToRhombus(quad, newCs, candidateCongruent[i], candidateCongruent[j]));
                        }
                    }
                }

                candidateCongruent.Add(newCs);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToRhombus(Quadrilateral quad, CongruentSegments cs1, CongruentSegments cs2, CongruentSegments cs3)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // The 3 congruent segments pairs must relate; one pair must link the two others.
            // Determine the link segments as well as the opposite sides.
            //
            CongruentSegments link = null;
            CongruentSegments opp1 = null;
            CongruentSegments opp2 = null;
            if (cs1.SharedSegment(cs2) != null && cs1.SharedSegment(cs3) != null)
            {
                link = cs1;
                opp1 = cs2;
                opp2 = cs3;
            }
            else if (cs2.SharedSegment(cs1) != null && cs2.SharedSegment(cs3) != null)
            {
                link = cs2;
                opp1 = cs1;
                opp2 = cs3;
            }
            else if (cs3.SharedSegment(cs1) != null && cs3.SharedSegment(cs2) != null)
            {
                link = cs3;
                opp1 = cs1;
                opp2 = cs2;
            }
            else return newGrounded;

            // Are the pairs on the opposite side of this quadrilateral?
            if (!quad.HasOppositeCongruentSides(opp1)) return newGrounded;
            if (!quad.HasOppositeCongruentSides(opp2)) return newGrounded;

            //
            // Create the new Rhombus object
            //
            Strengthened newRhombus = new Strengthened(quad, new Rhombus(quad));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(cs1);
            antecedent.Add(cs2);
            antecedent.Add(cs3);

            newGrounded.Add(new EdgeAggregator(antecedent, newRhombus, annotation));

            return newGrounded;
        }
    }
}