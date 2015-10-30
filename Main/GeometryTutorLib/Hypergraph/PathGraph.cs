using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.Hypergraph;

namespace GeometryTutorLib.Hypergraph
{
    //
    // A simpler graph representation in which all nodes are reachable and all hyperedges are valid
    //
    public class PathGraph
    {
        //
        // Are all nodes referred to in the hyperedges?
        //
        private bool ValidateEdgesAndNodes(List<int> nodes, List<Hypergraph.HyperEdge> edges)
        {
            foreach (Hypergraph.HyperEdge edge in edges)
            {
                foreach (int src in edge.sourceNodes)
                {
                    if (!nodes.Contains(src))
                    {
                        Debug.WriteLine("ValidateEdgesAndNodes: Expected to find node (" + src + ") in edges. Did not.");
                        return false;
                    }
                }
            }
            return true;
        }

        //
        // Basic Graph verirication and construction which requires the user specify the number of vertices.
        //
        public PathGraph(int originalSz, List<int> ns, List<Hypergraph.HyperEdge> es)
        {
            if (!ValidateEdgesAndNodes(ns, es)) return;

            numVerts = originalSz;
            vertexList = new List<Edge>[numVerts];

            //
            // Construct the graph by creating all the edges: hyperedges -> edges
            //
            foreach (Hypergraph.HyperEdge edge in es)
            {
                foreach (int sourceNode in edge.sourceNodes)
                {
                    // (from, to, weight = # source nodes, reversed?
                    AddEdge(sourceNode, edge.targetNode, edge.sourceNodes.Count, false, edge);
                }
            }
        }

        //
        // This graph represents a basic graph.
        //   A graph is composed of:
        //     (1) Array of Linkedlists
        //     (2) Each linked list will contain Edge objects specifying (u, v, w) where u and v are vertices and w is the weight of the edge.
        //         It is not necessary to maintain u in the Edge object, but for extensibility purposes, it is acceptable overhead.
        //
        //   An adjacency matrix will be extremely sparse results in slow processing (determining neighbor nodes)
        //

            // The number of vertices of the graph; this will change if vertex-disjoint processing is employed
            private int numVerts;
            public int NumVertices() { return numVerts; }

            // The array of Linked Lists of Edges
            private List<Edge>[] vertexList;

            //
            // A simple, private Edge class specifying an edge (u,v) with weight w
            //
            private class Edge
            {
                public int from;
                public int to;
                public int weight;
                public bool reversed;
                public Hypergraph.HyperEdge hyperedge;

                public Edge(int f, int t, int w, bool rev, Hypergraph.HyperEdge he) { from = f; to = t; weight = w; reversed = rev; hyperedge = he; }
                public Edge(Edge e) { from = e.from; to = e.to; weight = e.weight; reversed = e.reversed; hyperedge = e.hyperedge; } // copy constructor

                public override bool Equals(object obj)
                {
                    Edge e = obj as Edge;
                    if (e == null) return false;
                    return from == e.from && to == e.to;
                }
            }

            //
            // A copy constructor for a graph (so that we can perform multiple searches)
            // since the graph may be modified from one iteration to the next.
            //
            public PathGraph(PathGraph another)
            {
                numVerts = another.numVerts;

                //
                // Create and copy all vertex-edge information
                //
                this.vertexList = new List<Edge>[numVerts];

                for (int i = 0; i < vertexList.Length; i++)
                {
                    if (another.vertexList[i] != null)
                    {
                        this.vertexList[i] = new List<Edge>();
                        foreach (Edge e in another.vertexList[i])
                        {
                            this.vertexList[i].Add(new Edge(e));
                        }
                    }
                }
            }

            //
            // Adds the specified edge to the graph; if the List does not exist, we create it and add the Edge 
            //
            public void AddEdge(int u, int v, int weight, bool rev, Hypergraph.HyperEdge hyperedge)
            {
                // create the list, if needed
                if (vertexList[u] == null)
                {
                    vertexList[u] = new List<Edge>();
                }

                // Do not allow duplicate edges
                Utilities.AddUnique<Edge>(vertexList[u], new Edge(u, v, weight, rev, hyperedge));
            }

            //
            // Sets Edge (u,v) to weight w
            //
            public void SetEdge(int u, int v, int w, bool rev)
            {
                //
                // Check to see if the edge exists already
                //
                if (vertexList[u] == null)
                {
                    vertexList[u] = new List<Edge>();
                }

                foreach (Edge e in vertexList[u])
                {
                    if (e.to == v)
                    {
                        e.weight = w;
                        e.reversed = rev;
                        return;
                    }
                }

                // Add the edge like normal if it did not exist already
                vertexList[u].Add(new Edge(u, v, w, rev, null));
            }

            //
            // Removes Edge (u,v)
            //
            public void ResetEdge(int u, int v)
            {
                for (int i = 0; i < vertexList[u].Count; i++)
                {
                    Edge e = vertexList[u].ElementAt(i);
                    if (e.from == u && e.to == v)
                    {
                        vertexList[u].RemoveAt(i);
                        return;
                    }
                }
            }

            //
            // Sets Edge (u,v) to NIL; sets to (v, u) to weight w
            //
            public void ReverseEdge(int u, int v)
            {
                int weight = GetWeight(u, v);
                ResetEdge(u, v);
                AddEdge(v, u, weight, true, null);
            }

            //
            // Creates a new matrix by splitting all nodes (save start and goal nodes)
            //
            // The example that follows indicates start and goal nodes being default values.
            //
            //  original matrix     new matrix
            //   0 1 2 3                             0i 0o 1i 1o 2i 2o 3i 3o
            // 0                   0(start) 
            // 1                   0(start-output)
            // 2                   1(1i)
            // 3                   2(1o)
            //                     3(2i)
            //                     4(2o)
            //                     5(goal-input)
            //                     6(goal-output)
            public void InduceVertexDisjoint()
            {
                // Double the number of vertices: x becomes x' -> x''
                numVerts *= 2;

                // Create the new Vertex List
                List<Edge>[] newVertexList = new List<Edge>[numVerts];

                //
                // Traverse old edge set to make all nodes x split into x' and x''
                //
                for (int u = 0; u < vertexList.Length; u++)
                {
                    if (vertexList[u] != null)
                    {
                        newVertexList[2 * u + 1] = new List<Edge>();

                        foreach (Edge e in vertexList[u])
                        {
                            newVertexList[2 * u + 1].Add(new Edge(2 * u + 1, 2 * e.to, e.weight, false, e.hyperedge));
                        }
                    }
                }

                //
                // Mark the connections from x' to x'' with weight 0
                //
                for (int u = 0; u < vertexList.Length; u++)
                {
                    if (newVertexList[2 * u] == null)
                    {
                        newVertexList[2 * u] = new List<Edge>();
                        newVertexList[2 * u].Add(new Edge(2 * u, 2 * u + 1, 0, false, null));
                    }
                }

                // Overwrite the old Vertex list
                vertexList = newVertexList;
            }

            //
            // Returns a list of all vertices reachable from the given node
            // 
            public List<int> AdjacentNodes(int node)
            {
                List<int> adj = new List<int>();

                // Traverse and add the neighbors
                if (vertexList[node] != null)
                {
                    foreach (Edge e in vertexList[node])
                    {
                        adj.Add(e.to);
                    }
                }

                return adj;
            }

            //
            // Returns the weight of Edge (u, v)
            // 
            public int GetWeight(int u, int v)
            {
                foreach (Edge e in vertexList[u])
                {
                    if (e.from == u && e.to == v) return e.weight;
                }

                return -1;
            }

            //
            // Returns whether this was a reversed edge (u, v) -> (v, u) in the second graph
            // 
            public bool IsReversed(int u, int v)
            {
                if (vertexList[u] == null)
                {
                    return false;
                }

                foreach (Edge e in vertexList[u])
                {
                    if (e.from == u && e.to == v) return e.reversed;
                }

                return false;
            }

            //
            // Simple toString method for debugging purposes
            // Caveat: output is voluminous
            //
            public override string ToString()
            {
                String retS = "";

                // Traverse all vertices
                for (int r = 0; r < vertexList.Length; r++)
                {
                    // Add an indicator value 
                    retS += r + ":\t";

                    // Traverse the edges
                    if (vertexList[r] != null)
                    {
                        foreach (Edge e in vertexList[r])
                        {
                            retS += "(" + e.from + ", " + e.to + ", " + e.weight + ")\t";
                        }
                    }
                    retS += "\n";
                }

                return retS;
            }
    }
}