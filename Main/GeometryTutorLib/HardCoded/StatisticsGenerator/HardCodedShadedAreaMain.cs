using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using StatisticsGenerator;

namespace GeometryTutorLib.TutorParser
{
    public class HardCodedShadedAreaMain
    {
        // The problem parameters to analyze
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> figure;
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> given;
        private GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator known;
        private List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> goalRegions;
        private GeometryTutorLib.TutorParser.ImpliedComponentCalculator implied;
        private List<GeometryTutorLib.ConcreteAST.Figure> roots;

        //
        // Deductive Hypergraph construction.
        //
        private GeometryTutorLib.GenericInstantiator.Instantiator instantiator;

        private GeometryTutorLib.Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, GeometryTutorLib.Hypergraph.EdgeAnnotation> deductiveGraph;
        private GeometryTutorLib.Pebbler.PebblerHypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, GeometryTutorLib.Hypergraph.EdgeAnnotation> pebblerGraph;
        private GeometryTutorLib.Pebbler.Pebbler pebbler;
        private GeometryTutorLib.Area_Based_Analyses.AreaSolutionGenerator solutionAreaGenerator;

        // The area dictated from the problem for validation purposes.
        private double area;

        //
        // Timer for statistical purposes.
        //
        private GeometryTutorLib.Stopwatch stopwatch;

        private TimeSpan deductionTiming;
        private TimeSpan solverTiming;

        public TimeSpan GetDeductionTiming() { return deductionTiming; }
        public TimeSpan GetSolverTiming() { return solverTiming; }
        

        public HardCodedShadedAreaMain(List<GeometryTutorLib.ConcreteAST.GroundedClause> fs,
                                       List<GeometryTutorLib.ConcreteAST.GroundedClause> giv,
                                       GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator kn,
                                       List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> goalRs,
                                       GeometryTutorLib.TutorParser.ImpliedComponentCalculator impl)
        {
            this.figure = fs;
            this.given = giv;
            this.known = kn;
            this.goalRegions = goalRs;
            this.area = -1;
            this.implied = impl;

            instantiator = new GeometryTutorLib.GenericInstantiator.Instantiator();
            stopwatch = new Stopwatch();
        }

        public HardCodedShadedAreaMain(List<GeometryTutorLib.ConcreteAST.GroundedClause> fs,
                                       List<GeometryTutorLib.ConcreteAST.GroundedClause> giv,
                                       GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator kn,
                                       List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> goalRs,
                                       GeometryTutorLib.TutorParser.ImpliedComponentCalculator impl,
                                       double area) : this(fs, giv, kn, goalRs, impl)
        {
            this.area = area;
        }

        // Returns: <number of interesting problems, number of original problems generated>
        public ShadedAreaFigureStatisticsAggregator AnalyzeFigure()
        {
            ShadedAreaFigureStatisticsAggregator figureStats = new ShadedAreaFigureStatisticsAggregator();

            // Set the number of atomic regions.
            figureStats.numAtomicRegions = this.implied.atomicRegions.Count;

            // Start overall timing
            figureStats.stopwatch.Start();

            // Start stopwatch.
            stopwatch.Start();

            // Handle givens that strengthen the intrinsic parts of the figure; modifies if needed
            given = DoGivensStrengthenFigure();

            // Use a worklist technique to instantiate nodes to construct the hypergraph for this figure
            ConstructHypergraph();

            // Track implicit and explicit facts.
            figureStats.totalImplicitFacts = figure.Count;
            figureStats.totalExplicitFacts = deductiveGraph.Size() - figure.Count;

            // Create the integer-based hypergraph representation
            ConstructPebblingHypergraph();

            // Pebble that hypergraph
            Pebble();

            //
            // Stop stopwatch and mark deduction timing.
            //
            stopwatch.Stop();
            deductionTiming = stopwatch.Elapsed;
            stopwatch.Reset();
            
            //
            // Restart stopwatch for solving.
            //
            stopwatch.Reset();
            stopwatch.Start();
          
            //
            // Acquire the list of strengthened (pebbled) polygon nodes.
            //
            List<int> pebbledIndices = pebblerGraph.GetPebbledNodes();
            List<GeometryTutorLib.ConcreteAST.Strengthened> strengthenedNodes = deductiveGraph.GetStrengthenedNodes(pebbledIndices);

            // Perform any calculations required for shaded-area solution synthesis: strengthening, hierarchy construction, etc.
            AreaBasedCalculator areaCal = new AreaBasedCalculator(implied, strengthenedNodes);
            areaCal.PrepareAreaBasedCalculations();
            figureStats.numShapes = areaCal.GetAllFigures().Count;

            // Save the roots of the hierarchy for interesting analysis.
            roots = areaCal.GetRootShapes();
            figureStats.numRootShapes = roots.Count;

            //
            // Based on pebbling, we have a specific set of reachable nodes in the hypergraph.
            // Determine all the known values in the figure based on the pebbled hypergraph and all the known values stated in the problem.
            //
            List<GeometryTutorLib.ConcreteAST.GroundedClause> reachableConEqs = FindReachableCongEquationNodes();
            List<GeometryTutorLib.ConcreteAST.GroundedClause> triangles = FindReachableTriangles();
            known = GeometryTutorLib.Area_Based_Analyses.KnownValueAcquisition.AcquireAllKnownValues(known, reachableConEqs, triangles);

            //
            // Find the set of all equations for the shapes in this figure.
            //
            solutionAreaGenerator = new GeometryTutorLib.Area_Based_Analyses.AreaSolutionGenerator(areaCal.GetShapeHierarchy(), areaCal.GetUpdatedAtomicRegions());
            solutionAreaGenerator.SolveAll(known, areaCal.GetAllFigures());

            //
            // Stop the stopwatch for solving.
            //
            stopwatch.Stop();
            this.solverTiming = stopwatch.Elapsed;

            // Acquire a single solution for this specific problem for validation purposes.
            KeyValuePair<GeometryTutorLib.Area_Based_Analyses.ComplexRegionEquation, double> result = solutionAreaGenerator.GetSolution(goalRegions);

            // Number of area facts
            figureStats.numAreaFacts = result.Key.expr.NumRegions();

#if HARD_CODED_UI
            UIDebugPublisher.getInstance().clearWindow();
            UIDebugPublisher.getInstance().publishString("Original Problem: " + string.Format("{0:N4}", result.Value) + " = " + result.Key.CheapPrettyString());
#else
            if (Utilities.SHADED_AREA_SOLVER_DEBUG)
            {
                Debug.WriteLine("Original Problem: " + string.Format("{0:N4}", result.Value) + " = " + result.Key.CheapPrettyString());
            }
#endif
            //
            // Validate that calculated area value matches the value from the hard-coded problem.
            //
            Validate(result.Key, result.Value);
            figureStats.originalProblemInteresting = OriginalProblemInteresting();

            //figureStats.numCalculableRegions = solutionAreaGenerator.GetNumComputable();
            //figureStats.numIncalculableRegions = solutionAreaGenerator.GetNumIncomputable();

            //Debug.WriteLine("Calculable Regions: " + figureStats.numCalculableRegions);
            //Debug.WriteLine("Incalculable Regions: " + figureStats.numIncalculableRegions);
            //Debug.WriteLine("Total:                " + (figureStats.numCalculableRegions +  figureStats.numIncalculableRegions));

            // Stop overall timing.
            figureStats.stopwatch.Stop();

            if (Utilities.SHADED_AREA_SOLVER_DEBUG)
            {
                solutionAreaGenerator.PrintAllSolutions();
            }

            return figureStats;
        }

        public bool CompleteComputableRegions()
        {
            return solutionAreaGenerator.GetNumComputable() == (int)Math.Pow(2, implied.atomicRegions.Count) - 1;
        }

        public bool CompleteAtomicRegions()
        {
            return solutionAreaGenerator.GetNumAtomicComputable() == implied.atomicRegions.Count;
        }

        //
        // Determine if the set of givens and knowns mean we calculate the area of all atomic regions?
        // Equivalently, can we calculate the area of all 2^n - 1 regions?
        //
        public bool IsComplete()
        {
            bool countBasedRegionCheck = CompleteComputableRegions();
            bool atomBasedCheck = CompleteAtomicRegions();

            if (countBasedRegionCheck != atomBasedCheck)
            {
                // throw new Exception("Complete calculations disagree: regions(" + countBasedRegionCheck + ") " + "atoms(" + atomBasedCheck + ")");
            }

            return countBasedRegionCheck || atomBasedCheck;
        }

        //
        // For each compuable region, does the region touch all root-shapes in the hierarchy?
        //
        public void GetComputableInterestingCount(out int interestingComp, out int uninterestingComp,
                                                  out int interestingIncomp, out int uninterestingIncomp)
        {
            interestingComp = 0;
            uninterestingComp = 0;
            interestingIncomp = 0;
            uninterestingIncomp = 0;

            // Check all solutions with the roots.
            solutionAreaGenerator.ComputableInterestingCount(roots, out interestingComp, out uninterestingComp, out interestingIncomp, out uninterestingIncomp);
        }

        //
        // For statistical analysis only count the number of occurrences of each intrisic property.
        //
        private void CountIntrisicProperties(StatisticsGenerator.FigureStatisticsAggregator figureStats)
        {
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in figure)
            {
                figureStats.totalImplicitFacts++;
                if (clause is GeometryTutorLib.ConcreteAST.Point) figureStats.numPoints++;
                else if (clause is GeometryTutorLib.ConcreteAST.InMiddle) figureStats.numInMiddle++;
                else if (clause is GeometryTutorLib.ConcreteAST.Segment) figureStats.numSegments++;
                else if (clause is GeometryTutorLib.ConcreteAST.Intersection) figureStats.numIntersections++;
                else if (clause is GeometryTutorLib.ConcreteAST.Triangle) figureStats.numTriangles++;
                else if (clause is GeometryTutorLib.ConcreteAST.Angle) figureStats.numAngles++;
                else if (clause is GeometryTutorLib.ConcreteAST.Quadrilateral) figureStats.numQuadrilaterals++;
                else if (clause is GeometryTutorLib.ConcreteAST.Circle) figureStats.numCircles++;
                else
                {
                    Debug.WriteLine("Did not count " + clause);
                    figureStats.totalImplicitFacts--;
                }
            }
        }

        //
        // Modify the given information to account for redundancy in stated nodes
        // That is, does given information strengthen a figure node?
        //
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> DoGivensStrengthenFigure()
        {
            List<GeometryTutorLib.ConcreteAST.GroundedClause> modifiedGivens = new List<GeometryTutorLib.ConcreteAST.GroundedClause>();
            GeometryTutorLib.ConcreteAST.GroundedClause currentGiven = null;

            foreach (GeometryTutorLib.ConcreteAST.GroundedClause give in given)
            {
                currentGiven = give;
                foreach (GeometryTutorLib.ConcreteAST.GroundedClause component in figure)
                {
                    if (component.CanBeStrengthenedTo(give))
                    {
                        currentGiven = new GeometryTutorLib.ConcreteAST.Strengthened(component, give);
                        break;
                    }
                }
                modifiedGivens.Add(currentGiven);
            }

            return modifiedGivens;
        }

        //
        // Construct the main Hypergraph
        //
        private void ConstructHypergraph()
        {
            // Resets all saved data to allow multiple problems
            GeometryTutorLib.GenericInstantiator.Instantiator.Clear();

            // Build the hypergraph through instantiation
            deductiveGraph = instantiator.Instantiate(figure, given);
        }

        //
        // Create the Pebbler version of the hypergraph for analysis (integer-based hypergraph)
        //
        private void ConstructPebblingHypergraph()
        {
            // Create the Pebbler version of the hypergraph (all integer representation) from the original hypergraph
            pebblerGraph = deductiveGraph.GetPebblerHypergraph();
        }

        //
        // Actually perform pebbling on the integer hypergraph (pebbles forward and backward)
        //
        private void Pebble()
        {
            pebbler = new GeometryTutorLib.Pebbler.Pebbler(deductiveGraph, pebblerGraph);

            // Acquire the integer values of the intrinsic / figure nodes
            List<int> intrinsicSet = GeometryTutorLib.Utilities.CollectGraphIndices(deductiveGraph, figure);

            // Acquire the integer values of the givens (from the original 
            List<int> givenSet = GeometryTutorLib.Utilities.CollectGraphIndices(deductiveGraph, given);

            // Perform pebbling based on the <figure, given> pair.
            pebbler.PebbleForwardForShading(intrinsicSet, givenSet);

            if (GeometryTutorLib.Utilities.PEBBLING_DEBUG)
            {
                pebbler.DebugDumpEdges();
            }
        }

        //
        // Based on pebbling, we have a specific set of reachable nodes in the hypergraph.
        //
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> FindReachableCongEquationNodes()
        {
            List<GeometryTutorLib.ConcreteAST.GroundedClause> nodes = new List<GeometryTutorLib.ConcreteAST.GroundedClause>();

            foreach (GeometryTutorLib.Pebbler.PebblerHyperNode<GeometryTutorLib.ConcreteAST.GroundedClause, GeometryTutorLib.Hypergraph.EdgeAnnotation> node in pebblerGraph.vertices)
            {
                if (node.pebbled)
                {
                    if (deductiveGraph.vertices[node.id].data is GeometryTutorLib.ConcreteAST.Congruent ||
                        deductiveGraph.vertices[node.id].data is GeometryTutorLib.ConcreteAST.Equation ||
                        deductiveGraph.vertices[node.id].data is GeometryTutorLib.ConcreteAST.SegmentRatioEquation)
                    {
                        nodes.Add(deductiveGraph.vertices[node.id].data);
                    }
                }
            }

            return nodes;
        }

        //
        // Based on pebbling, we have a specific set of reachable nodes in the hypergraph.
        //
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> FindReachableTriangles()
        {
            List<GeometryTutorLib.ConcreteAST.GroundedClause> nodes = new List<GeometryTutorLib.ConcreteAST.GroundedClause>();

            foreach (GeometryTutorLib.Pebbler.PebblerHyperNode<GeometryTutorLib.ConcreteAST.GroundedClause, GeometryTutorLib.Hypergraph.EdgeAnnotation> node in pebblerGraph.vertices)
            {
                if (node.pebbled)
                {
                    if (deductiveGraph.vertices[node.id].data is GeometryTutorLib.ConcreteAST.Triangle)
                    {
                        nodes.Add(deductiveGraph.vertices[node.id].data);
                    }
                    else if (deductiveGraph.vertices[node.id].data is GeometryTutorLib.ConcreteAST.Strengthened)
                    {
                        if ((deductiveGraph.vertices[node.id].data as GeometryTutorLib.ConcreteAST.Strengthened).original is GeometryTutorLib.ConcreteAST.Triangle)
                        {
                            nodes.Add(deductiveGraph.vertices[node.id].data);
                        }
                    }
                }
            }

            return nodes;
        }

        //
        // Validate that calculated area value matches the value from the hard-coded problem.
        //
        private void Validate(GeometryTutorLib.Area_Based_Analyses.ComplexRegionEquation equation, double calculatedArea)
        {
            if (this.area < 0)
            {
                Debug.WriteLine("Cannot validate calculated area since no area was provided.");
            }

            //if (!GeometryTutorLib.Utilities.CompareValues(this.area, calculatedArea))
            //{
            //    throw new Exception("Expected area (" + this.area + ") not found; found (" + calculatedArea + ")");
            //}
        }

        private bool OriginalProblemInteresting()
        {
            return solutionAreaGenerator.IsProblemInteresting(roots, goalRegions);
        }
    }
}
