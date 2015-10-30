using System;
using System.Linq;
using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Hypergraph;
using System.Diagnostics;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class TemplateProblemGenerator
    {
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;
        Pebbler.Pebbler pebbler;
        ProblemAnalyzer.PathGenerator pathGenerator;

        public TemplateProblemGenerator(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g,
                                        Pebbler.Pebbler pebbler,
                                        ProblemAnalyzer.PathGenerator generator)
        {
            graph = g;
            this.pebbler = pebbler;
            pathGenerator = generator;
        }

        //
        // Do the assumptions of the problem define the entire figure?
        //
        public bool DefinesFigure(List<Descriptor> descriptors, List<Strengthened> strengthened)
        {
            // Combine the precomputed descriptors and strengthened clauses together into one list
            List<GroundedClause> goals = new List<GroundedClause>();
            descriptors.ForEach(r => goals.Add(r));
            strengthened.ForEach(r => goals.Add(r));

            bool allGoalsCovered = true;

            foreach (GroundedClause goal in goals)
            {
                // We need to restrict problems generated based on goal nodes; don't want an obvious notion as a goal
                if (goal.IsAbleToBeAGoalNode())
                {
                    // Find the integer clause ID representation in the standard hypergraph
                    int nodeIndex = Utilities.StructuralIndex(graph, goal);

                    if (nodeIndex == -1)
                    {
                        if (!(goal is ProportionalAngles) && !(goal is SegmentRatio))
                        {
                            System.Diagnostics.Debug.WriteLine("Did not find precomputed node in the hypergraph: " + goal.ToString());
                            allGoalsCovered = false;
                        }

                        if (Utilities.PROBLEM_GEN_DEBUG)
                        {
                            System.Diagnostics.Debug.WriteLine("ERROR: Did not find precomputed node in the hypergraph: " + goal.ToString());
                        }
                    }
                }
            }

            return allGoalsCovered;
        }

        //
        // Generate all forward and backward problems based on the template nodes in the input list
        //
        public KeyValuePair<List<Problem<Hypergraph.EdgeAnnotation>>, List<Problem<Hypergraph.EdgeAnnotation>>> Generate(List<Descriptor> descriptors,
                                                                                                                         List<Strengthened> strengthened,
                                                                                                                         List<GroundedClause> givens)
        {
            // Combine the precomputed descriptors and strengthened clauses together into one list
            List<GroundedClause> descriptorsAndStrengthened = new List<GroundedClause>();
            descriptors.ForEach(r => descriptorsAndStrengthened.Add(r));
            strengthened.ForEach(r => descriptorsAndStrengthened.Add(r));

            List<Problem<Hypergraph.EdgeAnnotation>> forwardList = GenerateForwardProblems(descriptorsAndStrengthened, pebbler.forwardPebbledEdges, givens.Count);
            List<Problem<Hypergraph.EdgeAnnotation>> backwardList = GenerateBackwardProblems(givens, pebbler.backwardPebbledEdges);

            if (Utilities.BACKWARD_PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine("--------------------------------- Begin Backward problems----------------------------");
                foreach (Problem<Hypergraph.EdgeAnnotation> problem in backwardList)
                {
                    Debug.WriteLine(problem.ConstructProblemAndSolution(graph) + "\n");
                }
                Debug.WriteLine("--------------------------------- End Backward problems----------------------------");
            }

            return new KeyValuePair<List<Problem<Hypergraph.EdgeAnnotation>>, List<Problem<Hypergraph.EdgeAnnotation>>>(forwardList, backwardList);
        }

        //
        // Generates all forward directional problems using the template-based clauses from the pre-computation
        //
        private List<Problem<Hypergraph.EdgeAnnotation>> GenerateForwardProblems(List<GroundedClause> goalClauses,
                                                                                 Pebbler.HyperEdgeMultiMap<Hypergraph.EdgeAnnotation> edgeDatabase,
                                                                                 int numGivens)
        {
//            System.Diagnostics.Debug.WriteLine("Forward");

            List<int> clauseIndices = AcquireGoalIndices(goalClauses);

            // The resultant structure of problems; graph size dictates associated array size in hashMap; number of givens is a limiting factor the size of problems
            ProblemHashMap<Hypergraph.EdgeAnnotation> problems = new ProblemHashMap<Hypergraph.EdgeAnnotation>(edgeDatabase, graph.vertices.Count, numGivens);

            // Generate all the problems based on the node indices
            foreach (int goalNode in clauseIndices)
            {
                if (Utilities.PROBLEM_GEN_DEBUG)
                {
                    System.Diagnostics.Debug.WriteLine("Template node; will generate problems (" + goalNode + "): " + graph.vertices[goalNode].data.ToString());
                }

                pathGenerator.GenerateProblemsUsingBackwardPathToLeaves(problems, edgeDatabase, goalNode);
            }

            return FilterForMinimalAndRedundantProblems(problems.GetAll());
        }

        private List<Problem<Hypergraph.EdgeAnnotation>> GenerateBackwardProblems(List<GroundedClause> goalClauses,
                                                                                  Pebbler.HyperEdgeMultiMap<Hypergraph.EdgeAnnotation> edgeDatabase)
        {
//            System.Diagnostics.Debug.WriteLine("Backward");

            List<int> clauseIndices = AcquireGoalIndices(goalClauses);

            // The resultant structure of problems; graph size dictates associated array size in hashMap; number of givens is a limiting factor the size of problems
            // Problem generation limits the number of givens in BACKWARD problems to 4 (or the constant in the HashMap structure)
            ProblemHashMap<Hypergraph.EdgeAnnotation> problems = new ProblemHashMap<Hypergraph.EdgeAnnotation>(edgeDatabase, graph.vertices.Count);

            // Generate all the problems based on the node indices
            foreach (int goalNode in clauseIndices)
            {
                if (Utilities.PROBLEM_GEN_DEBUG)
                {
                    System.Diagnostics.Debug.WriteLine("Template node; will generate problems (" + goalNode + "): " + graph.vertices[goalNode].data.ToString());
                }

                pathGenerator.GenerateProblemsUsingBackwardPathToLeaves(problems, edgeDatabase, goalNode);
            }

            // Filter backward problems so that only problems with goal being the original figure givens persist.
            return FilterBackwardProblems(clauseIndices, FilterForMinimalAndRedundantProblems(problems.GetAll()));
        }

        //
        // Filter the backward problems so that only problems with goals (which were givens) persist.
        //
        private List<Problem<Hypergraph.EdgeAnnotation>> FilterBackwardProblems(List<int> goalIndices, List<Problem<Hypergraph.EdgeAnnotation>> problems)
        {
            List<Problem<Hypergraph.EdgeAnnotation>> filtered = new List<Problem<Hypergraph.EdgeAnnotation>>();

            foreach (Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                if (goalIndices.Contains(problem.goal)) filtered.Add(problem);
            }

            return filtered;
        }


        //
        // Get the index of all the given nodes from the hypergraph
        //
        private List<int> AcquireGoalIndices(List<GroundedClause> goalClauses)
        {
            List<int> clauseIndices = new List<int>();

            foreach (GroundedClause clause in goalClauses)
            {
                // We need to restrict problems generated based on goal nodes; don't want an obvious notion as a goal
                if (clause.IsAbleToBeAGoalNode())
                {
                    // Find the integer clause ID representation in the standard hypergraph
                    int nodeIndex = Utilities.StructuralIndex(graph, clause);

                    if (nodeIndex == -1)
                    {
                        if (Utilities.PROBLEM_GEN_DEBUG)
                        {
                            // System.Diagnostics.Debug.WriteLine("ERROR: Did not find precomputed node in the hypergraph: " + clause.ToString());
                        }
                    }
                    else
                    {
                        clauseIndices.Add(nodeIndex);
                    }
                }
            }

            // Sort in ascending order
            clauseIndices.Sort();

            return clauseIndices;
        }

        //
        // Compares all generated problems:
        // If a problem has a subset of givens (compared to another problem) then the problem with the subset is chosen.
        // If a problem has the same givens and goal, the shorter (edge-based) problem is chosen.
        //
        public List<Problem<Hypergraph.EdgeAnnotation>> FilterForMinimalAndRedundantProblems(List<Problem<Hypergraph.EdgeAnnotation>> problems)
        {
            List<Problem<Hypergraph.EdgeAnnotation>> filtered = new List<Problem<Hypergraph.EdgeAnnotation>>();

            // It is possible for no problems to be generated
            if (!problems.Any()) return problems;

            // For each problem, break the givens into actual vs. suppressed given information
            problems.ForEach(problem => problem.DetermineSuppressedGivens(graph));

            //
            // Filter the problems based on same set of source nodes and goal node
            //   All of these problems have exactly the same goal node.
            //   Now, if we have multiple problems with the exact same (non-suppressed) source nodes, choose the one with shortest path.
            //
            bool[] marked = new bool[problems.Count];
            for (int p1 = 0; p1 < problems.Count - 1; p1++)
            {
                // We may have marked this earlier
                if (!marked[p1])
                {
                    // Save the minimal problem
                    Problem<Hypergraph.EdgeAnnotation> minimalProblem = problems[p1];
                    for (int p2 = p1 + 1; p2 < problems.Count; p2++)
                    {
                        // If we have not yet compared to a problem
                        if (!marked[p2])
                        {
                            // Both problems need the same goal node
                            if (minimalProblem.goal == problems[p2].goal)
                            {
                                // Check if the givens from the minimal problem and this candidate problem equate exactly
                                if (Utilities.EqualSets<int>(minimalProblem.givens, problems[p2].givens))
                                {
                                    // We have now analyzed this problem
                                    marked[p2] = true;

                                    // Choose the shorter problem (fewer edges wins)
                                    if (problems[p2].edges.Count < minimalProblem.edges.Count)
                                    {
                                        // if (Utilities.PROBLEM_GEN_DEBUG) Debug.WriteLine("Outer Filtering: " + minimalProblem.ToString() + " for " + problems[p2].ToString());
                                        minimalProblem = problems[p2];
                                    }
                                    else
                                    {
                                        // if (Utilities.PROBLEM_GEN_DEBUG) Debug.WriteLine("Outer Filtering: " + problems[p2].ToString() + " for " + minimalProblem.ToString());
                                    }
                                }
                                // Check if the givens from new problem are a subset of the givens of the minimal problem.
                                else if (Utilities.Subset<int>(minimalProblem.givens, problems[p2].givens))
                                {
                                    marked[p2] = true;

                                    if (Utilities.PROBLEM_GEN_DEBUG || Utilities.BACKWARD_PROBLEM_GEN_DEBUG)
                                    {
                                        // Debug.WriteLine("Filtering for Minimal Givens: " + minimalProblem.ToString() + " for " + problems[p2].ToString());
                                    }
                                    minimalProblem = problems[p2];
                                }
                                // Check if the givens from new problem are a subset of the givens of the minimal problem.
                                else if (Utilities.Subset<int>(problems[p2].givens, minimalProblem.givens))
                                {
                                    marked[p2] = true;

                                    if (Utilities.PROBLEM_GEN_DEBUG || Utilities.BACKWARD_PROBLEM_GEN_DEBUG)
                                    {
                                        // Debug.WriteLine("Filtering for Minimal Givens: " + problems[p2].ToString() + " for " + minimalProblem.ToString());
                                    }
                                }
                            }
                        }
                    }
                    // Add the minimal problem to the list to be returned
                    filtered.Add(minimalProblem);
                }
            }

            // Pick up last problem in the list
            if (!marked[problems.Count - 1]) filtered.Add(problems[problems.Count - 1]);

            if (Utilities.PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine("Generated Problems: " + problems.Count);
                Debug.WriteLine("Filtered Problems: " + (problems.Count - filtered.Count));
                Debug.WriteLine("Problems Remaining: " + filtered.Count);
            }

            if (problems.Count < filtered.Count)
            {
                Debug.WriteLine("Filtered list is larger than original list!");
            }

            return filtered;
        }
    }
}