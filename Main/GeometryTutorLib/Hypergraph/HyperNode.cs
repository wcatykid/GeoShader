using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.GenericInstantiator;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    public class HyperNode<T, A>
    {
        public T data;
        public int id;

        // Edges in which this node is a source
        public List<HyperEdge<A>> edges;

        // Edges in which this node is the target
        public List<HyperEdge<A>> targetEdges;

        public void AddEdge(HyperEdge<A> edge)
        {
            edges.Add(edge);
        }

        public void AddTargetEdge(HyperEdge<A> edge)
        {
            if (edge.targetNode != id) throw new ArgumentException("Given node is not the target as advertised " + edge);

            targetEdges.Add(edge);
        }

        public HyperNode(T d, int i)
        {
            id = i;
            data = d;
           
            edges = new List<HyperEdge<A>>();
            targetEdges = new List<HyperEdge<A>>();
        }

        // Creating a shallow copy of this node as a pebbler node
        public PebblerHyperNode<T, A> CreatePebblerNode()
        {
            return new PebblerHyperNode<T, A>(data, id);
        }

        public override string ToString()
        {
            string retS = data.ToString() + "\t\t\t\t= { ";

            retS += id + "SuccE = { ";
            foreach (HyperEdge<A> edge in edges) { retS += edge.ToString() + ", "; }
            if (edges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += "} TargetE = { ";
            foreach (HyperEdge<A> edge in targetEdges) { retS += edge.ToString() + ", "; }
            if (targetEdges.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } }";

            return retS;
        }
    }
}
