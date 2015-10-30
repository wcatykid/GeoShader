using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    //
    // Using a worklist technique, instantiate all nodes and construc the hypergraph on the fly.
    //
    public class Instantiator
    {
        // Contains all processed clauses and relationships amongst the clauses
        Hypergraph<GroundedClause, Hypergraph.EdgeAnnotation> graph;

        public Instantiator()
        {
            graph = new Hypergraph<GroundedClause, Hypergraph.EdgeAnnotation>();
        }

        //
        // Main instantiation function for all figures stated in the given list; worklist technique to construct the graph
        //
        public Hypergraph<GroundedClause, Hypergraph.EdgeAnnotation> Instantiate(List<ConcreteAST.GroundedClause> figure, List<ConcreteAST.GroundedClause> givens)
        {
            // The worklist initialized to initial set of ground clauses from the figure
            List<GroundedClause> worklist = new List<GroundedClause>(figure);
            worklist.AddRange(givens);

            // Add all initial elements to the graph
            figure.ForEach(g => { graph.AddNode(g); g.SetID(graph.Size()); } );
            givens.ForEach(g => { graph.AddNode(g); g.SetID(graph.Size()); } );

            // Indicate all figure-based information is intrinsic; this needs to verified with the UI
            figure.ForEach(f => f.MakeIntrinsic());

            // For problem generation, indicate that all given information is intrinsic
            givens.ForEach(g => g.MakeGiven());

            // Calculates Coverage of the figure
            //HandleAllFigureIntrinsicFacts(figure);

            HandleAllGivens(figure, givens);

            //
            // Process all new clauses until the worklist is empty
            //
            int numSequentialEquations = 0;
            while (worklist.Any())
            {
                // Acquire the first element from the list for processing
                GroundedClause clause = worklist[0];
                worklist.RemoveAt(0);

                if (Utilities.DEBUG) Debug.WriteLine("Working on: " + clause.clauseId + " " + clause.ToString());

                //
                // Cutoff counter; seek a bunch of equations in sequence.
                //
                int NUMEQUATIONS_FOR_CUTOFF = 50;

                if (clause is Equation || clause.IsAlgebraic() || clause is Supplementary) numSequentialEquations++;
                else numSequentialEquations = 0;

                if (numSequentialEquations >= NUMEQUATIONS_FOR_CUTOFF) return graph;

                if (graph.Size() > 800) return graph;

                //
                // Apply the clause to all applicable instantiators
                //
                if (clause is Point)
                {
                    Point.Record(clause);
                }
                else if (clause is Angle)
                {
                    // A list of all problem angles
                    Angle.Record(clause);

                    if (clause is RightAngle)
                    {
                        HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                    }
                    HandleDeducedClauses(worklist, ExteriorAngleEqualSumRemoteAngles.Instantiate(clause));
                    // HandleDeducedClauses(worklist, AngleAdditionAxiom.Instantiate(clause));

                    //HandleDeducedClauses(worklist, ConcreteAngle.Instantiate(null, clause));
                    //HandleDeducedClauses(worklist, AngleBisector.Instantiate(clause));

                    HandleDeducedClauses(worklist, PerpendicularImplyCongruentAdjacentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, AdjacentAnglesPerpendicularImplyComplementary.Instantiate(clause));

                    // Circle
                    HandleDeducedClauses(worklist, AngleInscribedSemiCircleIsRight.Instantiate(clause));
                    HandleDeducedClauses(worklist, InscribedAngleHalfInterceptedArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, CentralAngleEqualInterceptedArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, ChordTangentAngleHalfInterceptedArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, TwoInterceptedArcsHaveCongruentAngles.Instantiate(clause));
                }
                else if (clause is Arc)
                {
                    Arc.Record(clause);

                    if (clause is Semicircle)
                    {
                        HandleDeducedClauses(worklist, AngleInscribedSemiCircleIsRight.Instantiate(clause));
                    }
                }
                else if (clause is Segment)
                {
                    HandleDeducedClauses(worklist, Segment.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorDefinition.Instantiate(clause));
                    Segment.Record(clause);
                }
                else if (clause is InMiddle)
                {
                    HandleDeducedClauses(worklist, SegmentAdditionAxiom.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                }
                else if (clause is ArcInMiddle)
                {
                    // HandleDeducedClauses(worklist, ArcAdditionAxiom.Instantiate(clause));
                }
                else if (clause is Intersection)
                {
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));

                    if (clause is PerpendicularBisector)
                    {
                        HandleDeducedClauses(worklist, PerpendicularBisectorDefinition.Instantiate(clause));
                    }
                    else if (clause is Perpendicular)
                    {
                        HandleDeducedClauses(worklist, PerpendicularImplyCongruentAdjacentAngles.Instantiate(clause));
                        HandleDeducedClauses(worklist, AdjacentAnglesPerpendicularImplyComplementary.Instantiate(clause));
                        HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, PerpendicularToRadiusIsTangent.Instantiate(clause));
                    }
                    else
                    {
                        HandleDeducedClauses(worklist, VerticalAnglesTheorem.Instantiate(clause));
                        HandleDeducedClauses(worklist, AltIntCongruentAnglesImplyParallel.Instantiate(clause));
                        HandleDeducedClauses(worklist, SameSideSuppleAnglesImplyParallel.Instantiate(clause));
                        HandleDeducedClauses(worklist, TriangleProportionality.Instantiate(clause));
                        HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, CorrespondingAnglesOfParallelLines.Instantiate(clause));
                        HandleDeducedClauses(worklist, CongruentCorrespondingAnglesImplyParallel.Instantiate(clause));
                        HandleDeducedClauses(worklist, CongruentAdjacentAnglesImplyPerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                        HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, ParallelImplyAltIntCongruentAngles.Instantiate(clause));
                        HandleDeducedClauses(worklist, ParallelImplySameSideInteriorSupplementary.Instantiate(clause));
                        HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, SupplementaryDefinition.Instantiate(clause));

                        // Quad Theorems
                        HandleDeducedClauses(worklist, DiagonalsParallelogramBisectEachOther.Instantiate(clause));

                        // Circles
                        HandleDeducedClauses(worklist, ChordTangentAngleHalfInterceptedArc.Instantiate(clause));
                        HandleDeducedClauses(worklist, DiameterPerpendicularToChordBisectsChordAndArc.Instantiate(clause));
                        HandleDeducedClauses(worklist, ExteriorAngleHalfDifferenceInterceptedArcs.Instantiate(clause));
                        HandleDeducedClauses(worklist, TangentPerpendicularToRadius.Instantiate(clause));
                        HandleDeducedClauses(worklist, TangentsToCircleFromPointAreCongruent.Instantiate(clause));
                        HandleDeducedClauses(worklist, TwoChordsAnglesHalfSumInterceptedArc.Instantiate(clause));
                    }
                }
                else if (clause is Complementary)
                {
                    HandleDeducedClauses(worklist, RelationsOfCongruentAnglesAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                }
                else if (clause is Altitude)
                {
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));
                }
                else if (clause is AngleBisector)
                {
                    HandleDeducedClauses(worklist, AngleBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorTheorem.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                }
                else if (clause is Supplementary)
                {
                    HandleDeducedClauses(worklist, SameSideSuppleAnglesImplyParallel.Instantiate(clause));
                    HandleDeducedClauses(worklist, RelationsOfCongruentAnglesAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, SupplementaryAndCongruentImplyRightAngles.Instantiate(clause));
                }
                else if (clause is SegmentRatio)
                {
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                }
                else if (clause is Equation)
                {
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, SegmentRatio.InstantiateEquation(clause));
                    if (clause is AngleEquation)
                    {
                        HandleDeducedClauses(worklist, AnglesOfEqualMeasureAreCongruent.Instantiate(clause));
                        HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                    }
                    // If a geometric equation was constructed, it may not have been checked for proportionality
                    if ((clause as Equation).IsGeometric())
                    {
                        HandleDeducedClauses(worklist, ProportionalAngles.Instantiate(clause));
                        HandleDeducedClauses(worklist, SegmentRatio.InstantiateEquation(clause));
                    }
                }
                else if (clause is Midpoint)
                {
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointTheorem.Instantiate(clause));
                }
                else if (clause is Collinear)
                {
                    HandleDeducedClauses(worklist, StraightAngleDefinition.Instantiate(clause));
                }
                else if (clause is Median)
                {
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                }
                else if (clause is SegmentBisector)
                {
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsBisectEachOtherImplyParallelogram.Instantiate(clause));
                }
                else if (clause is Parallel)
                {
                    HandleDeducedClauses(worklist, TriangleProportionality.Instantiate(clause));
                    HandleDeducedClauses(worklist, CorrespondingAnglesOfParallelLines.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, ParallelImplyAltIntCongruentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, ParallelImplySameSideInteriorSupplementary.Instantiate(clause));

                    // Quadrilaterals
                    HandleDeducedClauses(worklist, ParallelogramDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, TrapezoidDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, OnePairOppSidesCongruentParallelImpliesParallelogram.Instantiate(clause));
                }
                else if (clause is SegmentRatioEquation)
                {
                    HandleDeducedClauses(worklist, SASSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSSSimilarity.Instantiate(clause));
                    //                    HandleDeducedClauses(worklist, SegmentRatio.InstantiateProportion(clause));
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                }
                else if (clause is ProportionalAngles)
                {
                    HandleDeducedClauses(worklist, ProportionalAngles.InstantiateProportion(clause));
                }
                else if (clause is CongruentTriangles)
                {
                    HandleDeducedClauses(worklist, CongruentTriangles.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransitiveCongruentTriangles.Instantiate(clause));
                }
                else if (clause is CongruentAngles)
                {
                    HandleDeducedClauses(worklist, SupplementaryAndCongruentImplyRightAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, TwoPairsCongruentAnglesImplyThirdPairCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AAS.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleAdditionAxiom.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentAnglesInTriangleImplyCongruentSides.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, AASimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, AltIntCongruentAnglesImplyParallel.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, CongruentCorrespondingAnglesImplyParallel.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentAdjacentAnglesImplyPerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, RelationsOfCongruentAnglesAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SupplementaryDefinition.Instantiate(clause));

                    // Quadrilaterals
                    HandleDeducedClauses(worklist, BothPairsOppAnglesCongruentImpliesParallelogram.Instantiate(clause));

                    // Circles
                    HandleDeducedClauses(worklist, MinorArcsCongruentIfCentralAnglesCongruent.Instantiate(clause));
                }
                else if (clause is CongruentSegments)
                {
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSS.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AAS.Instantiate(clause));
                    HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));
                    HandleDeducedClauses(worklist, EquilateralTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, CongruentSidesInTriangleImplyCongruentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentSegmentsImplySegmentRatioDefinition.Instantiate(clause));

                    // For quadrilaterals
                    HandleDeducedClauses(worklist, IsoscelesTrapezoidDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, KiteDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RhombusDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, BothPairsOppSidesCongruentImpliesParallelogram.Instantiate(clause));
                    HandleDeducedClauses(worklist, OnePairOppSidesCongruentParallelImpliesParallelogram.Instantiate(clause));

                    // Circles
                    HandleDeducedClauses(worklist, CongruentCircleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentArcsHaveCongruentChords.Instantiate(clause));
                }
                else if (clause is Triangle)
                {
                    Triangle.Record(clause);

                    //HandleDeducedClauses(worklist, SumAnglesInTriangle.Instantiate(clause));
                    HandleDeducedClauses(worklist, Angle.InstantiateReflexiveAngles(clause));
                    HandleDeducedClauses(worklist, Triangle.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSS.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, ASA.Instantiate(clause));
                    HandleDeducedClauses(worklist, AAS.Instantiate(clause));
                    HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));
                    HandleDeducedClauses(worklist, EquilateralTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, ExteriorAngleEqualSumRemoteAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentAnglesInTriangleImplyCongruentSides.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentSidesInTriangleImplyCongruentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, SSSSimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, AASimilarity.Instantiate(clause));
                    HandleDeducedClauses(worklist, TriangleProportionality.Instantiate(clause));
                    HandleDeducedClauses(worklist, AcuteAnglesInRightTriangleComplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, TwoPairsCongruentAnglesImplyThirdPairCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, CongruentSegmentsImplySegmentRatioDefinition.Instantiate(clause));

                    if (clause is IsoscelesTriangle)
                    {
                        HandleDeducedClauses(worklist, IsoscelesTriangleTheorem.Instantiate(clause));

                        // CTA: Needs to worl with Equilateral Triangles as well
                        HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                    }

                    if (clause is EquilateralTriangle)
                    {
                        HandleDeducedClauses(worklist, EquilateralTriangleHasSixtyDegreeAngles.Instantiate(clause));
                    }
                }
                else if (clause is Quadrilateral)
                {
                    Quadrilateral.Record(clause);

                    if (clause is Quadrilateral)
                    {
                        if ((clause as Quadrilateral).IsStrictQuadrilateral())
                        {
                            HandleDeducedClauses(worklist, ParallelogramDefinition.Instantiate(clause));
                            HandleDeducedClauses(worklist, RhombusDefinition.Instantiate(clause));
                            HandleDeducedClauses(worklist, SquareDefinition.Instantiate(clause));
                            HandleDeducedClauses(worklist, KiteDefinition.Instantiate(clause));
                            HandleDeducedClauses(worklist, TrapezoidDefinition.Instantiate(clause));

                            HandleDeducedClauses(worklist, BothPairsOppSidesCongruentImpliesParallelogram.Instantiate(clause));
                            HandleDeducedClauses(worklist, OnePairOppSidesCongruentParallelImpliesParallelogram.Instantiate(clause));
                            HandleDeducedClauses(worklist, BothPairsOppAnglesCongruentImpliesParallelogram.Instantiate(clause));
                            HandleDeducedClauses(worklist, DiagonalsBisectEachOtherImplyParallelogram.Instantiate(clause));
                        }
                    }

                    if (clause is Parallelogram)
                    {
                        HandleDeducedClauses(worklist, ParallelogramDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, RectangleDefinition.Instantiate(clause));

                        // Quad Theorems
                        HandleDeducedClauses(worklist, OppositeSidesOfParallelogramAreCongruent.Instantiate(clause));
                        HandleDeducedClauses(worklist, OppositeAnglesOfParallelogramAreCongruent.Instantiate(clause));
                        HandleDeducedClauses(worklist, DiagonalsParallelogramBisectEachOther.Instantiate(clause));

                    }

                    if (clause is Trapezoid)
                    {
                        HandleDeducedClauses(worklist, TrapezoidDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, IsoscelesTrapezoidDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, MedianTrapezoidParallelToBases.Instantiate(clause));
                        HandleDeducedClauses(worklist, MedianTrapezoidHalfSumBases.Instantiate(clause));
                    }

                    if (clause is IsoscelesTrapezoid)
                    {
                        HandleDeducedClauses(worklist, TrapezoidDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, IsoscelesTrapezoidDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, BaseAnglesIsoscelesTrapezoidCongruent.Instantiate(clause));
                    }

                    if (clause is Rhombus)
                    {
                        HandleDeducedClauses(worklist, RhombusDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, SquareDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, DiagonalsOfRhombusArePerpendicular.Instantiate(clause));
                        HandleDeducedClauses(worklist, DiagonalsOfRhombusBisectRhombusAngles.Instantiate(clause));
                    }

                    if (clause is Square)
                    {
                        HandleDeducedClauses(worklist, SquareDefinition.Instantiate(clause));
                    }

                    if (clause is Rectangle)
                    {
                        HandleDeducedClauses(worklist, RectangleDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, DiagonalsOfRectangleAreCongruent.Instantiate(clause));
                    }

                    if (clause is Kite)
                    {
                        HandleDeducedClauses(worklist, KiteDefinition.Instantiate(clause));
                        HandleDeducedClauses(worklist, DiagonalsOfKiteArePerpendicular.Instantiate(clause));
                    }
                }
                else if (clause is Strengthened)
                {
                    HandleDeducedClauses(worklist, IsoscelesTriangleTheorem.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, EquilateralTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, AcuteAnglesInRightTriangleComplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, AngleBisectorIsPerpendicularBisectorInIsosceles.Instantiate(clause));
                    HandleDeducedClauses(worklist, EquilateralTriangleHasSixtyDegreeAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, SASCongruence.Instantiate(clause));
                    HandleDeducedClauses(worklist, HypotenuseLeg.Instantiate(clause));

                    // For strengthening an intersection to a perpendicular
                    HandleDeducedClauses(worklist, PerpendicularImplyCongruentAdjacentAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, AdjacentAnglesPerpendicularImplyComplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, AltitudeDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, TransversalPerpendicularToParallelImplyBothPerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, PerpendicularDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, PerpendicularBisectorDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SegmentBisectorDefinition.Instantiate(clause));

                    // InMiddle Strengthened to Midpoint
                    HandleDeducedClauses(worklist, MidpointDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, MidpointTheorem.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianDefinition.Instantiate(clause));

                    // Right Angle
                    HandleDeducedClauses(worklist, RightAngleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, ComplementaryDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RightTriangleDefinition.Instantiate(clause));

                    // Correlating isoceles triangles with right triangles.
                    CoordinateRightIsoscelesTriangles.Instantiate(clause);

                    // For quadrilateral definitions
                    HandleDeducedClauses(worklist, TrapezoidDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, IsoscelesTrapezoidDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, SquareDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, KiteDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, ParallelogramDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RectangleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, RhombusDefinition.Instantiate(clause));

                    // Quad Theorems
                    HandleDeducedClauses(worklist, OppositeSidesOfParallelogramAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, OppositeAnglesOfParallelogramAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsParallelogramBisectEachOther.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsBisectEachOtherImplyParallelogram.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsOfRectangleAreCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsOfKiteArePerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsOfRhombusArePerpendicular.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiagonalsOfRhombusBisectRhombusAngles.Instantiate(clause));
                    HandleDeducedClauses(worklist, BaseAnglesIsoscelesTrapezoidCongruent.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianTrapezoidParallelToBases.Instantiate(clause));
                    HandleDeducedClauses(worklist, MedianTrapezoidHalfSumBases.Instantiate(clause));

                    // Circle
                    HandleDeducedClauses(worklist, AngleInscribedSemiCircleIsRight.Instantiate(clause));
                    HandleDeducedClauses(worklist, ChordTangentAngleHalfInterceptedArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, DiameterPerpendicularToChordBisectsChordAndArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, ExteriorAngleHalfDifferenceInterceptedArcs.Instantiate(clause));
                    HandleDeducedClauses(worklist, PerpendicularToRadiusIsTangent.Instantiate(clause));
                    HandleDeducedClauses(worklist, TangentPerpendicularToRadius.Instantiate(clause));
                    HandleDeducedClauses(worklist, TangentsToCircleFromPointAreCongruent.Instantiate(clause));
                }
                else if (clause is CircleIntersection)
                {
                    HandleDeducedClauses(worklist, DiameterPerpendicularToChordBisectsChordAndArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, PerpendicularToRadiusIsTangent.Instantiate(clause));
                }
                else if (clause is Tangent)
                {
                    // HandleDeducedClauses(worklist, ChordTangentAngleHalfInterceptedArc.Instantiate(clause));
                    // HandleDeducedClauses(worklist, ExteriorAngleHalfDifferenceInterceptedArcs.Instantiate(clause));
                    HandleDeducedClauses(worklist, TangentPerpendicularToRadius.Instantiate(clause));
                    HandleDeducedClauses(worklist, TangentsToCircleFromPointAreCongruent.Instantiate(clause));
                }
                else if (clause is CongruentArcs)
                {
                    HandleDeducedClauses(worklist, TransitiveSubstitution.Instantiate(clause)); // Simplifies as well
                    HandleDeducedClauses(worklist, CongruentArcsHaveCongruentChords.Instantiate(clause));
                    HandleDeducedClauses(worklist, MinorArcsCongruentIfCentralAnglesCongruent.Instantiate(clause));
                }
                else if (clause is Circle)
                {
                    Circle.Record(clause);

                    HandleDeducedClauses(worklist, CircleDefinition.Instantiate(clause));
                    HandleDeducedClauses(worklist, CentralAngleEqualInterceptedArc.Instantiate(clause));
                    //HandleDeducedClauses(worklist, ExteriorAngleHalfDifferenceInterceptedArcs.Instantiate(clause));
                    HandleDeducedClauses(worklist, InscribedAngleHalfInterceptedArc.Instantiate(clause));
                    //HandleDeducedClauses(worklist, TwoChordsAnglesHalfSumInterceptedArc.Instantiate(clause));
                    HandleDeducedClauses(worklist, InscribedQuadrilateralOppositeSupplementary.Instantiate(clause));
                    HandleDeducedClauses(worklist, TwoInterceptedArcsHaveCongruentAngles.Instantiate(clause));
                }
                else if (clause is CongruentCircles)
                {
                    HandleDeducedClauses(worklist, CongruentCircleDefinition.Instantiate(clause));
                }
            }

            return graph;
        }

        //
        // Add all new deduced clauses to the worklist if they have not been deduced before.
        // If the given clause has been deduced before, update the hyperedges that were generated previously
        //
        // Forward Instantiation does not permit any cycles in the resultant graph. We may deduce 
        //
        private void HandleDeducedClauses(List<GroundedClause> worklist, List<EdgeAggregator> newVals)
        {
            foreach (EdgeAggregator newEdge in newVals)
            {
                //Debug.WriteLine(newEdge.Value.clauseId + "(" + graph.Size() + ")" + ": " + newEdge.Value);


                GroundedClause graphNode = graph.GetNode(newEdge.consequent);

                //
                // If the node is not in the graph already?
                //
                if (graphNode == null)
                {
                    // Check to see if the new node is purely algebraic: A + A -> A
                    // If so, do not add the new node to the graph (nor add the edge).
                    if (!newEdge.consequent.IsPurelyAlgebraic())
                    {
                        // This node is not in the graph so add it; this should succeed
                        if (graph.AddNode(newEdge.consequent))
                        {
                            newEdge.consequent.SetID(graph.Size());
                        }

                        // Also add to the worklist
                        worklist.Add(newEdge.consequent);

                        AddForwardEdge(newEdge.antecedent, newEdge.consequent, newEdge.annotation); // 0: Annotation to be handled later
                    }
                }

                //
                // If the node is in the graph.
                //
                else
                {
                    AddForwardEdge(newEdge.antecedent, graphNode, newEdge.annotation);
                }
            }
        }

        private void AddForwardEdge(List<GroundedClause> antecedent, GroundedClause consequent, EdgeAnnotation annotation)
        {
            // Add the hyperedge to the hypergraph
            graph.AddEdge(antecedent, consequent, annotation);

            // Create the linkage between the antecedent and consequent for coverage
            foreach (GroundedClause ante in antecedent)
            {
                consequent.AddComponent(ante.clauseId);
                consequent.AddComponentList(ante.figureComponents);
            }
        }

        ////
        //// Constructs the coverage relationship amongst all figure components
        ////
        //private static void HandleAllFigureIntrinsicFacts(List<GroundedClause> figure)
        //{
        //    for (int f1 = 0; f1 < figure.Count; f1++)
        //    {
        //        for (int f2 = 0; f2 < figure.Count; f2++)
        //        {
        //            if (f1 != f2)
        //            {
        //                if (figure[f1].Covers(figure[f2])) figure[f1].AddComponent(figure[f2].clauseId);
        //            }
        //        }
        //    }
        //}

        //
        // Preprocess the given clauses
        //
        private static void HandleAllGivens(List<GroundedClause> figure, List<GroundedClause> givens)
        {
            ////
            //// Link the boolean facts to the intrinsic facts through coverage (implication)
            ////
            //foreach (GroundedClause given in givens)
            //{
            //    foreach (GroundedClause component in figure)
            //    {
            //        if (given.Covers(component))
            //        {
            //            given.AddComponent(component.clauseId);
            //            given.AddComponentList(component.figureComponents);
            //        }
            //    }
            //}

            //
            // Are any of the givens congruent relationships?
            //
            foreach (GroundedClause clause in givens)
            {
                if (clause is CongruentTriangles)
                {
                    CongruentTriangles conTris = clause as CongruentTriangles;
                    if (conTris.VerifyCongruentTriangles())
                    {
                        // indicate that these triangles are congruent to prevent future 'reproving' congruent
                        conTris.ProcessGivens();
                    }
                    // This is not a valid congruence.
                    else
                    {
                        // Remove?
                    }
                }
                //else if (clause is SimilarTriangles)
                //{
                //    SimilarTriangles simTris = clause as SimilarTriangles;
                //    if (simTris.VerifyCongruentTriangles())
                //    {
                //        // indicate that these triangles are congruent to prevent future 'reproving' congruent
                //        simTris.ProcessGivens();
                //    }
                //    // This is not a valid similarity.
                //    else
                //    {
                //        // Remove?
                //    }
                //}
            }
        }

        //
        // Clear all Instantiation Theorems / Axioms / Definitions
        //
        public static void Clear()
        {
            //
            // Algebra
            //
            RelationTransitiveSubstitution.Clear();
            TransitiveSubstitution.Clear();
            Point.Clear();
            Angle.Clear();
            Segment.Clear();
            Arc.Clear();
            Triangle.Clear();
            Quadrilateral.Clear();
            Circle.Clear();
            TransitiveCongruentTriangles.Clear();

            //
            // Axioms
            //
            AASimilarity.Clear();
            AngleAdditionAxiom.Clear();
            ASA.Clear();
            CongruentCorrespondingAnglesImplyParallel.Clear();
            CorrespondingAnglesOfParallelLines.Clear();
            SASCongruence.Clear();
            SSS.Clear();
            AnglesOfEqualMeasureAreCongruent.Clear();

            //
            // Definitions
            //
            AltitudeDefinition.Clear();
            AngleBisectorDefinition.Clear();
            ComplementaryDefinition.Clear();
            CongruentSegmentsImplySegmentRatioDefinition.Clear();
            CoordinateRightIsoscelesTriangles.Clear();
            EquilateralTriangleDefinition.Clear();
            IsoscelesTriangleDefinition.Clear();
            MedianDefinition.Clear();
            MidpointDefinition.Clear();
            PerpendicularDefinition.Clear();
            RightAngleDefinition.Clear();
            RightTriangleDefinition.Clear();
            SegmentBisectorDefinition.Clear();

            //
            // Theorems
            //
            AAS.Clear();
            AdjacentAnglesPerpendicularImplyComplementary.Clear();
            AltIntCongruentAnglesImplyParallel.Clear();
            AltitudeOfRightTrianglesImpliesSimilar.Clear();
            AngleBisectorIsPerpendicularBisectorInIsosceles.Clear();
            CongruentAdjacentAnglesImplyPerpendicular.Clear();
            CongruentAnglesInTriangleImplyCongruentSides.Clear();
            CongruentSidesInTriangleImplyCongruentAngles.Clear();
            ExteriorAngleEqualSumRemoteAngles.Clear();
            HypotenuseLeg.Clear();
            ParallelImplyAltIntCongruentAngles.Clear();
            ParallelImplySameSideInteriorSupplementary.Clear();
            PerpendicularImplyCongruentAdjacentAngles.Clear();
            RelationsOfCongruentAnglesAreCongruent.Clear();
            SameSideSuppleAnglesImplyParallel.Clear();
            SASSimilarity.Clear();
            SSSSimilarity.Clear();
            TransversalPerpendicularToParallelImplyBothPerpendicular.Clear();
            TriangleProportionality.Clear();
            TwoPairsCongruentAnglesImplyThirdPairCongruent.Clear();
            SupplementaryAndCongruentImplyRightAngles.Clear();

            // Quadrilaterals
            ParallelogramDefinition.Clear();
            KiteDefinition.Clear();
            RhombusDefinition.Clear();
            RectangleDefinition.Clear();
            SquareDefinition.Clear();
            TrapezoidDefinition.Clear();
            IsoscelesTrapezoidDefinition.Clear();

            // Quadrilateral Theorems
            BothPairsOppAnglesCongruentImpliesParallelogram.Clear();
            BothPairsOppSidesCongruentImpliesParallelogram.Clear();
            DiagonalsBisectEachOtherImplyParallelogram.Clear();
            OnePairOppSidesCongruentParallelImpliesParallelogram.Clear();

            // Circles
            AngleInscribedSemiCircleIsRight.Clear();
            CentralAngleEqualInterceptedArc.Clear();
            ChordTangentAngleHalfInterceptedArc.Clear();
            CongruentArcsHaveCongruentChords.Clear();
            DiameterPerpendicularToChordBisectsChordAndArc.Clear();
            ExteriorAngleHalfDifferenceInterceptedArcs.Clear();
            InscribedAngleHalfInterceptedArc.Clear();
            MinorArcsCongruentIfCentralAnglesCongruent.Clear();
            PerpendicularToRadiusIsTangent.Clear();
            TangentPerpendicularToRadius.Clear();
            TangentsToCircleFromPointAreCongruent.Clear();
            TwoChordsAnglesHalfSumInterceptedArc.Clear();
            //TwoInterceptedArcsHaveCongruentAngles.Clear();
        }
    }
}