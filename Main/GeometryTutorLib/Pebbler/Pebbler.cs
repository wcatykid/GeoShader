using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    //
    // A reduced version of the original hypergraph that provides simple pebbling and exploration
    //
    public class Pebbler
    {
        // The pebbling version (integer-based) of the hypergraph to work on. 
        private PebblerHypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> pebblerGraph;

        // The actual hypergraph for reference purposes only
        private Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;

        // A static list of edges that can be processed using means other than a fixpoint analysis.
        public HyperEdgeMultiMap<Hypergraph.EdgeAnnotation> forwardPebbledEdges { get; private set; }
        public HyperEdgeMultiMap<Hypergraph.EdgeAnnotation> backwardPebbledEdges { get; private set; }

        public Pebbler(Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph,
                       PebblerHypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> pGraph)
        {
            this.graph = graph;
            this.pebblerGraph = pGraph;
            
            forwardPebbledEdges = new HyperEdgeMultiMap<Hypergraph.EdgeAnnotation>(pGraph.vertices.Length);
            backwardPebbledEdges = new HyperEdgeMultiMap<Hypergraph.EdgeAnnotation>(pGraph.vertices.Length);

            forwardPebbledEdges.SetOriginalHypergraph(graph);
            backwardPebbledEdges.SetOriginalHypergraph(graph);
        }

        // Clear all pebbles from all nodes and edges in the hypergraph
        private void ClearPebbles()
        {
            foreach (PebblerHyperNode<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> node in pebblerGraph.vertices)
            {
                node.pebbled = false;

                foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in node.edges)
                {
                    edge.sourcePebbles.Clear();
                    edge.pebbled = false;
                }
            }
        }

        //
        // Use Dowling-Gallier pebbling technique to pebble using all given nodes
        //
        public void Pebble(List<int> figure, List<int> givens)
        {
            // Find all axiomatic, reflexive, and other obvious notions which may go both directions for solving problems.
            List<int> axiomaticNodes = new List<int>();
            List<int> reflexiveNodes = new List<int>();
            List<int> obviousDefinitionNodes = new List<int>();
            for (int v = 0; v < graph.Size(); v++)
            {
                ConcreteAST.GroundedClause node = graph.GetNode(v);

                if (node.IsAxiomatic()) axiomaticNodes.Add(v);
                if (node.IsReflexive()) reflexiveNodes.Add(v);
                if (node.IsClearDefinition()) obviousDefinitionNodes.Add(v);
            }

            // Forward pebble: it acquires the valid list of forward edges 
            PebbleForward(figure, givens, axiomaticNodes);

            // Backward pebble: acquires the valid list of bakcward edges 
            PebbleBackward(figure, axiomaticNodes, reflexiveNodes);
        }

        //
        // Use Dowling-Gallier pebbling technique to pebble using all given nodes
        //
        public void PebbleForwardForShading(List<int> figure, List<int> givens)
        {
            // Find all axiomatic, reflexive, and other obvious notions which may go both directions for solving problems.
            List<int> axiomaticNodes = new List<int>();
            List<int> reflexiveNodes = new List<int>();
            List<int> obviousDefinitionNodes = new List<int>();
            for (int v = 0; v < graph.Size(); v++)
            {
                ConcreteAST.GroundedClause node = graph.GetNode(v);

                if (node.IsAxiomatic()) axiomaticNodes.Add(v);
                if (node.IsReflexive()) reflexiveNodes.Add(v);
                if (node.IsClearDefinition()) obviousDefinitionNodes.Add(v);
            }

            // Forward pebble: it acquires the valid list of forward edges 
            PebbleForward(figure, givens, axiomaticNodes);
        }

        //
        // We are attempting to pebble exactly the same way in which the hypergraph was generated: using a
        // worklist, breadth-first manner of construction.
        //
        private void PebbleForward(List<int> figure, List<int> givens, List<int> axiomaticNodes)
        {
            // Combine all the given information uniquely.
            List<int> nodesToBePebbled = new List<int>(figure);
            Utilities.AddUniqueList<int>(nodesToBePebbled, axiomaticNodes);
            Utilities.AddUniqueList<int>(nodesToBePebbled, givens);

            // Sort in ascending order for pebbling
            nodesToBePebbled.Sort();

            // Pebble all nodes and percolate
            ForwardTraversal(forwardPebbledEdges, nodesToBePebbled);
        }

        private bool IsNodePebbled(int v)
        {
            return pebblerGraph.vertices[v].pebbled;
        }

        //
        // Pebble the graph in the backward direction using all pebbled nodes from the forward direction.
        // Note: we do so in a descending order (opposite the way we did from the forward direction); this attempts to 
        //
        private void PebbleBackward(List<int> figure, List<int> axiomaticNodes, List<int> reflexiveNodes)
        {
            //
            // Acquire all nodes which are to be pebbled (reachable during forward analysis)
            //
            List<int> deducedNodesToPebbleBackward = new List<int>();

            // Avoid re-pebbling figure again so start after the figure
            for (int v = pebblerGraph.vertices.Length - 1; v >= figure.Count; v--)
            {
                if (IsNodePebbled(v)) deducedNodesToPebbleBackward.Add(v);
            }

            // Clear all pebbles (nodes and edges)
            ClearPebbles();

            //
            // Pebble all Figure nodes, but do pursue edges: node -> node.
            // That is, the goal is to pebbles all the occurrences of figure nodes in edges (without traversing further).
            // We include, not just the intrinsic nodes in the list, but other relationships as well that are obvious:
            //       reflexive, OTHERS?
            //
            List<int> cumulativeIntrinsic = new List<int>();
            cumulativeIntrinsic.AddRange(figure);
            cumulativeIntrinsic.AddRange(reflexiveNodes);
            cumulativeIntrinsic.Sort();

            BackwardPebbleFigure(cumulativeIntrinsic);

            //
            // Pebble axiomatic nodes (and any edges); note axiomatic edges may occur in BOTH forward and backward problems
            //
            axiomaticNodes.Sort();
            ForwardTraversal(backwardPebbledEdges, axiomaticNodes);

            //
            // Pebble the graph in the backward direction using all pebbled nodes from the forward direction.
            // Note: we do so in a descending order (opposite the way we did from the forward direction)
            // We create an ascending list and will pull from the back of the list
            //
            ForwardTraversal(backwardPebbledEdges, deducedNodesToPebbleBackward);
        }


        //
        // Given a node, pebble the reachable parts of the graph (in the forward direction)
        // We pebble in a breadth first manner
        //
        private void ForwardTraversal(HyperEdgeMultiMap<Hypergraph.EdgeAnnotation> edgeDatabase, List<int> nodesToPebble)
        {
            List<int> worklist = new List<int>(nodesToPebble);

            //
            // Pebble until the list is empty
            //
            while (worklist.Any())
            {
                // Acquire the next value to consider
                int currentNodeIndex = worklist[0];
                worklist.RemoveAt(0);

                // Pebble the current node as a forward node and percolate forward
                pebblerGraph.vertices[currentNodeIndex].pebbled = true;

                // For all hyperedges leaving this node, mark a pebble along the arc
                foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> currentEdge in pebblerGraph.vertices[currentNodeIndex].edges)
                {
                    if (!Utilities.RESTRICTING_AXS_DEFINITIONS_THEOREMS || (Utilities.RESTRICTING_AXS_DEFINITIONS_THEOREMS && currentEdge.annotation.IsActive()))
                    {
                        if (!currentEdge.IsFullyPebbled())
                        {
                            // Indicate the node has been pebbled by adding to the list of pebbled vertices; should not have to be a unique addition
                            Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

                            // With this new node, check if the edge is full pebbled; if so, percolate
                            if (currentEdge.IsFullyPebbled())
                            {
                                // Has the target of this edge been pebbled previously? Pebbled -> Pebbled means we have a backward edge
                                if (!IsNodePebbled(currentEdge.targetNode))
                                {
                                    // Success, we have an edge
                                    // Construct a static set of pebbled hyperedges for problem construction
                                    edgeDatabase.Put(currentEdge);

                                    // Add this node to the worklist to percolate further
                                    if (!worklist.Contains(currentEdge.targetNode))
                                    {
                                        worklist.Add(currentEdge.targetNode);
                                        worklist.Sort();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //
        // Pebble only the figure DO NOT traverse the pebble through the graph 
        //
        private void BackwardPebbleFigure(List<int> figure)
        {
            foreach (int fIndex in figure)
            {
                // Pebble the current node as a backward; DO NOT PERCOLATE forward
                pebblerGraph.vertices[fIndex].pebbled = true;

                // For all hyperedges leaving this node, mark a pebble along the arc
                foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> currentEdge in pebblerGraph.vertices[fIndex].edges)
                {
                    // Avoid a fully pebbled edge
                    if (!currentEdge.IsFullyPebbled())
                    {
                        // Indicate the node has been pebbled by adding to the list of pebbled vertices
                        Utilities.AddUnique<int>(currentEdge.sourcePebbles, fIndex);
                    }
                }
            }
        }

        public void DebugDumpClauses()
        {
            StringBuilder edgeStr = new StringBuilder();

            int numNonPebbledNodes = 0;
            int numPebbledNodes = 0;

            Debug.WriteLine("\n Vertices:");
            edgeStr = new StringBuilder();
            for (int v = 0; v < pebblerGraph.vertices.Length; v++)
            {
                edgeStr.Append(v + ": ");

                if (IsNodePebbled(v))
                {
                    edgeStr.AppendLine("PEBBLE");
                    numPebbledNodes++;
                }
                else
                {
                    edgeStr.AppendLine("NOT");
                    numNonPebbledNodes++;
                }
            }

            Debug.WriteLine("\nPebbled Edges:");
            for (int v = 0; v < pebblerGraph.vertices.Length; v++)
            {
                if (pebblerGraph.vertices[v].edges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in pebblerGraph.vertices[v].edges)
                    {
                        if (v == edge.sourceNodes.Min())
                        {
                            edgeStr.Append(" { ");

                            if (edge.IsFullyPebbled()) edgeStr.Append("+ ");
                            else edgeStr.Append("- ");

                            foreach (int s in edge.sourceNodes)
                            {
                                edgeStr.Append(s + " ");
                            }
                            edgeStr.Append("} -> " + edge.targetNode + ", ");
                        }
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }

            Debug.WriteLine(edgeStr);
            DebugDumpEdges();
        }

        public void DebugDumpEdges()
        {
            StringBuilder edgeStr = new StringBuilder();

            edgeStr.AppendLine("\nUnPebbled Edges:");
            for (int v = 0; v < pebblerGraph.vertices.Length; v++)
            {
                if (pebblerGraph.vertices[v].edges.Any())
                {
                    bool containsEdge = false;
                    foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in pebblerGraph.vertices[v].edges)
                    {
                        if (!edge.IsFullyPebbled() && v == edge.sourceNodes.Min())
                        {
                            containsEdge = true;
                            break;
                        }
                    }

                    if (containsEdge)
                    {
                        edgeStr.Append(v + ": {");
                        foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in pebblerGraph.vertices[v].edges)
                        {
                            if (!edge.IsFullyPebbled() && v == edge.sourceNodes.Min())
                            {
                                edgeStr.Append(" { ");

                                foreach (int s in edge.sourceNodes)
                                {
                                    edgeStr.Append(s + " ");
                                }
                                edgeStr.Append("} -> " + edge.targetNode + ", ");
                            }
                        }
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }

            edgeStr.AppendLine("\nPebbled Edges:");
            for (int v = 0; v < pebblerGraph.vertices.Length; v++)
            {
                if (pebblerGraph.vertices[v].edges.Any())
                {
                    bool containsEdge = false;
                    foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in pebblerGraph.vertices[v].edges)
                    {
                        if (edge.IsFullyPebbled() && v == edge.sourceNodes.Min())
                        {
                            containsEdge = true;
                            break;
                        }
                    }

                    if (containsEdge)
                    {
                        edgeStr.Append(v + ": {");
                        foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in pebblerGraph.vertices[v].edges)
                        {
                            if (edge.IsFullyPebbled() && v == edge.sourceNodes.Min())
                            {
                                edgeStr.Append(" { + ");

                                foreach (int s in edge.sourceNodes)
                                {
                                    edgeStr.Append(s + " ");
                                }
                                edgeStr.Append("} -> " + edge.targetNode + ", ");
                            }
                        }
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }

            Debug.WriteLine(edgeStr);
        }
    }
}