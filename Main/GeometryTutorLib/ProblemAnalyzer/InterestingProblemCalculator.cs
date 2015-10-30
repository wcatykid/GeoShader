using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.Hypergraph;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class InterestingProblemCalculator
    {
        private readonly bool INTERESTING_DEBUG = false;

        private Hypergraph<GroundedClause, Hypergraph.EdgeAnnotation> graph;
        private List<GroundedClause> figure;
        private List<GroundedClause> givens;
        private List<int> givenIndices;
        private List<GroundedClause> goals;
        private List<int> goalIndices;

        public InterestingProblemCalculator(Hypergraph<GroundedClause, Hypergraph.EdgeAnnotation> g, List<GroundedClause> f, List<GroundedClause> givens, List<GroundedClause> goals)
        {
            this.graph = g;
            this.figure = f;
            this.givens = givens;
            this.goals = goals;

            // Calculate the givens indices in the hypergraph for this problem
            this.givenIndices = new List<int>();
            foreach (GroundedClause given in givens)
            {
                int givenIndex = Utilities.StructuralIndex(graph, given);
                if (givenIndex == -1) throw new Exception("GIVEN not found in the graph: " + given);
                givenIndices.Add(givenIndex);
            }

            // Calculate the goal indices in the hypergraph for this problem
            this.goalIndices = new List<int>();
            foreach (GroundedClause goal in goals)
            {
                int goalIndex = Utilities.StructuralIndex(graph, goal);
                if (goalIndex == -1) throw new Exception("GOAL not found in the graph: " + goal);
                goalIndices.Add(goalIndex);
            }

            //
            // For intrinsic property-based coverage
            //
            COVERAGE_WEIGHTS = new double[NUM_INTRINSIC];
            // Sum the factors
            double sum = 0;
            foreach (int i in COVERAGE_FACTOR) sum += i;
            // Divide for weights
            for (int w = 0; w < NUM_INTRINSIC; w++)
            {
                COVERAGE_WEIGHTS[w] = COVERAGE_FACTOR[w] / sum;
            }
        }

        public InterestingProblemCalculator(Hypergraph<GroundedClause, Hypergraph.EdgeAnnotation> g, List<GroundedClause> f, List<GroundedClause> givens)
        {
            this.graph = g;
            this.figure = f;
            this.givens = givens;
            this.goals = new List<GroundedClause>();

            // Calculate the givens indices in the hypergraph for this problem
            this.givenIndices = new List<int>();
            foreach (GroundedClause given in givens)
            {
                int givenIndex = Utilities.StructuralIndex(graph, given);
                if (givenIndex == -1) throw new Exception("GIVEN not found in the graph: " + given);
                givenIndices.Add(givenIndex);
            }

            // There are no goals specified
            this.goalIndices = new List<int>();
            //
            // For intrinsic property-based coverage
            //
            COVERAGE_WEIGHTS = new double[NUM_INTRINSIC];
            // Sum the factors
            double sum = 0;
            foreach (int i in COVERAGE_FACTOR) sum += i;
            // Divide for weights
            for (int w = 0; w < NUM_INTRINSIC; w++)
            {
                COVERAGE_WEIGHTS[w] = COVERAGE_FACTOR[w] / sum;
            }
        }

        //
        // Given a set of problems, determine which partition of problems meets the 'interesting' criteria
        //
        public List<Problem<Hypergraph.EdgeAnnotation>> DetermineInterestingProblems(List<Problem<Hypergraph.EdgeAnnotation>> problems)
        {
            List<Problem<Hypergraph.EdgeAnnotation>> interesting = new List<Problem<Hypergraph.EdgeAnnotation>>();

            foreach (Problem<Hypergraph.EdgeAnnotation> p in problems)
            {
                if (IsInterestingWithGivens(p))
                {
                    interesting.Add(p);
                }
            }

            return interesting;
        }

        private bool IsInterestingWithGivens(Problem<Hypergraph.EdgeAnnotation> problem)
        {
            // If there are no givens, but there are goals deduced we say it is interesting
            if (!givenIndices.Any()) return true;

            // If the givens are empty for this problem, but it still deduces the goal, it is interesting
            if (!problem.givens.Any() && goalIndices.Contains(problem.goal)) return true;

            // Consider number of givens
            if (problem.givens.Count > 6 /* queryVector.numberOfOriginalGivens */) return false;

            int numGivensInProblem = 0;
            foreach (int givenIndex in givenIndices)
            {
                if (problem.givens.Contains(givenIndex))
                {
                    numGivensInProblem++;
                }
            }

            problem.interestingPercentage = (int)((double)(numGivensInProblem) / givenIndices.Count * 100);

            bool problemContainsOtherGivens = problem.givens.Count - numGivensInProblem > 0;
            bool interesting = !problemContainsOtherGivens && numGivensInProblem > 0;

            return interesting;
        }

        //
        // Given a set of problems, determine which partition of problems meets the 'interesting' criteria: 100% of givens covered.
        //
        public List<MultiGoalProblem<Hypergraph.EdgeAnnotation>> DetermineStrictlyInterestingMultiGoalProblems(List<MultiGoalProblem<Hypergraph.EdgeAnnotation>> problems)
        {
            List<MultiGoalProblem<Hypergraph.EdgeAnnotation>> strictlyInteresting = new List<MultiGoalProblem<Hypergraph.EdgeAnnotation>>();

            foreach (MultiGoalProblem<Hypergraph.EdgeAnnotation> mgProblem in problems)
            {
                if (Utilities.EqualSets<int>(mgProblem.givens, this.givenIndices))
                {
                    strictlyInteresting.Add(mgProblem);
                }

            }

            return strictlyInteresting;
        }


        //
        // For intrinsic property-based coverage
        //
        private readonly int POINTS = 0;
        private readonly int SEGMENTS = 1;
        private readonly int ANGLES = 2;
        private readonly int INTERSECTION = 3;
        private readonly int TRIANGLES = 4;
        private readonly int IN_MIDDLES = 5;
        private readonly int NUM_INTRINSIC = 6;

        private readonly int[] COVERAGE_FACTOR = { 1, // Points
                                                   2, // Segments
                                                   3, // Angles
                                                   2, // Intersection
                                                   6, // Triangles
                                                   3, // InMiddle
                                                 };

        private double[] COVERAGE_WEIGHTS; // Init in constructor based on the coverage factors stated in the array above
        //private readonly double MINIMUM_WEIGHTED_COVERAGE_FACTOR = 1; // Weighted %: [0, 1]

        //
        // A problem is defined as interesting if:
        //   1. It is minimal in its given information
        //   2. The problem implies all of the facts of the given figure; that is, if the set of all the facts of a figure are not in the source of the problem, then reject 
        //
        private bool IsInteresting(Problem<Hypergraph.EdgeAnnotation> problem)
        {
            if (problem.givens.Count > 4) return false;

            if (ProblemCoversAllFigureGivens(problem)) return true;

            return false;

            // Acquire the percentage of the individual components in the figure that are covered
            //double[] problemCoverage = InterestingProblemCoverage(problem);

            //// Calculate the actual coverage factor
            //double finalCoverageFactor = 0;
            //for (int w = 0; w < NUM_INTRINSIC; w++)
            //{
            //    finalCoverageFactor += COVERAGE_WEIGHTS[w] * problemCoverage[w];
            //}

            //if (INTERESTING_DEBUG)
            //{
            //    System.Diagnostics.Debug.WriteLine("Weighted Coverage Factor: " + finalCoverageFactor);
            //}
            //return finalCoverageFactor > MINIMUM_WEIGHTED_COVERAGE_FACTOR ||
            //       Utilities.CompareValues(finalCoverageFactor, MINIMUM_WEIGHTED_COVERAGE_FACTOR);
        }

        //
        // Does a problem cover 100% of the given information
        //
        private bool ProblemCoversAllFigureGivens(Problem<Hypergraph.EdgeAnnotation> problem)
        {
            // Look at the target node of the problem.
            // Since the target is dependent on all preceding nodes, it will contain all of their covered nodes.
            // We require that this node cover ALL the given information for the figure.
            foreach (GroundedClause given in givens)
            {
                if (!graph.vertices[problem.goal].data.figureComponents.Contains(given.clauseId))
                {
                    //System.Diagnostics.Debug.WriteLine("Uncovered: " + given.ToString());
                    return false;
                }

                //System.Diagnostics.Debug.WriteLine("Covered: " + given.ToString());
            }

            return true;
        }

        //
        // A problem is defined as interesting if:
        //   1. It is minimal in its given information
        //   2. The problem implies all of the facts of the given figure; that is, if the set of all the facts of a figure are not in the source of the problem, then reject 
        //
        // Returns a 
        private double[] InterestingProblemCoverage(Problem<Hypergraph.EdgeAnnotation> problem)
        {
            List<int> problemGivens = problem.givens;

            //
            // Collect all of the figure intrinsic covered nodes
            //
            List<int> intrinsicCollection = new List<int>();
            foreach (int src in problem.givens)
            {
                Utilities.AddUniqueList<int>(intrinsicCollection, graph.vertices[src].data.figureComponents);
            }

            // Sort is not required, but for debug is easier to digest
            intrinsicCollection.Sort();

            // DEBUG
            //System.Diagnostics.Debug.WriteLine("\n" + problem + "\nCovered Nodes: ");
            //foreach (int coveredNode in intrinsicCollection)
            //{
            //    System.Diagnostics.Debug.WriteLine("\t" + coveredNode);
            //}

            //
            // Calculate the 
            //
            int[] numCoveredNodes = new int[NUM_INTRINSIC];
            int[] numUncoveredNodes = new int[NUM_INTRINSIC];
            int totalCovered = 0;
            int totalUncovered = 0;
            foreach (GroundedClause gc in figure)
            {
//                System.Diagnostics.Debug.WriteLine("Checking: " + gc.ToString());
                if (intrinsicCollection.Contains(gc.clauseId))
                {
                    if (gc is Point) numCoveredNodes[POINTS]++;
                    else if (gc is Segment) numCoveredNodes[SEGMENTS]++;
                    else if (gc is Angle) numCoveredNodes[ANGLES]++;
                    else if (gc is Intersection) numCoveredNodes[INTERSECTION]++;
                    else if (gc is Triangle) numCoveredNodes[TRIANGLES]++;
                    else if (gc is InMiddle) numCoveredNodes[IN_MIDDLES]++;
                    totalCovered++;
                }
                else
                {
                    if (INTERESTING_DEBUG)
                    {
                        System.Diagnostics.Debug.WriteLine("Uncovered: " + gc.ToString());
                    }
                    if (gc is Point) numUncoveredNodes[POINTS]++;
                    else if (gc is Segment) numUncoveredNodes[SEGMENTS]++;
                    else if (gc is Angle) numUncoveredNodes[ANGLES]++;
                    else if (gc is Intersection) numUncoveredNodes[INTERSECTION]++;
                    else if (gc is Triangle) numUncoveredNodes[TRIANGLES]++;
                    else if (gc is InMiddle) numUncoveredNodes[IN_MIDDLES]++;
                    totalUncovered++;
                }
            }

            if (INTERESTING_DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Covered: ");
                System.Diagnostics.Debug.WriteLine("\tPoints\t\t\t" + numCoveredNodes[POINTS]);
                System.Diagnostics.Debug.WriteLine("\tSegments\t\t" + numCoveredNodes[SEGMENTS]);
                System.Diagnostics.Debug.WriteLine("\tAngles\t\t\t" + numCoveredNodes[ANGLES]);
                System.Diagnostics.Debug.WriteLine("\tIntersection\t" + numCoveredNodes[INTERSECTION]);
                System.Diagnostics.Debug.WriteLine("\tTriangles\t\t" + numCoveredNodes[TRIANGLES]);
                System.Diagnostics.Debug.WriteLine("\tInMiddles\t\t" + numCoveredNodes[IN_MIDDLES]);
                System.Diagnostics.Debug.WriteLine("\t\t\t\t\t" + totalCovered);

                System.Diagnostics.Debug.WriteLine("Uncovered: ");
                System.Diagnostics.Debug.WriteLine("\tPoints\t\t\t" + numUncoveredNodes[POINTS]);
                System.Diagnostics.Debug.WriteLine("\tSegments\t\t" + numUncoveredNodes[SEGMENTS]);
                System.Diagnostics.Debug.WriteLine("\tAngles\t\t\t" + numUncoveredNodes[ANGLES]);
                System.Diagnostics.Debug.WriteLine("\tIntersection\t" + numUncoveredNodes[INTERSECTION]);
                System.Diagnostics.Debug.WriteLine("\tTriangles\t\t" + numUncoveredNodes[TRIANGLES]);
                System.Diagnostics.Debug.WriteLine("\tInMiddles\t\t" + numUncoveredNodes[IN_MIDDLES]);
                System.Diagnostics.Debug.WriteLine("\t\t\t\t\t" + totalUncovered);
            }

            //
            // Calculate the coverage percentages
            //
            double[] percentageCovered = new double[NUM_INTRINSIC];
            for (int w = 0; w < NUM_INTRINSIC; w++)
            {
                // If there are none of the particular node we have 'covered' them all
                if (numCoveredNodes[w] + numUncoveredNodes[w] == 0)
                {
                    percentageCovered[w] = 1;
                }
                else
                {
                    percentageCovered[w] = (double)(numCoveredNodes[w]) / (numCoveredNodes[w] + numUncoveredNodes[w]);
                }
            }

            return percentageCovered;
        }
    }
}
