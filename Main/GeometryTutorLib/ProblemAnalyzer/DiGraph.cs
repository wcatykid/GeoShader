using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // Implements a basic directional graph (with no node information)
    //
    public class DiGraph
    {
        //
        // To implement Tajan's Strongly Connected Components (and cycles in the graph)
        //
        // Each node v is assigned a unique integer v.index, which numbers the nodes consecutively in the order
        // in which they are discovered. It also maintains a value v.lowlink that represents (roughly speaking)
        // the smallest index of any node known to be reachable from v, including v itself. Therefore v must be
        // left on the stack if v.lowlink < v.index, whereas v must be removed as the root of a strongly connected
        // component if v.lowlink == v.index. The value v.lowlink is computed during the depth-first search from v,
        // as this finds the nodes that are reachable from v.
        //
        //protected class Vertex
        //{
        //    public int node;
        //    public int lowLink;
        //    public int index;

        //    public Vertex(int n)
        //    {
        //        node = n;
        //        lowLink = -1;
        //        index = -1;
        //    }

        //    public Vertex(Vertex v)
        //    {
        //        node = v.node;
        //        lowLink = -1;
        //        index = -1;
        //    }

        //    public override bool Equals(object obj) { return this.node.Equals((obj as Vertex).node); }
        //    public override int GetHashCode() { return base.GetHashCode(); }
        //}

        //
        // The dictionary is a map: a node to all of its successors
        //
        protected Dictionary<int, List<int>> edgeMap;
        protected Dictionary<int, List<int>> transposeEdgeMap;
        protected int numEdges;
        protected List<int> vertices;
        protected List<List<int>> sccs; // Strongly Connected Components

        public DiGraph()
        {
            edgeMap = new Dictionary<int, List<int>>();
            transposeEdgeMap = new Dictionary<int, List<int>>();
            numEdges = 0;
            vertices = new List<int>();
            sccs = new List<List<int>>();
        }

        //
        // Make a shallow copy of this graph (all vertices and edges)
        //
        public DiGraph(DiGraph thatGraph)
        {
            edgeMap = new Dictionary<int, List<int>>();
            transposeEdgeMap = new Dictionary<int, List<int>>();
            numEdges = thatGraph.numEdges;
            vertices = new List<int>(thatGraph.vertices);

            // Copy the integer indices
            foreach (KeyValuePair<int, List<int>> pair in thatGraph.edgeMap)
            {
                edgeMap.Add(pair.Key, new List<int>(pair.Value));
            }

            // Copy the integer indices
            foreach (KeyValuePair<int, List<int>> pair in thatGraph.transposeEdgeMap)
            {
                transposeEdgeMap.Add(pair.Key, new List<int>(pair.Value));
            }

            sccs = GetStronglyConnectedComponents();
        }

        //
        // Adds a basic edge to the graph
        //
        public void AddEdge(int from, int to)
        {
            AddEdge(edgeMap, from, to);
            AddEdge(transposeEdgeMap, to, from);

            numEdges++;
        }

        // Adds an edge to a map of edges
        private void AddEdge(Dictionary<int, List<int>> givenEdges, int from, int to)
        {
            // This order needed because we want the goal node of the problem first
            if (!vertices.Contains(to)) vertices.Add(to);
            if (!vertices.Contains(from)) vertices.Add(from);

            List<int> fromDependencies;
            if (givenEdges.TryGetValue(from, out fromDependencies))
            {
                Utilities.AddUnique<int>(fromDependencies, to);
            }
            else
            {
                givenEdges.Add(from, Utilities.MakeList<int>(to));
            }
        }

        //
        // Adds a many-to-one hyperedge to the graph by adding all the individual edges
        //
        public void AddHyperEdge(List<int> fromList, int to)
        {
            foreach (int from in fromList)
            {
                this.AddEdge(from, to);
            }
        }

        //
        // Simple heuristic for which we may use graph minors to acquire isomorphisms
        //
        public int NumEdges()
        {
            return numEdges;
        }

        //
        // The depth of the graph is defined as being the length of the maximal path to the leaf nodes
        // We assume that this graph is a DAG.
        //
        // We use a DFS technique to determine maximality
        //
        // Since we have an implied 1-1 map between vertices and indices, we work on indices
        //
        public int GetLength()
        {
            // passing index 0, depth 1
            return GetLengthHelper(vertices[0], 1);
        }
        private int GetLengthHelper(int currentNode, int currentDepth)
        {
            int maxDepth = -1;

            // Get the edges from this node to traverse
            List<int> backwardEdges;

            // Get the transpose edges from this node (as we are traversing backward from the goal)
            // This node is a leaf if no edges: return the known depth
            if (!transposeEdgeMap.TryGetValue(currentNode, out backwardEdges)) return currentDepth;

            // Traverse the edges tracking the max depth
            foreach (int edge in backwardEdges)
            {
                int tempDepth = GetLengthHelper(edge, currentDepth + 1);
                if (maxDepth < tempDepth) maxDepth = tempDepth;
            }

            return maxDepth;
        }

        //
        // General graph traversal assuming a DAG; we start at the goal node and walk the transpose edges
        //
        // Since this is a DAG, width is defined as the spread of the graph (like a tree) 
        // Use a BFS technique where we continually update the levelWidth variable to ensure we know where the next level starts
        //
        // Since we have an implied 1-1 map between vertices and indices, we work on indices
        //
        public int GetWidth()
        {
            Queue<int> worklist = new Queue<int>();

            // Add the 'goal' node as a catalyst
            worklist.Enqueue(vertices[0]);

            // width for this level
            int currentLevelWidth = 1;

            // max width for the entire graph
            int maxLevelWidth = 1;

            // For verification purposes, we track the total number of nodes visited
            int sumOfAllWidths = 0;

            //
            // Traverse the entire graph
            //
            while (worklist.Any())
            { 
                // Traverse an entire level
                int currentLevelAccumulator = 0;
                for (int ell = 0; ell < currentLevelWidth; ell++)
                {
                    // Get the next node
                    int currentNode = worklist.Dequeue();

                    // Get the edges from this node to traverse
                    List<int> backwardEdges;
                    if (transposeEdgeMap.TryGetValue(currentNode, out backwardEdges))
                    {
                        // Add all targets to the worklist
                        backwardEdges.ForEach(edgeIndex => worklist.Enqueue(edgeIndex));
                    }
                    currentLevelAccumulator += backwardEdges == null ? 0 : backwardEdges.Count;
                }

                // Completed a level; check width values, including if we have a new maxWidth
                if (currentLevelWidth > maxLevelWidth) maxLevelWidth = currentLevelWidth;

                sumOfAllWidths += currentLevelWidth;

                // Next level's number of nodes
                currentLevelWidth = currentLevelAccumulator;
            }

            if (sumOfAllWidths < vertices.Count)
            {
                throw new Exception("Error in width determination: Did not traverse all nodes!");
            }

            return maxLevelWidth;
        }

        public bool ContainsCycle()
        {
            // Update the SCCs
            sccs = GetStronglyConnectedComponents();

            // Since all strongly connected components should contain one node, there should be the exact same number of SCCs as vertices.
            return sccs.Count != vertices.Count;
        }

        public string GetStronglyConnectedComponentDump()
        {
            sccs = GetStronglyConnectedComponents();

            StringBuilder str = new StringBuilder();
            str.AppendLine("SCCs: ");
            int counter = 0;
            foreach (List<int> scc in sccs)
            {
                str.Append("\t" + (counter++) + ": ");
                foreach (int v in scc)
                {
                    str.Append(v + " ");
                }
                str.AppendLine("");
            }

            return str.ToString();
        }

        //
        // Use Tarjan's Algorithm to acquire the Strongly Connected Components of a given directed graph
        //
        private List<List<int>> GetStronglyConnectedComponents()
        {
            List<List<int>> stronglyConnectedComponents = new List<List<int>>();
            Stack<int> workStack = new Stack<int>();
            int overallIndex = 0;

            int[] sccIndex = new int[vertices.Count];
            int[] lowLink = new int[vertices.Count];

            // Init to -1 for the tracking data for Tarjan's
            for (int i = 0; i < vertices.Count; i++)
            {
                sccIndex[i] = -1;
                lowLink[i] = -1;
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                if (sccIndex[i] < 0)
                {
                    StronglyConnectedSub(vertices[i], i, overallIndex, workStack, stronglyConnectedComponents, sccIndex, lowLink);
                }
            }

            return stronglyConnectedComponents;
        }
        private void StronglyConnectedSub(int vertex, int vertexIndex, int overallIndex, Stack<int> workStack, List<List<int>> stronglyConnectedComponents, int[] sccIndex, int[] lowLink)
        {
            //
            // Define the current vertex reachability
            //
            sccIndex[vertexIndex] = overallIndex;
            lowLink[vertexIndex] = overallIndex;
            overallIndex++;

            workStack.Push(vertex);

            //
            // Pursue all dependencies
            //
            List<int> dependencies;
            if (edgeMap.TryGetValue(vertex, out dependencies))
            {
                //
                // Follow each edge in depth-first manner
                //
                foreach (int dependent in dependencies)
                {
                    int dependentIndex = vertices.IndexOf(dependent);

                    if (sccIndex[dependentIndex] < 0)
                    {
                        StronglyConnectedSub(dependent, dependentIndex, overallIndex, workStack, stronglyConnectedComponents, sccIndex, lowLink);
                        lowLink[vertexIndex] = Math.Min(lowLink[vertexIndex], lowLink[dependentIndex]);
                    }
                    else if (workStack.Contains(dependent))
                    {
                        lowLink[vertexIndex] = Math.Min(lowLink[vertexIndex], lowLink[dependentIndex]);
                    }
                }
            }

            if (lowLink[vertexIndex] == sccIndex[vertexIndex])
            {
                List<int> scc = new List<int>();
                int w;
                do
                {
                    w = workStack.Pop();
                    scc.Add(w);
                } while (!vertex.Equals(w));

                stronglyConnectedComponents.Add(scc);
            }
        }

        //
        // The algorithm loops through each node of the graph, in an arbitrary order, initiating a depth-first search that
        // terminates when it hits any node that has already been visited since the beginning of the topological sort
        //
        // L ← Empty list that will contain the sorted nodes
        // while there are unmarked nodes do
        //    select an unmarked node n
        //    visit(n) 
        public List<int> TopologicalSort()
        {
            // L ← Empty list that will contain the sorted elements
            List<int> L = new List<int>();

            // Unmarked
            List<int> unmarked = new List<int>();
            vertices.ForEach(vertex => unmarked.Add(vertex));

            // Temporarily marked
            List<int> tempMarked = new List<int>();

            // Permanently marked
            List<int> marked = new List<int>();

            while (unmarked.Any())
            {
                // remove a node n from unmarked
                int n = unmarked[0];
                unmarked.RemoveAt(0);

                Visit(n, unmarked, tempMarked, marked, L);
            }

            return L;
        }

        // function visit(node n)
        //    if n has a temporary mark then stop (not a DAG)
        //    if n is not marked (i.e. has not been visited yet) then
        //        mark n temporarily
        //        for each node m with an edge from n to m do
        //            visit(m)
        //        mark n permanently
        //        add n to head of L
        private void Visit(int n, List<int> unmarked, List<int> tempMarked, List<int> marked, List<int> L)
        {
            // if n has a temporary mark then stop (not a DAG)
            if (tempMarked.Contains(n)) return;

            // if n is not marked (i.e. has not been visited yet) then
            if (!marked.Contains(n))
            {
                // mark n temporarily
                tempMarked.Add(n);

                // for each node m with an edge from n to m do
                List<int> dependencies;
                if (edgeMap.TryGetValue(n, out dependencies))
                {
                    foreach (int dependent in dependencies)
                    {
                        Visit(dependent, unmarked, tempMarked, marked, L);
                    }
                }

                // mark n permanently
                marked.Add(n);
                unmarked.Remove(n);
                
                // add n to head of L
                L.Add(n);
            }
        }
    }
}