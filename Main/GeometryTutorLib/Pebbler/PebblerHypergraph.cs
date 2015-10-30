using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    //
    // Coloration of pebbles for forward and backward problem derivation;
    // No edge should EVER be purple, only nodes
    //
    //public enum PebblerColorType
    //{
    //    NO_PEBBLE = -1,
    //    RED_FORWARD = 0,
    //    //BLUE_BACKWARD = 1,
    //    //BLACK_EDGE = 2
    //};

    //
    // A reduced version of the original hypergraph that provides simple pebbling and exploration
    //
    public class PebblerHypergraph<T, A>
    {
        // The main graph data structure
        public PebblerHyperNode<T, A>[] vertices { get; private set; }

        public List<int> GetPebbledNodes()
        {
            List<int> indices = new List<int>();

            for (int n = 0 ; n < vertices.Length; n++)
            {
                if (vertices[n].pebbled)
                {
                    indices.Add(n);
                }
            }

            return indices;
        }

        // The actual hypergraph for reference purposes only
        //private Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;
        //public void SetOriginalHypergraph(Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g)
        //{
        //    graph = g;
        //    forwardPebbledEdges.SetOriginalHypergraph(g);
        //    backwardPebbledEdges.SetOriginalHypergraph(g);
        //}

        // A static list of edges that can be processed using means other than a fixpoint analysis.
        //public HyperEdgeMultiMap<A> forwardPebbledEdges { get; private set; }
        //public HyperEdgeMultiMap<A> backwardPebbledEdges { get; private set; }

        //// For backward pebbling only: if we have pebbled an edge completely, the target node is then reachable.
        //private List<int> backwardEdgeReachableNodes;

        public PebblerHypergraph(PebblerHyperNode<T, A>[] inputVertices)
        {
            //graph = null; // This must be set outside to use
            vertices = inputVertices;
            //forwardPebbledEdges = new HyperEdgeMultiMap<A>(vertices.Length);
            //backwardPebbledEdges = new HyperEdgeMultiMap<A>(vertices.Length);
            //backwardEdgeReachableNodes = new List<int>();
        }

        ////
        //// Clear all pebbles from all nodes and edges in the hypergraph
        ////
        //public void ClearPebbles()
        //{
        //    foreach (PebblerHyperNode<T, A> node in vertices)
        //    {
        //        node.pebble = PebblerColorType.NO_PEBBLE;

        //        foreach (PebblerHyperEdge<A> edge in node.edges)
        //        {
        //            edge.sourcePebbles.Clear();
        //            edge.SetColor(PebblerColorType.NO_PEBBLE);
        //        }
        //    }
        //}

        public int NumVertices() { return vertices.Length; }

        //
        // Is the given node pebbled?
        //
        //public bool IsNodePebbledForward(int index)
        //{
        //    return vertices[index].pebble == PebblerColorType.RED_FORWARD;
        //}

        //public bool IsNodeNotPebbled(int index)
        //{
        //    return vertices[index].pebble == PebblerColorType.NO_PEBBLE;
        //}

        ////
        //// Use Dowling-Gallier pebbling technique to pebble using all given nodes
        ////
        //// Pebbling requires TWO phases:
        ////    1. Pebble in the forward direction any node which is reached once is RED.
        ////       Any node reached twice is PURPLE and identifies a backward node (via an eventual backward edge)
        ////       The result of this phase are:
        ////           a. marked (RED) nodes and (RED) edges for forward analysis through the graph.
        ////           b. marked (PURPLE) nodes (no edges).
        ////    2. Using the purple nodes as a starting point, we pebble the graph in a backward direction.
        ////       This is the same algorithm as pebbling forward (RED) edges, but this time we color the edges BLUE for backward.
        ////       The result of this phase are:
        ////           a. All applicable nodes marked BLUE or PURPLE.
        ////           b. All applicable backward edges marked BLUE.
        ////
        //public void Pebble(List<int> figure, List<int> givens)
        //{
        //    // Find all axiomatic nodes.
        //    List<int> axiomaticNodes = new List<int>();
        //    List<int> reflexiveNodes = new List<int>();
        //    List<int> obviousDefinitionNodes = new List<int>();
        //    for (int v = 0; v < graph.Size(); v++)
        //    {
        //        ConcreteAST.GroundedClause node = graph.GetNode(v);

        //        if (node.IsAxiomatic()) axiomaticNodes.Add(v);
        //        if (node.IsReflexive()) reflexiveNodes.Add(v);
        //        if (node.IsClearDefinition()) obviousDefinitionNodes.Add(v);                
        //    }

        //    // Forward pebble: it acquires the valid list of forward edges 
        //    PebbleForward(figure, givens, axiomaticNodes);

        //    // Backward pebble: acquires the valid list of bakcward edges 
        //    PebbleBackward(figure, axiomaticNodes, reflexiveNodes);
        //}

        ////
        //// In this version we are attempting to pebble exactly the same way in which the hypergraph was
        //// generated: using a worklist, breadth-first manner of construction.
        //// Returns the set of backward edges and backward reachable nodes
        //private void PebbleForward(List<int> figure, List<int> givens, List<int> axiomaticNodes)
        //{
        //    // Unique combining of all the given information
        //    List<int> nodesToBePebbled = new List<int>(figure);
        //    Utilities.AddUniqueList<int>(nodesToBePebbled, axiomaticNodes);
        //    Utilities.AddUniqueList<int>(nodesToBePebbled, givens);

        //    // Sort in ascending order for pebbling
        //    nodesToBePebbled.Sort();

        //    // Pebble all nodes and percolate
        //    ForwardTraversal(forwardPebbledEdges, nodesToBePebbled);
        //}

        ////
        //// Pebble the graph in the backward direction using all pebbled nodes from the forward direction.
        //// Note: we do so in a descending order (opposite the way we did from the forward direction); this attempts to 
        ////
        //private void PebbleBackward(List<int> figure, List<int> axiomaticNodes, List<int> reflexiveNodes)
        //{
        //    //
        //    // Acquire all nodes which are to be pebbled (reachable during forward analysis)
        //    //
        //    List<int> deducedNodesToPebbleBackward = new List<int>();

        //    // Avoid re-pebbling figure again so start after the figure
        //    for (int v = vertices.Length - 1; v >= figure.Count; v--)
        //    {
        //        if (IsNodePebbledForward(v))
        //        {
        //            deducedNodesToPebbleBackward.Add(v);
        //        }
        //    }

        //    // Clear all pebbles (nodes and edges)
        //    ClearPebbles();

        //    //
        //    // Pebble all Figure nodes, but do pursue edges: node -> node.
        //    // That is, the goal is to pebbles all the occurrences of figure nodes in edges (without traversing further).
        //    // We include, not just the intrinsic nodes in the list, but other relationships as well that are obvious:
        //    //       reflexive, OTHERS?
        //    //
        //    List<int> cumulativeIntrinsic = new List<int>();
        //    cumulativeIntrinsic.AddRange(figure);
        //    cumulativeIntrinsic.AddRange(reflexiveNodes);
        //    cumulativeIntrinsic.Sort();

        //    BackwardPebbleFigure(cumulativeIntrinsic);

        //    //
        //    // Pebble axiomatic nodes (and any edges); note axiomatic edges may occur in BOTH forward and backward problems
        //    //
        //    axiomaticNodes.Sort();
        //    ForwardTraversal(backwardPebbledEdges, axiomaticNodes);

        //    //
        //    // Pebble the graph in the backward direction using all pebbled nodes from the forward direction.
        //    // Note: we do so in a descending order (opposite the way we did from the forward direction)
        //    // We create an ascending list and will pull from the back of the list
        //    //
        //    ForwardTraversal(backwardPebbledEdges, deducedNodesToPebbleBackward);
        //}


        ////
        //// Given a node, pebble the reachable parts of the graph (in the forward direction)
        //// We pebble in a breadth first manner
        ////
        //private void ForwardTraversal(HyperEdgeMultiMap<A> edgeDatabase, List<int> nodesToPebble)
        //{
        //    List<int> worklist = new List<int>(nodesToPebble);

        //    //
        //    // Pebble until the list is empty
        //    //
        //    while (worklist.Any())
        //    {
        //        // Acquire the next value to consider
        //        int currentNodeIndex = worklist[0];
        //        worklist.RemoveAt(0);

        //        // Pebble the current node as a forward node and percolate forward
        //        vertices[currentNodeIndex].pebble = PebblerColorType.RED_FORWARD;

        //        // For all hyperedges leaving this node, mark a pebble along the arc
        //        foreach (PebblerHyperEdge<A> currentEdge in vertices[currentNodeIndex].edges)
        //        {
        //            if (!currentEdge.IsFullyPebbled())
        //            {
        //                // Indicate the node has been pebbled by adding to the list of pebbled vertices; should not have to be a unique addition
        //                Utilities.AddUnique<int>(currentEdge.sourcePebbles, currentNodeIndex);

        //                // Set the color of the edge (as a forward edge)
        //                currentEdge.SetColor(PebblerColorType.RED_FORWARD);

        //                // With this new node, check if the edge is full pebbled; if so, percolate
        //                if (currentEdge.IsFullyPebbled())
        //                {
        //                    // Has the target of this edge been pebbled previously? Pebbled -> Pebbled means we have a backward edge
        //                    if (IsNodePebbledForward(currentEdge.targetNode))
        //                    {
        //                        currentEdge.SetColor(PebblerColorType.BLACK_EDGE);
        //                    }
        //                    else if (IsNodeNotPebbled(currentEdge.targetNode))
        //                    {
        //                        // Success, we have a forward (RED) edge
        //                        // Construct a static set of pebbled hyperedges for problem construction
        //                        edgeDatabase.Put(currentEdge);

        //                        // Add this node to the worklist to percolate further
        //                        if (!worklist.Contains(currentEdge.targetNode))
        //                        {
        //                            worklist.Add(currentEdge.targetNode);
        //                            worklist.Sort();
        //                        }
        //                        //Utilities.InsertOrdered(worklist, currentEdge.targetNode);
        //                    }
        //                    else
        //                    {
        //                        throw new ArgumentException("Unexpected coloring of node: " + currentNodeIndex + " " + vertices[currentEdge.targetNode].pebble);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ////
        //// Pebble only the figure DO NOT traverse the pebble through the graph 
        ////
        //private void BackwardPebbleFigure(List<int> figure)
        //{
        //    foreach (int fIndex in figure)
        //    {
        //        // Pebble the current node as a backward; DO NOT PERCOLATE forward
        //        vertices[fIndex].pebble = PebblerColorType.RED_FORWARD; //.BLUE_BACKWARD;

        //        // For all hyperedges leaving this node, mark a pebble along the arc
        //        foreach (PebblerHyperEdge<A> currentEdge in vertices[fIndex].edges)
        //        {
        //            // Only pursue edges that are unpebbled or blue
        //            if (currentEdge.pebbleColor == PebblerColorType.NO_PEBBLE || currentEdge.pebbleColor == PebblerColorType.RED_FORWARD) //.BLUE_BACKWARD)
        //            {
        //                // Avoid a fully pebbled edge
        //                if (!currentEdge.IsFullyPebbled())
        //                {
        //                    // Indicate the node has been pebbled by adding to the list of pebbled vertices
        //                    Utilities.AddUnique<int>(currentEdge.sourcePebbles, fIndex);

        //                    // Set the color of the edge (as a forward edge)
        //                    currentEdge.SetColor(PebblerColorType.RED_FORWARD); //.BLUE_BACKWARD);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void DebugDumpClauses()
        //{
        //    StringBuilder edgeStr = new StringBuilder();

        //    int numNonPebbledNodes = 0;
        //    int numRedNodes = 0;
        //    int numBlueNodes = 0;
        //    int numPurpleNodes = 0;

        //    int numNonPebbledEdges = 0;
        //    int numRedEdges = 0;
        //    int numBlueEdges = 0;
        //    int numPurpleEdges = 0;
        //    int numBlackEdges = 0;

        //    Debug.WriteLine("\n Vertices:");
        //    edgeStr = new StringBuilder();
        //    for (int v = 0; v < vertices.Length; v++)
        //    {
        //        edgeStr.Append(v + ": ");
        //        switch (vertices[v].pebble)
        //        {
        //            case PebblerColorType.NO_PEBBLE:
        //                edgeStr.Append("NO PEBBLE");
        //                numNonPebbledNodes++;
        //                break;
        //            case PebblerColorType.RED_FORWARD:
        //                edgeStr.Append("RED");
        //                numRedNodes++;
        //                break;
        //            case PebblerColorType.BLUE_BACKWARD:
        //                edgeStr.Append("BLUE");
        //                numBlueNodes++;
        //                break;
        //            //case PebblerColorType.PURPLE_BOTH:
        //            //    edgeStr.Append("PURPLE");
        //            //    numPurpleNodes++;
        //            //    break;
        //        }
        //        edgeStr.AppendLine("");
        //    }

        //    Debug.WriteLine("\nPebbled Edges:");
        //    for (int v = 0; v < vertices.Length; v++)
        //    {
        //        if (vertices[v].edges.Any())
        //        {
        //            edgeStr.Append(v + ": {");
        //            foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //            {
        //                if (v == edge.sourceNodes.Min())
        //                {
        //                    edgeStr.Append(" { ");

        //                    if (edge.IsFullyPebbled()) edgeStr.Append("+ ");
        //                    else edgeStr.Append("- ");
        //                    if (edge.pebbleColor == PebblerColorType.NO_PEBBLE)
        //                    {
        //                        edgeStr.Append("(N) ");
        //                        numNonPebbledEdges++;
        //                    }
        //                    if (edge.pebbleColor == PebblerColorType.RED_FORWARD)
        //                    {
        //                        edgeStr.Append("(R) ");
        //                        numRedEdges++;
        //                    }
        //                    if (edge.pebbleColor == PebblerColorType.BLUE_BACKWARD)
        //                    {
        //                        edgeStr.Append("(BL) ");
        //                        numBlueEdges++;
        //                    }
        //                    //if (edge.pebbleColor == PebblerColorType.PURPLE_BOTH)
        //                    //{
        //                    //    edgeStr.Append("(P) ");
        //                    //    numPurpleEdges++;
        //                    //}
        //                    if (edge.pebbleColor == PebblerColorType.BLACK_EDGE)
        //                    {
        //                        edgeStr.Append("(BK) ");
        //                        numBlackEdges++;
        //                    }
        //                    foreach (int s in edge.sourceNodes)
        //                    {
        //                        edgeStr.Append(s + " ");
        //                    }
        //                    edgeStr.Append("} -> " + edge.targetNode + ", ");
        //                }
        //            }
        //            edgeStr.Remove(edgeStr.Length - 2, 2);
        //            edgeStr.Append(" }\n");
        //        }
        //    }

        //    edgeStr.AppendLine("\nPebbled Backward Edges:");
        //    for (int v = 0; v < vertices.Length; v++)
        //    {
        //        if (vertices[v].edges.Any())
        //        {
        //            bool containsBlueEdge = false;
        //            foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //            {
        //                if (edge.pebbleColor == PebblerColorType.BLUE_BACKWARD && v == edge.sourceNodes.Min())
        //                {
        //                    containsBlueEdge = true;
        //                    break;
        //                }
        //            }

        //            if (containsBlueEdge)
        //            {
        //                edgeStr.Append(v + ": {");
        //                foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //                {
        //                    if (edge.pebbleColor == PebblerColorType.BLUE_BACKWARD)
        //                    {
        //                        if (v == edge.sourceNodes.Min())
        //                        {
        //                            edgeStr.Append(" { ");

        //                            if (edge.IsFullyPebbled()) edgeStr.Append("+ ");
        //                            else edgeStr.Append("- ");

        //                            edgeStr.Append("(BL) ");

        //                            foreach (int s in edge.sourceNodes)
        //                            {
        //                                edgeStr.Append(s + " ");
        //                            }
        //                            edgeStr.Append("} -> " + edge.targetNode + ", ");
        //                        }
        //                    }
        //                }
        //            }
        //            edgeStr.Remove(edgeStr.Length - 2, 2);
        //            edgeStr.Append(" }\n");
        //        }
        //    }

        //    edgeStr.AppendLine("Nodes: ");
        //    edgeStr.AppendLine("\tNot Pebbled:\t" + numNonPebbledNodes);
        //    edgeStr.AppendLine("\tRed:\t\t\t" + numRedNodes);
        //    edgeStr.AppendLine("\tBlue:\t\t\t" + numBlueNodes);
        //    edgeStr.AppendLine("\tPurple:\t\t\t" + numPurpleNodes);
        //    edgeStr.AppendLine("\tTotal:\t\t\t" + (numNonPebbledNodes + numRedNodes + numBlueNodes + numPurpleNodes));

        //    edgeStr.AppendLine("Edges: ");
        //    edgeStr.AppendLine("\tNot Pebbled:\t" + numNonPebbledEdges);
        //    edgeStr.AppendLine("\tRed:\t\t\t" + numRedEdges);
        //    edgeStr.AppendLine("\tBlue:\t\t\t" + numBlueEdges);
        //    edgeStr.AppendLine("\tPurple:\t\t\t" + numPurpleEdges);
        //    edgeStr.AppendLine("\tBlack:\t\t\t" + numBlackEdges);
        //    edgeStr.AppendLine("\tTotal:\t\t\t" + (numNonPebbledEdges + numRedEdges + numBlueEdges + numBlackEdges + numPurpleEdges));

        //    Debug.WriteLine(edgeStr);
        //}

        //public void DebugDumpEdges()
        //{
        //    StringBuilder edgeStr = new StringBuilder();

        //    edgeStr.AppendLine("\nUnPebbled Edges:");
        //    for (int v = 0; v < vertices.Length; v++)
        //    {
        //        if (vertices[v].edges.Any())
        //        {
        //            bool containsEdge = false;
        //            foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //            {
        //                if (edge.pebbleColor != PebblerColorType.RED_FORWARD && v == edge.sourceNodes.Min())
        //                {
        //                    containsEdge = true;
        //                    break;
        //                }
        //            }

        //            if (containsEdge)
        //            {
        //                edgeStr.Append(v + ": {");
        //                foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //                {
        //                    if (edge.pebbleColor != PebblerColorType.RED_FORWARD)
        //                    {
        //                        if (v == edge.sourceNodes.Min())
        //                        {
        //                            edgeStr.Append(" { ");

        //                            foreach (int s in edge.sourceNodes)
        //                            {
        //                                edgeStr.Append(s + " ");
        //                            }
        //                            edgeStr.Append("} -> " + edge.targetNode + ", ");
        //                        }
        //                    }
        //                }
        //            }
        //            edgeStr.Remove(edgeStr.Length - 2, 2);
        //            edgeStr.Append(" }\n");
        //        }
        //    }

        //    edgeStr.AppendLine("\nPebbled Edges:");
        //    for (int v = 0; v < vertices.Length; v++)
        //    {
        //        if (vertices[v].edges.Any())
        //        {
        //            bool containsEdge = false;
        //            foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //            {
        //                if (edge.pebbleColor == PebblerColorType.RED_FORWARD && v == edge.sourceNodes.Min())
        //                {
        //                    containsEdge = true;
        //                    break;
        //                }
        //            }

        //            if (containsEdge)
        //            {
        //                edgeStr.Append(v + ": {");
        //                foreach (PebblerHyperEdge<A> edge in vertices[v].edges)
        //                {
        //                    if (edge.pebbleColor == PebblerColorType.RED_FORWARD)
        //                    {
        //                        if (v == edge.sourceNodes.Min())
        //                        {
        //                            edgeStr.Append(" { ");

        //                            if (edge.IsFullyPebbled()) edgeStr.Append("+ ");
        //                            else edgeStr.Append("- ");

        //                            foreach (int s in edge.sourceNodes)
        //                            {
        //                                edgeStr.Append(s + " ");
        //                            }
        //                            edgeStr.Append("} -> " + edge.targetNode + ", ");
        //                        }
        //                    }
        //                }
        //            }
        //            edgeStr.Remove(edgeStr.Length - 2, 2);
        //            edgeStr.Append(" }\n");
        //        }
        //    }

        //    Debug.WriteLine(edgeStr);
        //}
    }
}