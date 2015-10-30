using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class DiagonalsBisectEachOtherImplyParallelogram : Theorem
    {
        private readonly static string NAME = "If Diagonals of a Quadrilateral Bisect Each Other, then the Quadrilateral is a Parallelogram ";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM);

        public static void Clear()
        {
            candidateBisectors.Clear();
            candidateQuadrilaterals.Clear();
            candidateStrengthened.Clear();
        }

        private static List<SegmentBisector> candidateBisectors = new List<SegmentBisector>();
        private static List<Quadrilateral> candidateQuadrilaterals = new List<Quadrilateral>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        //     A _________________ B
        //      /                /
        //     /                /
        //    /                /
        // D /________________/ C
        //
        // Parallelogram(A, B, C, D) -> SegmentBisector(Segment(A, C), Segment(B, D)), SegmentBisector(Segment(B, D), Segment(A, C)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral)
            {
                Quadrilateral quad = clause as Quadrilateral;

                if (!quad.IsStrictQuadrilateral()) return newGrounded;

                for (int i = 0; i < candidateBisectors.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateBisectors.Count; j++)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, candidateBisectors[i], candidateBisectors[j], candidateBisectors[i], candidateBisectors[j]));
                    }
                }

                for (int i = 0; i < candidateStrengthened.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateStrengthened.Count; j++)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, candidateStrengthened[i].strengthened as SegmentBisector,
                                                                        candidateStrengthened[j].strengthened as SegmentBisector,
                                                                        candidateStrengthened[i], candidateStrengthened[j]));
                    }
                }

                foreach (Strengthened oldStreng in candidateStrengthened)
                {
                    foreach (SegmentBisector oldSB in candidateBisectors)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, oldStreng.strengthened as SegmentBisector, oldSB, oldStreng, oldSB));
                    }
                }

                candidateQuadrilaterals.Add(quad);
            }
            else if (clause is SegmentBisector)
            {
                SegmentBisector newSB = clause as SegmentBisector;

                foreach (Quadrilateral quad in candidateQuadrilaterals)
                {
                    foreach (SegmentBisector oldSB in candidateBisectors)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, newSB, oldSB, newSB, oldSB));
                    }
                }

                foreach (Quadrilateral quad in candidateQuadrilaterals)
                {
                    foreach (Strengthened streng in candidateStrengthened)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, newSB, streng.strengthened as SegmentBisector, newSB, streng));
                    }
                }

                candidateBisectors.Add(newSB);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                if (!(newStreng.strengthened is SegmentBisector)) return newGrounded;

                foreach (Quadrilateral quad in candidateQuadrilaterals)
                {
                    foreach (SegmentBisector oldSB in candidateBisectors)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, newStreng.strengthened as SegmentBisector, oldSB, newStreng, oldSB));
                    }
                }

                foreach (Quadrilateral quad in candidateQuadrilaterals)
                {
                    foreach (Strengthened oldStreng in candidateStrengthened)
                    {
                        newGrounded.AddRange(InstantiateToTheorem(quad, newStreng.strengthened as SegmentBisector, oldStreng.strengthened as SegmentBisector, newStreng, oldStreng));
                    }
                }

                candidateStrengthened.Add(newStreng);
            }

            return newGrounded;
        }

        //     A _________________ B
        //      /                /
        //     /     \/         /
        //    /      /\        /
        // D /________________/ C
        //
        // Quadrilateral(A, B, C, D), SegmentBisector(Segment(A, C), Segment(B, D)),
        //                            SegmentBisector(Segment(B, D), Segment(A, C)) -> Parallelogram(A, B, C, D)
        //
        private static List<EdgeAggregator> InstantiateToTheorem(Quadrilateral quad, SegmentBisector sb1, SegmentBisector sb2, GroundedClause originalSB1, GroundedClause originalSB2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The two segment bisectors must intersect at the same point
            if (!sb1.bisected.intersect.StructurallyEquals(sb2.bisected.intersect)) return newGrounded;

            // The bisectors must be part of the other segment bisector.
            if (!sb1.bisected.HasSegment(sb2.bisector)) return newGrounded;
            if (!sb2.bisected.HasSegment(sb1.bisector)) return newGrounded;

            // Do these segment bisectors define the diagonals of the quadrilateral?
            if (!sb1.bisector.HasSubSegment(quad.topLeftBottomRightDiagonal) && !sb2.bisector.HasSubSegment(quad.topLeftBottomRightDiagonal)) return newGrounded;
            if (!sb1.bisector.HasSubSegment(quad.bottomLeftTopRightDiagonal) && !sb2.bisector.HasSubSegment(quad.bottomLeftTopRightDiagonal)) return newGrounded;

            // Determine the CongruentSegments opposing sides and output that.
            Strengthened newParallelogram = new Strengthened(quad, new Parallelogram(quad));
            
            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(originalSB1);
            antecedent.Add(originalSB2);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallelogram, annotation));

            return newGrounded;
        }
    }
}