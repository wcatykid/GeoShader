using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // The goal is three-fold in this class.
    //   (1) Provides functionality to create a hypergraph: adding nodes and edges
    //   (2) Convert all clauses to an integer hypergraph representation.
    //   (3) Provide functionality to explore the hypergraph.
    //
    public class Hypergraph<T, A>
    {
        // The main graph data structure
        public List<HyperNode<T, A>> vertices { get; private set; }

        public Hypergraph()
        {
            vertices = new List<HyperNode<T, A>>();
            edgeCount = 0;
        }

        public Hypergraph(int capacity)
        {
            vertices = new List<HyperNode<T, A>>(capacity);
            edgeCount = 0;
        }


        public int Size() { return vertices.Count; }
        private int edgeCount;
        public int EdgeCount() { return edgeCount; }

        //
        // Integer-based representation of the main hypergraph
        //
        public PebblerHypergraph<T, A> GetPebblerHypergraph()
        {
            //
            // Strictly create the nodes
            //
            PebblerHyperNode<T, A>[] pebblerNodes = new PebblerHyperNode<T, A>[vertices.Count];
            for (int v = 0; v < vertices.Count; v++)
            {
                pebblerNodes[v] = vertices[v].CreatePebblerNode();
            }

            //
            // Non-redundantly create all hyperedges
            //
            for (int v = 0; v < vertices.Count; v++)
            {
                foreach (HyperEdge<A> edge in vertices[v].edges)
                {
                    // Only add once to all nodes when this is the 'minimum' source node
                    if (v == edge.sourceNodes.Min())
                    {
                        PebblerHyperEdge<A> newEdge = new PebblerHyperEdge<A>(edge.sourceNodes, edge.targetNode, edge.annotation);
                        foreach (int src in edge.sourceNodes)
                        {
                            pebblerNodes[src].AddEdge(newEdge);
                        }
                    }
                }
            }

            return new PebblerHypergraph<T, A>(pebblerNodes);
        }

        public List<Strengthened> GetStrengthenedNodes(List<int> indices)
        {
            List<Strengthened> strengList = new List<Strengthened>();

            foreach (int index in indices)
            {
                if (vertices[index].data is Strengthened) strengList.Add(vertices[index].data as Strengthened);
            }

            return strengList;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        private int ConvertToLocalIntegerIndex(T inputData)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].data.Equals(inputData))
                {
                    return i;
                }
            }

            return -1;
        }

        //
        // Return the index of the given node
        //
        public int GetNodeIndex(T inputData)
        {
            return ConvertToLocalIntegerIndex(inputData);
        }

        //
        // Return the stored node in the graph
        //
        public T GetNode(int id)
        {
            if (id < 0 || id > vertices.Count)
            {
                throw new ArgumentException("Unexpected id in hypergraph node access: " + id);
            }

            return vertices[id].data;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public bool HasNode(T inputData)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                if (vertex.data.Equals(inputData)) return true;
            }

            return false;
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public T GetNode(T inputData)
        {
            foreach (HyperNode<T, A> vertex in vertices)
            {
                if (vertex.data.Equals(inputData)) return vertex.data;
            }

            return default(T);
        }

        //
        // Check if the graph contains this specific grounded clause
        //
        public bool AddNode(T inputData)
        {
            if (!HasNode(inputData))
            {
                vertices.Add(new HyperNode<T, A>(inputData, vertices.Count)); // <data, id>
                return true;
            }

            return false;
        }

        //
        // Is this edge in the graph (using local, integer-based information)
        //
        private bool HasLocalEdge(List<int> antecedent, int consequent)
        {
            foreach (int ante in antecedent)
            {
                if (ante < 0 || ante > vertices.Count) throw new ArgumentException("Index of bounds on local edge: " + ante);
            }
  
            if (consequent < 0 || consequent > vertices.Count) throw new ArgumentException("Index of bounds on local edge: " + consequent);


            foreach (HyperNode<T, A> vertex in vertices)
            {
                foreach (HyperEdge<A> edge in vertex.edges)
                {
                    if (edge.DefinesEdge(antecedent, consequent)) return true;
                }
            }

            return false;
        }

        //
        // Check if the graph contains an edge defined by a many to one clause mapping
        //
        public bool HasEdge(List<T> antecedent, T consequent)
        {
            KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

            return HasLocalEdge(local.Key, local.Value);
        }

        //
        // Convert information to local, integer-based representation
        //
        private KeyValuePair<List<int>, int> ConvertToLocal(List<T> antecedent, T consequent)
        {
            List<int> localAnte = new List<int>();

            foreach (T ante in antecedent)
            {
                int index = ConvertToLocalIntegerIndex(ante);

                if (index == -1)
                {
                    throw new ArgumentException("Source node not found as a hypergraph node: " + ante);
                }

                localAnte.Add(index);
            }

            int localConsequent = ConvertToLocalIntegerIndex(consequent);

            if (localConsequent == -1)
            {
                throw new ArgumentException("Target value referenced not found as a hypergraph node: " + consequent);
            }

            return new KeyValuePair<List<int>, int>(localAnte, localConsequent);
        }

        //
        // Adding an edge to the graph
        //
        public bool AddEdge(List<T> antecedent, T consequent, A annotation)
        {
            //
            // Add a local representaiton of this edge to each node in which it is applicable
            //
            if (HasEdge(antecedent, consequent)) return false;

            KeyValuePair<List<int>, int> local = ConvertToLocal(antecedent, consequent);

            HyperEdge<A> edge = new HyperEdge<A>(local.Key, local.Value, annotation);

//System.Diagnostics.Debug.WriteLine("Adding edge: " + edge.ToString());

            foreach (int src in local.Key)
            {
                vertices[src].AddEdge(edge);
            }

            // Add this as a target edge to the target node.
            vertices[local.Value].AddTargetEdge(edge);
            edgeCount++;

            return true;
        }

        //
        // Adding an edge to the graph based on known indices.
        //
        public bool AddIndexEdge(List<int> antecedent, int consequent, A annotation)
        {
            if (HasLocalEdge(antecedent, consequent)) return false;

            HyperEdge<A> edge = new HyperEdge<A>(antecedent, consequent, annotation);

            foreach (int src in antecedent)
            {
                vertices[src].AddEdge(edge);
            }

            // Add this as a target edge to the target node.
            vertices[consequent].AddTargetEdge(edge);
            edgeCount++;

            return true;
        }

        public void DebugDumpClauses()
        {
            Debug.WriteLine("All Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
            }


            Debug.WriteLine("\nEdges: ");
            edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].edges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].edges)
                    {
                        edgeStr.Append(" { ");
                        foreach (int s in edge.sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Append("} -> " + edge.targetNode + ", ");
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }
            Debug.WriteLine(edgeStr);
        }

        public void DumpNonEquationClauses()
        {
            Debug.WriteLine("Non-Equation Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (!(vertices[v].data is Equation))
                {
                    Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
                }
            }
        }

        public void DumpEquationClauses()
        {
            Debug.WriteLine("Equation Clauses:\n");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].data is Equation)
                {
                    Debug.WriteLine(edgeStr + " " + v + " " + vertices[v].data.ToString());
                }
            }
        }

        public void DumpClauseForwardEdges()
        {
            Debug.WriteLine("\n Forward Edges: ");

            StringBuilder edgeStr = new StringBuilder();
            for (int v = 0; v < vertices.Count; v++)
            {
                if (vertices[v].edges.Any())
                {
                    edgeStr.Append(v + ": {");
                    foreach (HyperEdge<A> edge in vertices[v].edges)
                    {
                        edgeStr.Append(" { ");
                        foreach (int s in edge.sourceNodes)
                        {
                            edgeStr.Append(s + " ");
                        }
                        edgeStr.Append("} -> " + edge.targetNode + ", ");
                    }
                    edgeStr.Remove(edgeStr.Length - 2, 2);
                    edgeStr.Append(" }\n");
                }
            }
            Debug.WriteLine(edgeStr);
        }
    }
}