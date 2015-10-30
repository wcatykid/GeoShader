using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // This class represents both forward and backward edges in the hypergraph.
    //
    public class HyperEdge<A>
    {
        // Allows us to note how the edge was derived
        public A annotation { get; private set; }
        
        public List<int> sourceNodes;
        public int targetNode;

        public HyperEdge(List<int> src, int target, A annot)
        {
            sourceNodes = src;
            targetNode = target;
            annotation = annot;

            if (src.Contains(target))
            {
                throw new ArgumentException("There exists a direct cycle in a hyperedge: " + this);
            }
        }

        // The source nodes and target must be the same for equality.
        public override bool Equals(object obj)
        {
            HyperEdge<A> thatEdge = obj as HyperEdge<A>;
            if (thatEdge == null) return false;
            foreach (int src in sourceNodes)
            {
                if (!thatEdge.sourceNodes.Contains(src)) return false;
            }
            return targetNode == thatEdge.targetNode;
        }

        //
        // This is an equals method by only providing the many-to-one mapping that defines an edge.
        //
        public bool DefinesEdge(List<int> antecedent, int consequent)
        {
            foreach (int ante in antecedent)
            {
                if (!sourceNodes.Contains(ante)) return false;
            }

            return targetNode == consequent;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            String retS = " { ";
            foreach (int node in sourceNodes)
            {
                retS += node + ",";
            }
            if (sourceNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += " } -> " + targetNode;
            return retS;
        }
    }
}
