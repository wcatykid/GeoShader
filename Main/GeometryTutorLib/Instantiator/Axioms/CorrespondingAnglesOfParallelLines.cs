using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CorrespondingAnglesOfParallelLines : Axiom
    {
        private static readonly string NAME = "Corresponding Angles"; // "If Two Parallel Lines are Cut by a Transversal, then Corresponding Angles are Congruent (Axiom)";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.CORRESPONDING_ANGLES_OF_PARALLEL_LINES);

        private static List<Parallel> candidateParallel = new List<Parallel>();
        private static List<Intersection> candidateIntersection = new List<Intersection>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateParallel.Clear();
            candidateIntersection.Clear();
        }

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.CORRESPONDING_ANGLES_OF_PARALLEL_LINES;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallel)
            {
                Parallel parallel = clause as Parallel;
                for (int i = 0; i < candidateIntersection.Count; i++)
                {
                    for (int j = i + 1; j < candidateIntersection.Count; j++)
                    {
                        newGrounded.AddRange(InstantiateIntersection(parallel, candidateIntersection[i], candidateIntersection[j]));
                    }
                }
                candidateParallel.Add(parallel);
            }
            else if (clause is Intersection)
            {
                Intersection newInter = clause as Intersection;
                foreach (Intersection oldInter in candidateIntersection)
                {
                    foreach (Parallel parallel in candidateParallel)
                    {
                        newGrounded.AddRange(InstantiateIntersection(parallel, newInter, oldInter));
                    }
                }
                candidateIntersection.Add(newInter);
            }

            return newGrounded;
        }


        private static List<EdgeAggregator> InstantiateIntersection(Parallel parallel, Intersection inter1, Intersection inter2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Avoid:
            //      |            |
            //    __|    ________|
            //      |            |
            //      |            |
            // Both intersections (transversal segments) must contain the actual transversal; that is, a direct, segment relationship must exist
            if (!inter1.CreatesAValidTransversalWith(inter2)) return newGrounded;

            // No corresponding angles if we have:
            //
            //    |          |         |
            //    |__________|         |_________
            //                                   |
            //                                   |
            //
            if (inter1.StandsOnEndpoint() && inter2.StandsOnEndpoint()) return newGrounded;

            // if (Utilities.DEBUG) System.Diagnostics.Debug.WriteLine("Working on: \n\t" + inter1.ToString() + "\n\t" + inter2.ToString());

            //
            // Verify we have a parallel / intersection situation using the given information
            //
            Segment transversal = inter1.AcquireTransversal(inter2);

            // Ensure the non-traversal segments align with the parallel segments
            Segment coincidingParallel1 = parallel.CoincidesWith(inter1.OtherSegment(transversal));
            Segment coincidingParallel2 = parallel.CoincidesWith(inter2.OtherSegment(transversal));

            // The pair of non-transversals needs to align exactly with the parallel pair of segments
            if (coincidingParallel1 == null || coincidingParallel2 == null) return newGrounded;

            // STANDARD Dual Crossings
            // Corresponding angles:
            //
            //      |          |  
            //   ___|__________|__
            //      |          |  
            //      |          | 
            //
            if (inter1.Crossing() && inter2.Crossing()) return InstantiateCompleteIntersection(parallel, inter1, inter2);

            // NOT Corresponding if an H-Shape
            //
            // |     |
            // |_____|
            // |     |
            // |     |
            //
            if (inter1.CreatesHShape(inter2)) return newGrounded;

            // NOT Corresponding angles if:
            //
            //         |______
            //         |
            //   ______|
            //         |
            //
            KeyValuePair<Point, Point> sShapePoints = inter1.CreatesStandardSShape(inter2);
            if (sShapePoints.Key != null && sShapePoints.Value != null) return newGrounded;

            // NOT Corresponding angles if:
            //
            //       |______
            //       |
            // ______|
            KeyValuePair<Point, Point> leanerShapePoints = inter1.CreatesLeanerShape(inter2);
            if (leanerShapePoints.Key != null && leanerShapePoints.Value != null) return newGrounded;

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

            sShapePoints = inter1.CreatesSimpleSShape(inter2);
            if (sShapePoints.Key != null && sShapePoints.Value != null) return newGrounded;

            // Corresponding angles if:
            //                o       e
            // standsOn (o)   o       e    standsOnEndpoint (e)
            //             eeeoeeeeeeee
            //                o
            //                o           
            //
            KeyValuePair<Point, Point> simpleTShapePoints = inter1.CreatesSimpleTShape(inter2);
            if (simpleTShapePoints.Key != null && simpleTShapePoints.Value != null)
            {
                return InstantiateSimpleTIntersection(parallel, inter1, inter2, simpleTShapePoints.Key, simpleTShapePoints.Value);
            }

            // Corresponding angles if:
            //    ____________
            //       |    |
            //       |    |
            //
            KeyValuePair<Point, Point> piShapePoints = inter1.CreatesSimplePIShape(inter2);
            if (piShapePoints.Key != null && piShapePoints.Value != null)
            {
                return InstantiateSimplePiIntersection(parallel, inter1, inter2, piShapePoints.Key, piShapePoints.Value);
            }

            // Corresponding if:
            //
            // |     |        |
            // |_____|____    |_________
            // |              |     |
            // |              |     |
            //
            KeyValuePair<Point, Point> chairShapePoints = inter1.CreatesChairShape(inter2);
            if (chairShapePoints.Key != null && chairShapePoints.Value != null)
            {
                return InstantiateChairIntersection(parallel, inter1, chairShapePoints.Key, inter2, chairShapePoints.Value);
            }

            // Corresponding angles if:
            //    ____________
            //       |    |
            //       |    |
            //
            piShapePoints = inter1.CreatesPIShape(inter2);
            if (piShapePoints.Key != null && piShapePoints.Value != null)
            {
                return InstantiatePiIntersection(parallel, inter1, piShapePoints.Key, inter2, piShapePoints.Value);
            }

            //
            //      |                |
            // _____|____      ______|______
            //      |                |
            //      |_____      _____|
            //
            KeyValuePair<Point, Point> crossedTShapePoints = inter1.CreatesCrossedTShape(inter2);
            if (crossedTShapePoints.Key != null && crossedTShapePoints.Value != null)
            {
                return InstantiateCrossedTIntersection(parallel, inter1, inter2, crossedTShapePoints.Key, crossedTShapePoints.Value);
            }

            // Corresponding if a flying-Shape
            //
            // |     |
            // |_____|___
            // |     |
            // |     |
            //
            KeyValuePair<Intersection, Point> flyingShapeValues = inter1.CreatesFlyingShape(inter2);
            if (flyingShapeValues.Key != null && flyingShapeValues.Value != null)
            {
                return InstantiateFlyingIntersection(parallel, inter1, inter2, flyingShapeValues.Key, flyingShapeValues.Value);
            }

            //        |
            //  ______|______
            //        |
            //   _____|_____
            Point offCross = inter1.CreatesFlyingShapeWithCrossing(inter2);
            if (offCross != null) return InstantiateFlyingCrossedIntersection(parallel, inter1, inter2, offCross);

            //        |
            //  ______|______
            //        |
            //        |_____
            //        |
            offCross = inter1.CreatesExtendedChairShape(inter2);
            if (offCross != null) return InstantiateExtendedChairIntersection(parallel, inter1, inter2, offCross);

            return newGrounded;
        }

        //                   top
        //                    o
        //  offStands  oooooooe
        //                    e
        //offEndpoint   eeeeeee
        //                    o
        //                 bottom
        //                       Returns: <offEndpoint, offStands>
        private static List<EdgeAggregator> InstantiateSimplePiIntersection(Parallel parallel, Intersection inter1, Intersection inter2, Point offEndpoint, Point offStands)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine which is the endpoint and stands intersections
            //
            Intersection endpointInter = null;
            Intersection standsInter = null;
            if (inter1.StandsOnEndpoint() && inter2.StandsOn())
            {
                endpointInter = inter1;
                standsInter = inter2;
            }
            else if (inter2.StandsOnEndpoint() && inter1.StandsOn())
            {
                endpointInter = inter2;
                standsInter = inter1;
            }
            else return newGrounded;

            //
            // Determine the top and bottom points
            //
            Segment transversal = inter1.AcquireTransversal(inter2);
            Segment transversalStands = standsInter.GetCollinearSegment(transversal);

            Point top = null;
            Point bottom = null;
            if (Segment.Between(standsInter.intersect, transversalStands.Point1, endpointInter.intersect))
            {
                top = transversalStands.Point1;
                bottom = transversalStands.Point2;
            }
            else
            {
                top = transversalStands.Point2;
                bottom = transversalStands.Point1;
            }

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(top, standsInter.intersect, offStands),
                                                                        new Angle(standsInter.intersect, endpointInter.intersect, offEndpoint));
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(bottom, endpointInter.intersect, offEndpoint),
                                               new Angle(endpointInter.intersect, standsInter.intersect, offStands));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        // Corresponding angles if:
        //            sameSide offRightEnd
        // standsOn (o)   o       e
        //                o       e    standsOnEndpoint (e)
        // offLeftEnd  eeeoeeeeeeee
        //                o
        //                o           
        //
        // Returns <offLeftEnd, offRightEnd>
        //
        private static List<EdgeAggregator> InstantiateSimpleTIntersection(Parallel parallel, Intersection inter1, Intersection inter2, Point offLeftEnd, Point offRightEnd)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine which is the endpoint and stands intersections
            //
            Intersection endpointInter = null;
            Intersection standsInter = null;
            if (inter1.StandsOnEndpoint() && inter2.StandsOn())
            {
                endpointInter = inter1;
                standsInter = inter2;
            }
            else if (inter2.StandsOnEndpoint() && inter1.StandsOn())
            {
                endpointInter = inter2;
                standsInter = inter1;
            }
            else return newGrounded;

            // Determine the sameSide point
            Segment transversal = inter1.AcquireTransversal(inter2);
            Segment parallelStands = standsInter.OtherSegment(transversal);
            Segment crossingTester = new Segment(offRightEnd, parallelStands.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);

            Point sameSide = transversal.PointLiesOnAndBetweenEndpoints(intersection) ? parallelStands.Point2 : parallelStands.Point1;

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offLeftEnd, standsInter.intersect, sameSide),
                                                                        new Angle(standsInter.intersect, endpointInter.intersect, offRightEnd));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }



        //     offCross
        //        |
        //  ______|______ rightCrossing
        //        |
        //        |_____  offStands
        //        |
        //        |
        //     bottomStands
        private static List<EdgeAggregator> InstantiateExtendedChairIntersection(Parallel parallel, Intersection inter1, Intersection inter2, Point offCross)
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

            //
            // Determination of Points
            //
            Point offStands = standsInter.CreatesTShape();

            Segment transversal = inter1.AcquireTransversal(inter2);
            Segment transversalStands = standsInter.GetCollinearSegment(transversal);

            Point bottomStands = Segment.Between(standsInter.intersect, transversalStands.Point1, crossingInter.intersect) ? transversalStands.Point1 : transversalStands.Point2;

            // Which side for rightCrossing
            Segment parallelCrossing = crossingInter.OtherSegment(transversal);
            Segment crossingTester = new Segment(offStands, parallelCrossing.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);

            Point rightCrossing = transversal.PointLiesOnAndBetweenEndpoints(intersection) ? parallelCrossing.Point2 : parallelCrossing.Point1;

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(bottomStands, standsInter.intersect, offStands),
                                                                        new Angle(standsInter.intersect, crossingInter.intersect, rightCrossing));
            newAngleRelations.Add(gca);

            gca = new GeometricCongruentAngles(new Angle(offStands, standsInter.intersect, crossingInter.intersect),
                                               new Angle(rightCrossing, crossingInter.intersect, offCross));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        //                 offCross
        //                     |
        // leftCross     ______|______   rightCross
        //                     |
        // leftStands     _____|_____    rightStands
        //
        //
        private static List<EdgeAggregator> InstantiateFlyingCrossedIntersection(Parallel parallel, Intersection inter1, Intersection inter2, Point offCross)
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

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offCross, crossingInter.intersect, rightCross),
                                                                        new Angle(crossingInter.intersect, standsInter.intersect, rightStands));
            newAngleRelations.Add(gca);

            gca = new GeometricCongruentAngles(new Angle(offCross, crossingInter.intersect, leftCross),
                                                                        new Angle(crossingInter.intersect, standsInter.intersect, leftStands));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        //
        // Creates a shape like an extended t
        //     offCross                          offCross  
        //      |                                   |
        // _____|____ sameSide       sameSide ______|______
        //      |                                   |
        //      |_____ offStands     offStands _____|
        //
        // Returns <offStands, offCross>
        private static List<EdgeAggregator> InstantiateCrossedTIntersection(Parallel parallel, Intersection inter1, Intersection inter2, Point offStands, Point offCross)
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

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offStands, standsInter.intersect, crossingInter.intersect),
                                                                        new Angle(sameSide, crossingInter.intersect, offCross));
            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }


        //
        // Chair Corresponding
        //
        // |     |                  |
        // |_____|____   leftInter  |_________ tipOfT
        // |                        |     |
        // |                        |     |
        //                         off   tipOfT
        //
        //                                bottomInter
        private static List<EdgeAggregator> InstantiateChairIntersection(Parallel parallel, Intersection inter1, Point off, Intersection inter2, Point bottomTip)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Segment transversal = inter1.AcquireTransversal(inter2);
            Point tipOfT1 = inter1.CreatesTShape();
            Point tipOfT2 = inter2.CreatesTShape();

            Point leftTip = null;
            Intersection leftInter = null;
            Intersection bottomInter = null;

            if (transversal.PointLiesOn(tipOfT1))
            {
                leftInter = inter1;
                bottomInter = inter2;
                leftTip = tipOfT1;
            }
            // thatInter is leftInter
            else if (transversal.PointLiesOn(tipOfT2))
            {
                leftInter = inter2;
                bottomInter = inter1;
                leftTip = tipOfT2;
            }

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            // CTA: Hack fix to alleviate exception thrown from improper congruent constructions.
            GeometricCongruentAngles gca = null;
            try
            {
                gca = new GeometricCongruentAngles(new Angle(leftTip, bottomInter.intersect, bottomTip),
                                                   new Angle(bottomInter.intersect, leftInter.intersect, off));
            }
            catch (Exception e)
            {
                if (Utilities.DEBUG) System.Diagnostics.Debug.WriteLine(e.ToString());

                return newGrounded;
            }

            newAngleRelations.Add(gca);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        //
        // Creates a shape like a crazy person flying
        //
        //            top   top
        //             |     |
        // larger      |_____|___ off
        //             |     |
        //             |     |
        //
        // Similar to H-shape with an extended point
        // Returns the 'larger' intersection that contains the point: off
        private static List<EdgeAggregator> InstantiateFlyingIntersection(Parallel parallel, Intersection inter1, Intersection inter2, Intersection larger, Point off)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Intersection smallerInter = inter1.Equals(larger) ? inter2 : inter1;

            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment parallel1 = inter1.OtherSegment(transversal);
            Segment parallel2 = inter2.OtherSegment(transversal);

            Point largerTop = parallel1.Point1;
            Point largerBottom = parallel1.Point2;

            Point otherTop = null;
            Point otherBottom = null;

            Segment crossingTester = new Segment(parallel1.Point1, parallel2.Point1);
            Point intersection = transversal.FindIntersection(crossingTester);
            // opposite sides
            if (transversal.PointLiesOnAndBetweenEndpoints(intersection))
            {
                otherTop = parallel2.Point2;
                otherBottom = parallel2.Point1;
            }
            // same sides
            else
            {
                otherTop = parallel2.Point1;
                otherBottom = parallel2.Point2;
            }

            //
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca1 = new GeometricCongruentAngles(new Angle(off, smallerInter.intersect, otherTop),
                                                                         new Angle(smallerInter.intersect, larger.intersect, largerTop));
            newAngleRelations.Add(gca1);
            GeometricCongruentAngles gca2 = new GeometricCongruentAngles(new Angle(off, smallerInter.intersect, otherBottom),
                                                                         new Angle(smallerInter.intersect, larger.intersect, largerBottom));
            newAngleRelations.Add(gca2);

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        // Corresponding angles if:
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

            Segment transversal = inter1.AcquireTransversal(inter2);

            Segment nonParallel1 = inter1.GetCollinearSegment(transversal);
            Segment nonParallel2 = inter2.GetCollinearSegment(transversal);

            Point left = Segment.Between(inter1.intersect, nonParallel1.Point1, inter2.intersect) ? nonParallel1.Point1 : nonParallel1.Point2;
            Point right = Segment.Between(inter2.intersect, inter1.intersect, nonParallel2.Point1) ? nonParallel2.Point1 : nonParallel2.Point2;

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            // CTA: Hack to avoid an exception being thrown during testing.
            try
            {
                GeometricCongruentAngles gca1 = new GeometricCongruentAngles(new Angle(left, inter1.intersect, off1),
                                                                             new Angle(inter1.intersect, inter2.intersect, off2));
                newAngleRelations.Add(gca1);

                GeometricCongruentAngles gca2 = new GeometricCongruentAngles(new Angle(right, inter2.intersect, off2),
                                                                             new Angle(inter2.intersect, inter1.intersect, off1));
                newAngleRelations.Add(gca2);
            }
            catch (Exception)
            {
                return newGrounded;
            }

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
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
            // Generate the new congruence
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            if (!down.Equals(stands.intersect))
            {
                GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(offEnd, endpt.intersect, stands.intersect),
                                                                            new Angle(offStands, stands.intersect, down));
                newAngleRelations.Add(gca);
            }

            if (!up.Equals(endpt.intersect))
            {
                GeometricCongruentAngles gcaOptional = new GeometricCongruentAngles(new Angle(up, endpt.intersect, offEnd),
                                                                                    new Angle(endpt.intersect, stands.intersect, offStands));
                newAngleRelations.Add(gcaOptional);
            }

            return MakeRelations(newAngleRelations, parallel, inter1, inter2);
        }

        // Corresponding angles if (we have 8 points here)
        //
        //  InterLeft                        InterRight
        //                |          |
        //      offLeft __|__________|__ offRight
        //                |          |
        //                |          |
        //
        private static List<EdgeAggregator> InstantiateCompleteIntersection(Parallel parallel, Intersection crossingInterLeft, Intersection crossingInterRight)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Segment transversal = crossingInterLeft.AcquireTransversal(crossingInterRight);

            //
            // Find off1 and off2
            //
            Segment crossingLeftParallel = crossingInterLeft.OtherSegment(transversal);
            Segment crossingRightParallel = crossingInterRight.OtherSegment(transversal);

            //
            // Determine which points are on the same side of the transversal.
            //
            Segment testingCrossSegment = new Segment(crossingLeftParallel.Point1, crossingRightParallel.Point1);
            Point intersection = transversal.FindIntersection(testingCrossSegment);

            Point crossingLeftTop = crossingLeftParallel.Point1;
            Point crossingLeftBottom = crossingLeftParallel.Point2;

            Point crossingRightTop = null;
            Point crossingRightBottom = null;
            if (transversal.PointLiesOnAndBetweenEndpoints(intersection))
            {
                crossingRightTop = crossingRightParallel.Point2;
                crossingRightBottom = crossingRightParallel.Point1;
            }
            else
            {
                crossingRightTop = crossingRightParallel.Point1;
                crossingRightBottom = crossingRightParallel.Point2;
            }

            // Point that is outside of the parallel lines and transversal
            Segment leftTransversal = crossingInterLeft.GetCollinearSegment(transversal);
            Segment rightTransversal = crossingInterRight.GetCollinearSegment(transversal);

            Point offCrossingLeft = Segment.Between(crossingInterLeft.intersect, leftTransversal.Point1, crossingInterRight.intersect) ? leftTransversal.Point1 : leftTransversal.Point2;
            Point offCrossingRight = Segment.Between(crossingInterRight.intersect, crossingInterLeft.intersect, rightTransversal.Point1) ? rightTransversal.Point1 : rightTransversal.Point2;

            //
            // Generate the new congruences
            //
            List<CongruentAngles> newAngleRelations = new List<CongruentAngles>();

            GeometricCongruentAngles gca = new GeometricCongruentAngles(new Angle(crossingLeftTop, crossingInterLeft.intersect, crossingInterRight.intersect),
                                                                        new Angle(crossingRightTop, crossingInterRight.intersect, offCrossingRight));
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(crossingLeftTop, crossingInterLeft.intersect, offCrossingLeft),
                                               new Angle(crossingRightTop, crossingInterRight.intersect, crossingInterLeft.intersect));
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(crossingLeftBottom, crossingInterLeft.intersect, offCrossingLeft),
                                               new Angle(crossingRightBottom, crossingInterRight.intersect, crossingInterLeft.intersect));
            newAngleRelations.Add(gca);
            gca = new GeometricCongruentAngles(new Angle(crossingLeftBottom, crossingInterLeft.intersect, crossingInterRight.intersect),
                                               new Angle(crossingRightBottom, crossingInterRight.intersect, offCrossingRight));
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
                newAngles.MakeAxiomatic();
                newGrounded.Add(new EdgeAggregator(antecedent, newAngles, annotation));
            }

            return newGrounded;
        }
    }
}