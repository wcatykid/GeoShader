using System.Collections.Generic;

namespace GeometryTutorLib.HintGenerator
{
    /// <summary>
    /// Functionality to generate a hint based on a problem and the current steps taken to solve the problem.
    /// </summary>
    public class HintGenerator
    {
        private Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;

        public HintGenerator(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g)
        {
            graph = g;
        }

        //
        // In order to provide a hint, we need to have:
        //     (1) the current problem
        //     (2) all of the steps entered thus far
        //
        // Algorithm:
        //   (a) Order the problem edges: least to greatest (they are by default)
        //   (b) Determine if any of the edges are complete and can deduce a new piece of information.
        //   (c) If no edge is complete, choose the node:
        //             (1) smallest node in an edge.
        //             (2) edge is more complete than any others.
        //             (3) node is not part of the problem givens
        //             (4) node is not part of the stepsTaken
        //           
        public ConcreteAST.GroundedClause GetHint(ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem,
                                                  List<ConcreteAST.GroundedClause> stepsTaken)
        {
            //
            // Convert the stepsTaken into indices
            //
            List<int> takenIndices = new List<int>();
            foreach (ConcreteAST.GroundedClause step in stepsTaken)
            {
                int index = Utilities.StructuralIndex(graph, step);
                if (index != -1) takenIndices.Add(index);
            }

            // Add the problem givens to the steps taken.
            problem.givens.ForEach(g => takenIndices.Add(g));

            int hintIndex = GetDeducibleEdgeTarget(problem, takenIndices);
            if (hintIndex != -1) return graph.vertices[hintIndex].data;

            hintIndex = GetClosestToCompleteEdge(problem, takenIndices);
            if (hintIndex != -1) return graph.vertices[hintIndex].data;

            return null;
        }

        private bool EdgeSourcesKnown(Pebbler.PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge, List<int> taken)
        {
            foreach (int src in edge.sourceNodes)
            {
                if (!taken.Contains(src)) return false;
            }

            return true;
        }

        private int EdgeSourcesUnknownCount(Pebbler.PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge, List<int> taken)
        {
            int count = 0;
            foreach (int src in edge.sourceNodes)
            {
                if (!taken.Contains(src)) count++;
            }

            return count;
        }

        //
        // For each edge, if all the source nodes are known, then return the target.
        //
        private int GetDeducibleEdgeTarget(ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem, List<int> taken)
        {
            for (int edgeIndex = 0; edgeIndex < problem.edges.Count; edgeIndex++)
            {
                // Avoid edges, we have already deduced
                if (!taken.Contains(problem.edges[edgeIndex].targetNode))
                {
                    if (EdgeSourcesKnown(problem.edges[edgeIndex], taken)) return problem.edges[edgeIndex].targetNode;
                }
            }

            return -1;
        }

        //
        // For each edge, find the edge which is closest to knowing all sources, return the node with the least target.
        //
        private int GetClosestToCompleteEdge(ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem, List<int> taken)
        {
            int minUnknownCount = int.MaxValue;
            Pebbler.PebblerHyperEdge<Hypergraph.EdgeAnnotation> minUnknownEdge = null;

            for (int edgeIndex = 0; edgeIndex < problem.edges.Count; edgeIndex++)
            {
                // Avoid edges, we have already deduced
                if (!taken.Contains(problem.edges[edgeIndex].targetNode))
                {
                    // Find minimal edge
                    int currUnknownCount = EdgeSourcesUnknownCount(problem.edges[edgeIndex], taken);
                    if (currUnknownCount < minUnknownCount) // Not <= since we want the earliest edge.
                    {
                        minUnknownCount = currUnknownCount;
                        minUnknownEdge = problem.edges[edgeIndex];
                    }
                }
            }

            if (minUnknownEdge == null) return -1;

            // Now that we know the minimum edge, find the minimum node in the edge that is unknown 
            foreach (int src in minUnknownEdge.sourceNodes)
            {
                if (!taken.Contains(src)) return src;
            }

            return -1;
        }
    }
}
