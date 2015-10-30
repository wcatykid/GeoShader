using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class ParallelImplyAltIntCongruentAngles : Theorem
    {
        private readonly static string NAME = "Alternate Interior Angles"; //"Parallel Lines Imply Congruent Alternate Interior Angles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES);

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
        // Intersection(N, Segment(A,B), Segment(E, F))-> Congruent(Angle(F, N, M), Angle(N, M, C))
        //                                                Congruent(Angle(E, N, M), Angle(N, M, D))
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
            annotation.active = EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES;

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
                        newGrounded.AddRange(CheckAndGenerateParallelImplyAlternateInterior(candIntersection[i], candIntersection[j], parallel));
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
                        newGrounded.AddRange(CheckAndGenerateParallelImplyAlternateInterior(newIntersection, inter, parallel));
                    }
                }

                candIntersection.Add(newIntersection);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGenerateParallelImplyAlternateInterior(Intersection inter1, Intersection inter2, Parallel parallel)
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
                //   offThis   ______
                //                   |
                //   offThat   ______|
                //
                // Return <offThis, offThat>
                KeyValuePair<Point, Point> cShapePoints = inter1.CreatesBasicCShape(inter2);
                if (cShapePoints.Key != null && cShapePoints.Value != null) return newGrounded;

                //
                // Creates a basic S-Shape with standsOnEndpoints
                //
                //                  ______ offThat       _______
                //                 |                     \
                //   offThis ______|                 _____\
                //
                // Return <offThis, offThat>
                KeyValuePair<Point, Point> sShapePoints = inter1.CreatesBasicSShape(inter2);
                if (sShapePoints.Key != null && sShapePoints.Value != null)
                {
                    return GenerateSimpleS(parallel, inter1, sShapePoints.Key, inter2, sShapePoints.Value);
                }

                return newGrounded;
            }

            //     _______/________
            //           /
            //          /
            //   ______/_______
            //        /
            if (inter1.Crossing() && inter2.Crossing()) return GenerateDualCrossings(parallel, inter1, inter2);

            // No alt int in this case:
            // Creates an F-Shape
            //   top
            //    _____ offEnd     <--- Stands on Endpt
            //   |
            //   |_____ offStands  <--- Stands on 
            //   |
            //   | 
            //  bottom
            KeyValuePair<Point, Point> fShapePoints = inter1.CreatesFShape(inter2);
            if (fShapePoints.Key != null && fShapePoints.Value != null) return newGrounded;

            // Alt. Int if an H-Shape
            //
            // |     |
            // |_____|
            // |     |
            // |     |
            //
            if (inter1.CreatesHShape(inter2)) return GenerateH(parallel, inter1, inter2);

            // Creates an S-Shape
            //
            //         |______
            //         |
            //   ______|
            //         |
            //
            //   Order of non-collinear points is order of intersections: <this, that>
            KeyValuePair<Point, Point> standardSShapePoints = inter1.CreatesStandardSShape(inter2);
            if (standardSShapePoints.Key != null && standardSShapePoints.Value != null)
            {
                return GenerateSimpleS(parallel, inter1, standardSShapePoints.Key, inter2, standardSShapePoints.Value);
            }

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

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(off, bottom.intersect, top.intersect),
                                                                        new Angle(oppSide, top.intersect, bottom.intersect));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
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
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offStands, standsInter.intersect, crossingInter.intersect),
                                                                        new Angle(oppSide, crossingInter.intersect, standsInter.intersect));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }


        //
        // Creates a basic S-Shape with standsOnEndpoints
        //
        //                  ______ offThat
        //                 |
        //   offThis ______|
        //
        // Return <offThis, offThat>
        private static List<EdgeAggregator> GenerateSimpleS(Parallel parallel, Intersection inter1, Point offThis, Intersection inter2, Point offThat)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offThat, inter2.intersect, inter1.intersect),
                                                                        new Angle(offThis, inter1.intersect, inter2.intersect));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
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
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(rightCross, crossingInter.intersect, standsInter.intersect),
                                                                        new Angle(leftStands, standsInter.intersect, crossingInter.intersect));
            newAngleRelations.Add(gca);

            gca = new GeometricCongruentAngles(new Angle(leftCross, crossingInter.intersect, standsInter.intersect),
                                               new Angle(rightStands, standsInter.intersect, crossingInter.intersect));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
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
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(leftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                        new Angle(rightBottom, crossingInterRight.intersect, crossingInterLeft.intersect));
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(rightTop, crossingInterRight.intersect, crossingInterLeft.intersect),
                                               new Angle(leftBottom, crossingInterLeft.intersect, crossingInterRight.intersect));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, crossingInterLeft, crossingInterRight);
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
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(leftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                        new Angle(rightBottom, crossingInterRight.intersect, crossingInterLeft.intersect));
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(rightTop, crossingInterRight.intersect, crossingInterLeft.intersect),
                                               new Angle(leftBottom, crossingInterLeft.intersect, crossingInterRight.intersect));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, crossingInterLeft, crossingInterRight);
        }

        private static List<EdgeAggregator> MakeRelations(List<CongruentAngles> newAngleRelations, Parallel parallel, Intersection inter1, Intersection inter2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(parallel);
            antecedent.Add(inter1);
            antecedent.Add(inter2);

            foreach (CongruentAngles newAngles in newAngleRelations)
            {
                newGrounded.Add(new EdgeAggregator(antecedent, newAngles, annotation));
            }

            return newGrounded;
        }
    }
}