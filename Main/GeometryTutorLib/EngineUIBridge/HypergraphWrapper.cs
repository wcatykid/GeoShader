using System.Collections.Generic;

namespace GeometryTutorLib.EngineUIBridge
{
    /// <summary>
    /// Contains a hypergraph and provides basic support for the front end to use the back-end graph:
    ///   Operations:
    ///      Query existence of a node
    ///      Return a hint when requested.
    /// </summary>
    public class HypergraphWrapper
    {
        private Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;
        private HintGenerator.HintGenerator hintGenerator;

        public HypergraphWrapper(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g)
        {
            graph = g;
            hintGenerator = new HintGenerator.HintGenerator(g);
        }

        //
        // Basic determination if the given node is in the hypergraph.
        //
        public bool QueryNodeInGraph(ConcreteAST.GroundedClause queryNode)
        {
            return graph.HasNode(queryNode);
        }

        //
        // Returns a list of indices of nodes not found in the graph.
        //
        public List<int> QueryNodesInGraph(List<ConcreteAST.GroundedClause> queryNodes)
        {
            List<int> notFoundIndices = new List<int>();

            for (int index = 0; index < queryNodes.Count; index++)
            {
                if (!QueryNodeInGraph(queryNodes[index])) notFoundIndices.Add(index);
            }

            return notFoundIndices;
        }

        //
        // In order to provide a hint, we need to have:
        //     (1) the current problem
        //     (2) all of the steps entered thus far
        //
        public ConcreteAST.GroundedClause QueryHint(ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation> problem,
                                                    List<ConcreteAST.GroundedClause> stepsTaken)
        {
            return hintGenerator.GetHint(problem, stepsTaken);
        }
    }
}