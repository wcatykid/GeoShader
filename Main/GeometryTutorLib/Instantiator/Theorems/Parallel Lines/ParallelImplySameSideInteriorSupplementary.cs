using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{
    public class ParallelImplySameSideInteriorSupplementary : Theorem
    {
        private readonly static string NAME = "Same-Side Supplementary"; // "If two parallel lines are cut by a transversal, then Same-Side Interior Angles are Supplementary.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY);

        public ParallelImplySameSideInteriorSupplementary() { }

        private static List<Intersection> candIntersection = new List<Intersection>();
        private static List<Parallel> candidateParallel = new List<Parallel>();

        // Resets all saved data.
        public static void Clear()
        {
            candIntersection.Clear();
            candidateParallel.Clear();
        }


        //
        // Parallel(Segment(C, D), Segment(E, F)),
        // Intersection(M, Segment(A,B), Segment(C, D)),
        // Intersection(N, Segment(A,B), Segment(E, F))-> Supplementary(Angle(F, N, M), Angle(N, M, D))
        //                                                Supplementary(Angle(E, N, M), Angle(N, M, C))
        //
        //                                            B
        //                                           /
        //                              C-----------/-----------D
        //                                         / M
        //                                        /
        //                             E---------/-----------F
        //                                      / N
        //                                     A
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (!(c is Parallel) && !(c is Intersection)) return newGrounded;

            if (c is Parallel)
            {
                Parallel parallel = c as Parallel;

                // Find two candidate lines cut by the same transversal
                for (int i = 0; i < candIntersection.Count - 1; i++)
                {
                    for (int j = i + 1; j < candIntersection.Count; j++)
                    {
                        newGrounded.AddRange(CheckAndGenerateParallelImplySameSide(candIntersection[i], candIntersection[j], parallel));
                    }
                }

                candidateParallel.Add(parallel);
            }
            else if (c is Intersection)
            {
                Intersection newIntersection = c as Intersection;

                // Find two candidate lines cut by the same transversal
                foreach (Intersection inter in candIntersection)
                {
                    foreach (Parallel parallel in candidateParallel)
                    {
                        newGrounded.AddRange(CheckAndGenerateParallelImplySameSide(newIntersection, inter, parallel));
                    }
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateParallelImplySameSide(Intersection inter1, Intersection inter2, Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The two intersections should not be at the same vertex
            if (inter1.intersect.Equals(inter2.intersect)) return newGrounded;

            // Determine the transversal
            Segment transversal = inter1.AcquireTransversal(inter2);
            if (transversal == null) return newGrounded;

            //
            // Ensure the non-traversal segments align with the parallel segments
            //
            Segment parallel1 = inter1.OtherSegment(transversal);
            Segment parallel2 = inter2.OtherSegment(transversal);

            // The non-transversals should not be the same (coinciding)
            if (parallel1.IsCollinearWith(parallel2)) return newGrounded;

            Segment coincidingParallel1 = parallel.CoincidesWith(parallel1);
            Segment coincidingParallel2 = parallel.CoincidesWith(parallel2);

            // The pair of non-transversals needs to align exactly with the parallel pair of segments
            if (coincidingParallel1 == null || coincidingParallel2 == null) return newGrounded;

            // Both intersections should not be referring to an intersection point on the same parallel segment
            if (parallel.segment1.PointLiesOn(inter1.intersect) && parallel.segment1.PointLiesOn(inter2.intersect)) return newGrounded;
            if (parallel.segment2.PointLiesOn(inter1.intersect) && parallel.segment2.PointLiesOn(inter2.intersect)) return newGrounded;

            // The resultant candidate parallel segments shouldn't share any vertices
            if (coincidingParallel1.SharedVertex(coincidingParallel2) != null) return newGrounded;

            if (inter1.StandsOnEndpoint() && inter2.StandsOnEndpoint())
            {
                //
                // Creates a basic S-Shape with standsOnEndpoints
                //
                //                  ______ offThat
                //                 |
                //   offThis ______|
                //
                // Return <offThis, offThat>
                KeyValuePair<Point, Point> sShapePoints = inter1.CreatesBasicSShape(inter2);
                if (sShapePoints.Key != null && sShapePoints.Value != null) return newGrounded;

                //
                // Creates a basic C-Shape with standsOnEndpoints
                //
                //   offThis   ______
                //                   |
                //   offThat   ______|
                //
                // Return <offThis, offThat>
                KeyValuePair<Point, Point> cShapePoints = inter1.CreatesBasicCShape(inter2);
                if (cShapePoints.Key != null && cShapePoints.Value != null)
                {
                    return GenerateSimpleC(parallel, inter1, cShapePoints.Key, inter2, cShapePoints.Value);
                }

                return newGrounded;
            }

            //     _______/________
            //           /
            //          /
            //   ______/_______
            //        /
            if (inter1.Crossing() && inter2.Crossing()) return GenerateDualCrossings(parallel, inter1, inter2);

            // Alt. Int if an H-Shape
            //
            // |     |
            // |_____|
            // |     |
            // |     |
            //
            if (inter1.CreatesHShape(inter2)) return GenerateH(parallel, inter1, inter2);

            // Corresponding if a flying-Shape
            //
            // |     |
            // |_____|___ offCross
            // |     |
            // |     |
            //
            Point offCross = inter1.CreatesFlyingShapeWithCrossing(inter2);
            if (offCross != null)
            {
                return GenerateFlying(parallel, inter1, inter2, offCross);
            }

            //
            // Creates a shape like an extended t
            //     offCross                          offCross  
            //      |                                   |
            // _____|____                         ______|______
            //      |                                   |
            //      |_____ offStands     offStands _____|
            //
            // Returns <offStands, offCross>
            KeyValuePair<Point, Point> tShapePoints = inter1.CreatesCrossedTShape(inter2);
            if (tShapePoints.Key != null && tShapePoints.Value != null)
            {
                return GenerateCrossedT(parallel, inter1, inter2, tShapePoints.Key, tShapePoints.Value);
            }

            //
            // Creates a Topped F-Shape
            //            top
            // offLeft __________ offEnd    <--- Stands on
            //             |
            //             |_____ off       <--- Stands on 
            //             |
            //             | 
            //           bottom
            //
            //   Returns: <bottom, off>
            KeyValuePair<Intersection, Point> toppedFShapePoints = inter1.CreatesToppedFShape(inter2);
            if (toppedFShapePoints.Key != null && toppedFShapePoints.Value != null)
            {
                return GenerateToppedFShape(parallel, inter1, inter2, toppedFShapePoints.Key, toppedFShapePoints.Value);
            }

            // Corresponding angles if:
            //    ____________
            //       |    |
            //       |    |
            //
            KeyValuePair<Point, Point> piShapePoints = inter1.CreatesPIShape(inter2);
            if (piShapePoints.Key != null && piShapePoints.Value != null)
            {
                return InstantiatePiIntersection(parallel, inter1, piShapePoints.Key, inter2, piShapePoints.Value);
            }

            // Corresponding angles if:
            //    _____
            //   |
            //   |_____ 
            //   |
            //   |
            //
            KeyValuePair<Point, Point> fShapePoints = inter1.CreatesFShape(inter2);
            if (fShapePoints.Key != null && fShapePoints.Value != null)
            {
                return InstantiateFIntersection(parallel, inter1, fShapePoints.Key, inter2, fShapePoints.Value);
            }

            return newGrounded;
        }

        //
        // Creates a Topped F-Shape
        //            top
        // oppSide __________       <--- Stands on
        //             |
        //             |_____ off   <--- Stands on 
        //             |
        //             | 
        //           bottom
        //
        //   Returns: <bottom, off>
        private static List<EdgeAggregator> GenerateToppedFShape(Parallel parallel, Intersection inter1, Intersection inter2, Intersection bottom, Point off)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Intersection top = inter1.Equals(bottom) ? inter2 : inter1;

            // Determine the side of the top intersection needed for the angle
            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment parallelTop = top.OtherSegment(transversal);
            Segment parallelBottom = bottom.OtherSegment(transversal);

            Segment crossingTester = new Segment(off, parallelTop.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);

            Point oppSide = transversal.PointLiesOnAndBetweenEndpoints(intersection) ? parallelTop.Point1 : parallelTop.Point2;
            Point sameSide = parallelTop.OtherPoint(oppSide);

            //
            // Generate the new congruence
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(off, bottom.intersect, top.intersect),
                                                   new Angle(sameSide, top.intersect, bottom.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, inter1, inter2);
        }


        // Corresponding angles if:
        //
        //   up  <- may not occur
        //   |_____  offEnd
        //   |
        //   |_____  offStands
        //   |
        //   |
        //  down 
        //
        private static List<EdgeAggregator> InstantiateFIntersection(Parallel parallel, Intersection inter1, Point off1, Intersection inter2, Point off2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Point offEnd = null;
            Point offStands = null;
            Intersection endpt = null;
            Intersection stands = null;
            if (inter1.StandsOnEndpoint())
            {
                endpt = inter1;
                offEnd = off1;
                stands = inter2;
                offStands = off2;
            }
            else if (inter2.StandsOnEndpoint())
            {
                endpt = inter2;
                offEnd = off2;
                stands = inter1;
                offStands = off1;
            }

            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment nonParallelEndpt = endpt.GetCollinearSegment(transversal);
            Segment nonParallelStands = stands.GetCollinearSegment(transversal);

            Point down = null;
            Point up = null;
            if (Segment.Between(stands.intersect, nonParallelStands.Point1, endpt.intersect))
            {
                down = nonParallelStands.Point1;
                up = nonParallelStands.Point2;
            }
            else
            {
                down = nonParallelStands.Point2;
                up = nonParallelStands.Point1;
            }

            //
            //   up  <- may not occur
            //   |_____  offEnd
            //   |
            //   |_____  offStands
            //   |
            //   |
            //  down 
            //

            //
            // Generate the new supplement relationship
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(offEnd, endpt.intersect, stands.intersect),
                                                   new Angle(offStands, stands.intersect, endpt.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, inter1, inter2);
        }

        // Same-Side supplementary angles if:
        //
        //   left _____________ right
        //           |     |
        //           |     |
        //          off1  off2
        //
        //     Inter 1       Inter 2
        //
        private static List<EdgeAggregator> InstantiatePiIntersection(Parallel parallel, Intersection inter1, Point off1, Intersection inter2, Point off2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //Segment transversal = inter1.AcquireTransversal(inter2);

            //Segment nonParallel1 = inter1.GetCollinearSegment(transversal);
            //Segment nonParallel2 = inter2.GetCollinearSegment(transversal);

            //Point left = Segment.Between(inter1.intersect, nonParallel1.Point1, inter2.intersect) ? nonParallel1.Point1 : nonParallel1.Point2;
            //Point right = Segment.Between(inter2.intersect, inter1.intersect, nonParallel2.Point1) ? nonParallel2.Point1 : nonParallel2.Point2;

            //
            // Generate the new supplement relationship
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            // CTA:
            // Hack to avoid exception during long tests.
            try
            {
                Supplementary supp = new Supplementary(new Angle(off1, inter1.intersect, inter2.intersect),
                                                       new Angle(off2, inter2.intersect, inter1.intersect));
                newSupps.Add(supp);

            } catch (Exception e)
            {
                if (Utilities.DEBUG) System.Diagnostics.Debug.WriteLine(e.ToString());

                return newGrounded;
            }

            return MakeHypergraphRelation(newSupps, parallel, inter1, inter2);
        }

        //
        // Creates a shape like an extended t
        //           offCross                           offCross  
        //              |                                   |
        // oppSide _____|____ sameSide       sameSide ______|______ oppSide
        //              |                                   |
        //              |_____ offStands     offStands _____|
        //
        // Returns <offStands, offCross>
        private static List<EdgeAggregator> GenerateCrossedT(Parallel parallel, Intersection inter1, Intersection inter2, Point offStands, Point offCross)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (inter1.Crossing() && inter2.StandsOnEndpoint())
            {
                crossingInter = inter1;
                standsInter = inter2;
            }
            else if (inter2.Crossing() && inter1.StandsOnEndpoint())
            {
                crossingInter = inter2;
                standsInter = inter1;
            }

            // Determine the side of the crossing needed for the angle
            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment parallelStands = standsInter.OtherSegment(transversal);
            Segment parallelCrossing = crossingInter.OtherSegment(transversal);

            Segment crossingTester = new Segment(offStands, parallelCrossing.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);

            Point sameSide = transversal.PointLiesOnAndBetweenEndpoints(intersection) ? parallelCrossing.Point2 : parallelCrossing.Point1;
            Point oppSide = parallelCrossing.OtherPoint(sameSide);

            //
            // Generate the new supplement relationship
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(offStands, standsInter.intersect, crossingInter.intersect),
                                                   new Angle(sameSide, crossingInter.intersect, standsInter.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, inter1, inter2);
        }


        //
        // Creates a basic C-Shape with standsOnEndpoints
        //
        //   offThis   ______
        //                   |
        //   offThat   ______|
        //
        // Return <offThis, offThat>
        private static List<EdgeAggregator> GenerateSimpleC(Parallel parallel, Intersection inter1, Point offThis, Intersection inter2, Point offThat)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Generate the new supplement relationship
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(offThis, inter1.intersect, inter2.intersect),
                                                   new Angle(offThat, inter2.intersect, inter1.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, inter1, inter2);
        }

        //                 offCross
        //                     |
        // leftCross     ______|______   rightCross
        //                     |
        // leftStands     _____|_____    rightStands
        //
        private static List<EdgeAggregator> GenerateFlying(Parallel parallel, Intersection inter1, Intersection inter2, Point offCross)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine which is the crossing intersection and which stands on the endpoints
            //
            Intersection crossingInter = null;
            Intersection standsInter = null;
            if (inter1.Crossing())
            {
                crossingInter = inter1;
                standsInter = inter2;
            }
            else if (inter2.Crossing())
            {
                crossingInter = inter2;
                standsInter = inter1;
            }

            // Determine the side of the crossing needed for the angle
            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment parallelStands = standsInter.OtherSegment(transversal);
            Segment parallelCrossing = crossingInter.OtherSegment(transversal);

            // Determine point orientation in the plane
            Point leftStands = parallelStands.Point1;
            Point rightStands = parallelStands.Point2;
            Point leftCross = null;
            Point rightCross = null;

            Segment crossingTester = new Segment(leftStands, parallelCrossing.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);
            if (transversal.PointLiesOnAndBetweenEndpoints(intersection))
            {
                leftCross = parallelCrossing.Point2;
                rightCross = parallelCrossing.Point1;
            }
            else
            {
                leftCross = parallelCrossing.Point1;
                rightCross = parallelCrossing.Point2;
            }

            //
            // Generate the new supplement relationship
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(leftStands, standsInter.intersect, crossingInter.intersect),
                                                   new Angle(leftCross, crossingInter.intersect, standsInter.intersect));
            newSupps.Add(supp);

            supp = new Supplementary(new Angle(rightStands, standsInter.intersect, crossingInter.intersect),
                                     new Angle(rightCross, crossingInter.intersect, standsInter.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, inter1, inter2);
        }

        //     leftTop    rightTop
        //         |         |
        //         |_________|
        //         |         |
        //         |         |
        //   leftBottom rightBottom
        //
        private static List<EdgeAggregator> GenerateH(Parallel parallel, Intersection crossingInterLeft, Intersection crossingInterRight)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Segment transversal = crossingInterLeft.AcquireTransversal(crossingInterRight);

            //
            // Find tops and bottoms
            //
            Segment crossingLeftParallel = crossingInterLeft.OtherSegment(transversal);
            Segment crossingRightParallel = crossingInterRight.OtherSegment(transversal);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(crossingLeftParallel.Point1, crossingRightParallel.Point1);
            Point intersection = transversal.FindIntersection(testingCrossSegment);

            Point leftTop = crossingLeftParallel.Point1;
            Point leftBottom = crossingLeftParallel.Point2;

            Point rightTop = null;
            Point rightBottom = null;
            if (transversal.PointLiesOnAndBetweenEndpoints(intersection))
            {
                rightTop = crossingRightParallel.Point2;
                rightBottom = crossingRightParallel.Point1;
            }
            else
            {
                rightTop = crossingRightParallel.Point1;
                rightBottom = crossingRightParallel.Point2;
            }

            //
            // Generate the new supplement relationship
            //
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(leftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                   new Angle(rightTop, crossingInterRight.intersect, crossingInterLeft.intersect));
            newSupps.Add(supp);

            supp = new Supplementary(new Angle(leftBottom, crossingInterLeft.intersect, crossingInterRight.intersect),
                                     new Angle(rightBottom, crossingInterRight.intersect, crossingInterLeft.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, crossingInterLeft, crossingInterRight);
        }

        //          leftTop    rightTop
        //              |         |
        //  offleft ____|_________|_____ offRight
        //              |         |
        //              |         |
        //         leftBottom rightBottom
        //
        private static List<EdgeAggregator> GenerateDualCrossings(Parallel parallel, Intersection crossingInterLeft, Intersection crossingInterRight)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Segment transversal = crossingInterLeft.AcquireTransversal(crossingInterRight);

            //
            // Find offLeft and offRight
            //
            Segment crossingLeftParallel = crossingInterLeft.OtherSegment(transversal);
            Segment crossingRightParallel = crossingInterRight.OtherSegment(transversal);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(crossingLeftParallel.Point1, crossingRightParallel.Point1);
            Point intersection = transversal.FindIntersection(testingCrossSegment);

            Point leftTop = crossingLeftParallel.Point1;
            Point leftBottom = crossingLeftParallel.Point2;

            Point rightTop = null;
            Point rightBottom = null;
            if (transversal.PointLiesOnAndBetweenEndpoints(intersection))
            {
                rightTop = crossingRightParallel.Point2;
                rightBottom = crossingRightParallel.Point1;
            }
            else
            {
                rightTop = crossingRightParallel.Point1;
                rightBottom = crossingRightParallel.Point2;
            }

            // Point that is outside of the parallel lines and transversal
            Segment leftTransversal = crossingInterLeft.GetCollinearSegment(transversal);
            Segment rightTransversal = crossingInterRight.GetCollinearSegment(transversal);

            Point offLeft = Segment.Between(crossingInterLeft.intersect, leftTransversal.Point1, crossingInterRight.intersect) ? leftTransversal.Point1 : leftTransversal.Point2;
            Point offRight = Segment.Between(crossingInterRight.intersect, crossingInterLeft.intersect, rightTransversal.Point1) ? rightTransversal.Point1 : rightTransversal.Point2;

            //
            // Generate the new congruences
            //

            //          leftTop    rightTop
            //              |         |
            //  offleft ____|_________|_____ offRight
            //              |         |
            //              |         |
            //         leftBottom rightBottom
            List<Supplementary> newSupps = new List<Supplementary>();

            Supplementary supp = new Supplementary(new Angle(leftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                   new Angle(rightTop, crossingInterRight.intersect, crossingInterLeft.intersect));
            newSupps.Add(supp);

            supp = new Supplementary(new Angle(leftBottom, crossingInterLeft.intersect, crossingInterRight.intersect),
                                     new Angle(rightBottom, crossingInterRight.intersect, crossingInterLeft.intersect));
            newSupps.Add(supp);

            return MakeHypergraphRelation(newSupps, parallel, crossingInterLeft, crossingInterRight);
        }

        private static List<EdgeAggregator> MakeHypergraphRelation(List<Supplementary> newSupps, Parallel parallel, Intersection inter1, Intersection inter2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel);
            antecedent.Add(inter1);
            antecedent.Add(inter2);

            foreach (Supplementary supp in newSupps)
            {
                newGrounded.Add(new EdgeAggregator(antecedent, supp, annotation));
            }

            return newGrounded;
        }
    }
}