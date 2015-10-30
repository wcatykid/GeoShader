using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    /**
     * This class implements all necessary methods to carry out Suurballe's Algorithm for dual-shortest path finding.
     * 
     * SURBALLE-DUAL-SHORTEST-PATH(G(V, E), S, T)
     * 
     *   G'(V', E') := CONSTRUCT-DISJOINT-VERTEX-PathGraph($G(V, E)$)
     * 
     *   P := DIJKSTRA(G'(V', E'), S, T)
     * 
     *   G''(V'', E'') := REVERSE-PATH-EDGES(G'(V', E'), P)
     * 
     *   P' := DIJKSTRA(G''(V'', E''), S, T)
     * 
     *   REMOVE-COMMON-EDGES(P, P')
     * 
     *   return <P, P'>
     *    
     * @author ctalvin
     *
     */

    //
    // Implements all functionality necessary to perform Suurballe's algorithm
    //
    public class Suurballe
    {
        private const bool DEBUG = true; // ShortestPathMain.DEBUG;
        private const int NIL = -1;

        //
        // Start and goal nodes as specified by the user
        //
        private int startNode;
        private int goalNode;

        // The input PathGraph
        private PathGraph graph;

        // For each vertex in the PathGraph, we track the shortest distance to the node,
        // predecessor, and a pointer to the node in the heap (so we may DecreaseKey)
        private VertexValue[] shortPathEst;

        //
        // An aggregator class for shortest distance and predecessor data; specified by CLRS
        //
        private class VertexValue
        {
            public int d;           // shortest distance to the given node from the start node
            public int pi;          // index of the node's predecessor
            public HeapNode<int> heapNode; // pointer to the actual node in the heap

            // Basic constructor
            public VertexValue(int d, int pi) { this.d = d; this.pi = pi; heapNode = null; }
        }

        //
        // Basic Path constructor
        //
        public Suurballe(PathGraph g)
        {
            graph = g;
            startNode = 0;                  // Default path starts at the first vertex: 0 
            goalNode = g.NumVertices() - 1; // Default goal is the last vertex
        }

        //
        // Follows Procedure in CLRS on page 648:
        // Initializes all weights to a specified upper bound: positive infinity
        //
        void InitializeSingleSource(int upperBound)
        {
            int numVerts = graph.NumVertices();
            shortPathEst = new VertexValue[numVerts];

            for (int i = 0; i < numVerts; i++)
            {
                shortPathEst[i] = new VertexValue(upperBound, NIL);
            }

            // Gives us a starting point to begin search at the start node
            shortPathEst[startNode].d = 0; // start-node.d = 0
        }

        //
        // Follows the RELAX specifications dictated in CLRS: Page 649.
        // w(u, v) : weight from vertex u to vertex v
        // Reduces the shortest path of a node (if necessary) and updates the value of the node in the heap (DecreaseKey)
        //
        //void Relax(int u, int v, int w, FibonacciHeap<int> heap)
        void Relax(int u, int v, int w, MinHeap heap)
        //void Relax(int u, int v, int w,  NaiveMinContainer<int> heap)
        {
            //
            // If we find a shorter path, update the new shortest path for a node AND the predecessor
            //
            if (shortPathEst[v].d > shortPathEst[u].d + w)
            {
                shortPathEst[v].d = shortPathEst[u].d + w;
                shortPathEst[v].pi = u;

                //
                // Relaxing may result in a node in the heap having a shorter path to that node;
                // so decrease key to ensure the heap represents the new state
                //
                heap.DecreaseKey(shortPathEst[v].heapNode, shortPathEst[v].d);
            }
        }

        //
        // Performs Dijkstra's shortest path algorithm on the the constructed PathGraph object
        //
        // @param path -- Returns the actual shortest path in the List parameter
        // @return the length of the shortest path via the return value
        //
        public int Dijkstra(List<int> path)
        {
            // Initialize all shortest paths to a node to be large and the shortest path to a start node to be 0
            InitializeSingleSource(int.MaxValue - 100000); // -100000 prevents issues with arithmetic overflow

            //
            // Create the min-Fibonacci Heap by pushing all vertices onto the Heap
            //
            int numV = graph.NumVertices();
            //FibonacciHeap<int> vertices = new FibonacciHeap<int>();
            MinHeap vertices = new MinHeap(numV);
            for (int i = 0; i < numV; i++) // VertexValues node : shortPathEst)
            {
                // Insert the weight, d, which is the short path to node i
                HeapNode<int> node = new HeapNode<int>(i);
                vertices.Insert(node, shortPathEst[i].d);
                shortPathEst[i].heapNode = node; // maintain a pointer to the object for DecreaseKey
            }

            //
            // Visit all vertices in the PathGraph (always visiting the shortest distance from the start node first)
            // Do so by using a Fibonacci Heap
            //
            while (!vertices.IsEmpty())
            {
                //
                // Acquire the minimum: min will contain a pair <shortest distance to node n, node n>
                //
                HeapNode<int> min = vertices.ExtractMin();    // Now, extract the node

                //
                // For each adjacent vertex, relax
                //
                int nodeIndex = min.data;
                List<int> adjNodes = graph.AdjacentNodes(nodeIndex);
                foreach (int neighbor in adjNodes)
                {
                    Relax(nodeIndex, neighbor, graph.GetWeight(nodeIndex, neighbor), vertices);
                }
            }

            //
            // Look at predecessors to acquire the actual shortest path
            //
            path.Add(goalNode); // Add the last node

            // Predecessor node 
            int predNode = shortPathEst[goalNode].pi;

            // Loop while we have not yet reached the start node
            // If we hit a NIL, a path does not exist
            while (predNode != startNode && predNode != NIL)
            {
                path.Insert(0, predNode); // Insert at the beginning of the List; index 0
                predNode = shortPathEst[predNode].pi;
            }

            // Check if the goal node was even reachable
            if (predNode == NIL)
            {
                path.Clear();
                return NIL;
            }

            // Insert the first node at the beginning
            path.Insert(0, predNode);

            return shortPathEst[goalNode].d; // recall .d contains the shortest path length to the goalNode
        }

        //
        // Given a path, reverse all edges in the PathGraph
        //
        private void reverseShortestPathEdges(List<int> path)
        {
            // For all vertices in the path
            for (int i = 1; i < path.Count - 2; i++)
            {
                // Reverse the edge; DO NOT reverse the first or last arc
                graph.ReverseEdge(path.ElementAt(i), path.ElementAt(i + 1));
            }
        }

        //
        // Returns the next block that will be added to the path (given the current vertex)
        // This method is strictly for the first path.
        // Note: in the case where we have a direct subsequence of edges to be deleted, we loop to avoid them appropriately
        private List<int> getNextBlock(Dictionary<int, int> edges, List<int>[] blocks, int vertex)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                // Do we have the next block?
                if (blocks[i].ElementAt(0) == vertex)
                {
                    // We have a series of edges that are to be removed
                    if (blocks[i].Count == 1)
                    {
                        // Reset the list and seek the next block
                        i = 0;
                        vertex = edges.ElementAt(blocks[i].ElementAt(0)).Value;
                    }
                    else return blocks[i];
                }
            }

            return null;
        }

        //
        // Returns the next block that will be added to the path (given the current vertex)
        // This method is strictly for the second path.
        // Note: in the case where we have a direct subsequence of edges to be deleted, we loop to avoid them appropriately
        private List<int> getNextBlock(Dictionary<int, int> edges, List<List<int>> blocks, int vertex)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                // Do we have the next block?
                if (blocks.ElementAt(i).ElementAt(0) == vertex)
                {
                    // We have a series of edges that are to be removed
                    if (blocks.ElementAt(i).Count == 1)
                    {
                        // Reset the list and seek the next block
                        i = 0;
                        vertex = edges.ElementAt(blocks.ElementAt(i).ElementAt(0)).Value;
                    }
                    else return blocks.ElementAt(i);
                }
            }

            return null;
        }

        //
        // Returns the next block that will be added to the path (given the current vertex)
        // We are passed which block list to look at with the bool variable fromFirstPath
        private List<int> getNextBlock(Dictionary<int, int> edges, List<int>[] fPathBlocks, List<List<int>> sPathBlocks, bool fromFirstPath, int vertex)
        {
            List<int> blk = fromFirstPath ? getNextBlock(edges, fPathBlocks, vertex) : getNextBlock(edges, sPathBlocks, vertex);

            if (blk == null) return null;

            // We don't want to modify the block, just in case
            blk = new List<int>(blk);

            // Remove the first value, it is redundant / the edge we are removing
            blk.Remove(0);

            return blk;
        }

        //
        // Given two paths, first and second, combine the paths removing the common edges returning newFirst and newSecond
        //
        private int combinePaths(List<int> first, List<int> second, int fLength, int sLength,
                List<int> newFirst, List<int> newSecond)
        {
            int weightToSubtract = 0; // Weight of the common edges we will subtract from the actual length of the shortest path

            List<List<int>> secondPathBlocks = new List<List<int>>();
            List<int> currentBlock = new List<int>();
            Dictionary<int, int> reversedEdges = new Dictionary<int, int>();
            for (int s = 0; s < second.Count - 1; s++)
            {
                currentBlock.Add(second.ElementAt(s));

                if (graph.IsReversed(second.ElementAt(s), second.ElementAt(s + 1)))
                {
                    // Save the reversed edge to create blocks for the first path
                    reversedEdges.Add(second.ElementAt(s + 1), second.ElementAt(s));

                    // Add this block to the list of blocks
                    secondPathBlocks.Add(currentBlock);

                    // Create a new block
                    currentBlock = new List<int>();

                    // The weight of the edges we will remove from the path
                    weightToSubtract += graph.GetWeight(second.ElementAt(s), second.ElementAt(s + 1));
                }
            }
            // Add the last node to the last block
            currentBlock.Add(second.ElementAt(second.Count - 1));
            // Add the last block to the list
            secondPathBlocks.Add(currentBlock);

            if (DEBUG) Debug.WriteLine("Number of Reversals: " + reversedEdges.Count);

            // There are no shared edges
            if (!reversedEdges.Any())
            {
                newFirst.AddRange(first);
                newSecond.AddRange(second);
                return fLength + sLength;
            }

            List<int>[] firstPathBlks = new List<int>[reversedEdges.Count + 1];
            currentBlock = new List<int>();
            int blockCount = 0;
            for (int f = 0; f < first.Count - 1; f++)
            {
                // The edges will be reversed from what they were
                int fromVertex = first.ElementAt(f);
                int toVertex = first.ElementAt(f + 1);

                currentBlock.Add(first.ElementAt(f));

                // We found a reversed edge
                if (reversedEdges.ContainsKey(fromVertex))
                {
                    // Add this block to the list of blocks
                    firstPathBlks[blockCount++] = currentBlock;

                    // Create a new block
                    currentBlock = new List<int>();
                }
            }
            // Add the goal node to the last block
            currentBlock.Add(first.ElementAt(first.Count - 1));
            firstPathBlks[blockCount++] = currentBlock;

            //
            // Combine the blocks into two distinct paths
            //
            // Do so for the first path, initially
            int actualLastVertex = first.ElementAt(first.Count - 1);

            // Start the first list
            newFirst.AddRange(firstPathBlks[0]);
            bool fromFirstPath = true;

            // Loop while we do not have an entire path
            int currentLastVertex = newFirst.ElementAt(newFirst.Count - 1);
            while (currentLastVertex != actualLastVertex)
            {
                fromFirstPath = !fromFirstPath;
                newFirst.AddRange(getNextBlock(reversedEdges, firstPathBlks, secondPathBlocks, fromFirstPath, currentLastVertex));
                currentLastVertex = newFirst.ElementAt(newFirst.Count - 1);
            }

            // Start the second list
            newSecond.AddRange(secondPathBlocks.ElementAt(0));
            fromFirstPath = false;

            // Loop while we do not have an entire path
            currentLastVertex = newSecond.ElementAt(newSecond.Count - 1);
            while (currentLastVertex != actualLastVertex)
            {
                fromFirstPath = !fromFirstPath;
                newSecond.AddRange(getNextBlock(reversedEdges, firstPathBlks, secondPathBlocks, fromFirstPath, currentLastVertex));
                currentLastVertex = newSecond.ElementAt(newSecond.Count - 1);
            }

            return fLength + sLength - 2 * weightToSubtract;
        }

        //
        // Given a vertex-disjoint path, return the condensed path by removing the x' to x'' expansions
        // The resultant path is composed with respect to the original PathGraph (hence, division by 2) 
        //
        private void condensePath(List<int> path, List<int> condensedPath)
        {
            condensedPath.Clear();

            // Traverse the path, except for the last vertex (due to index out of bounds problem in the loop)
            for (int i = 0; i < path.Count - 1; i++)
            {
                // If x' and x'' are part of the path, take only x' / 2
                if (path.ElementAt(i) / 2 != path.ElementAt(i + 1) / 2) // && graph.getWeight(path.ElementAt(i), path.ElementAt(i+1)) == 0))
                {
                    condensedPath.Add(path.ElementAt(i) / 2);
                }
            }

            // Pick up the last vertex
            condensedPath.Add(path.ElementAt(path.Count - 1) / 2);
        }

        //
        // Update the start and goal nodes for vertex-disjoint analysis
        //
        public void SetStartGoalNodes(int start, int goal)
        {
            startNode = start;
            goalNode = goal;
        }

        //
        // Update the start and goal nodes for vertex-disjoint analysis
        //
        private void updateStartGoalNodes(int start, int goal)
        {
            startNode = 2 * (start - 1);
            goalNode = 2 * (goal - 1) + 1;
        }

        //
        // Find the dual-shortest paths of the PathGraph; implements Suurballe's algorithm
        //
        public long dualShortestPath(int start, int goal, List<int> actualPath1, List<int> actualPath2)
        {
            actualPath1.Clear();
            actualPath2.Clear();

            // Are the given start and goal nodes valid?
            if (start <= 0 || goal > graph.NumVertices() || goal <= 0 || start > graph.NumVertices())
            {
                Debug.WriteLine("Start Node or Goal Node specified is invalid.");
                return -1;
            }

            // Create a new PathGraph such that every node is duplicated and split into x' and x''
            // Incoming nodes go to x', outgoing edges go from x''
            // The weight between x' and x'' is zero 0
            graph.InduceVertexDisjoint();
            updateStartGoalNodes(start, goal);

            //
            // Find the first shortest path using Dijkstra's algorithm
            //
            List<int> firstPath = new List<int>();
            int firstLength = Dijkstra(firstPath);

            // Check if a path existed
            if (firstLength == -1)
            {
                Debug.WriteLine("No first path was found: (" + start + ", " + goal + ")");
                return -1;
            }

            if (DEBUG) Debug.WriteLine("First Path: with length (" + firstLength + ")");

            //
            // Reverse the used edges
            //
            reverseShortestPathEdges(firstPath);

            //
            // Find the second shortest path using Dijkstra's algorithm on the PathGraph with reversed edges
            //
            List<int> secondPath = new List<int>();
            int secondLength = Dijkstra(secondPath);

            // Check if a path existed
            if (secondLength == -1)
            {
                Debug.WriteLine("No second path was found: (" + start + ", " + goal + ")");
                return -1;
            }

            if (DEBUG) Debug.WriteLine("Second Path: with length (" + secondLength + ")");

            //
            // Compare the paths looking for common edges
            // Calculate the length of the dual-shortest paths subtracting the common edges
            // Split the paths to creates two vertex-disjoint paths
            //
            List<int> tempPath1 = new List<int>();
            List<int> tempPath2 = new List<int>();
            int combinedPathLength = combinePaths(firstPath, secondPath, firstLength, secondLength, tempPath1, tempPath2);

            //
            // Condense the path back down from x'->x'' to x
            //
            condensePath(tempPath1, actualPath1);
            condensePath(tempPath2, actualPath2);

            return combinedPathLength;
        }
    }
}
