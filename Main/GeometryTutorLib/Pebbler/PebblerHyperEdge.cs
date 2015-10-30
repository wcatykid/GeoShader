using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Pebbler
{
    //
    // This class represents a general hyperedge; both forward and backward edges in the hypergraph.
    //
    public class PebblerHyperEdge<A>
    {
        public List<int> sourceNodes { get; private set; }
        public int targetNode { get; private set; }

        // The original edge annotation purely for reference.
        public A annotation { get; private set; }

        // Contains all source nodes that have been pebbled: for each source node,
        // there is a 'standard edge' that must be pebbled
        public List<int> sourcePebbles;
        //public PebblerColorType pebbleColor;

        // Whether the node has been pebbled or not.
        public bool pebbled;

        public PebblerHyperEdge(List<int> src, int target, A annotation)
        {
            this.annotation = annotation;
            sourceNodes = src;
            sourcePebbles = new List<int>(); // If empty, we assume all false (not pebbled)
            targetNode = target;
            pebbled = false;
        }

        public bool IsFullyPebbled()
        {
            foreach (int srcNode in sourceNodes)
            {
                if (!sourcePebbles.Contains(srcNode)) return false;
            }

            return sourceNodes.Count == sourcePebbles.Count;
        }

        //public void SetColor(PebblerColorType color)
        //{
        //    pebbleColor = color;
        //}

        // The source nodes and target must be the same for equality.
        public override bool Equals(object obj)
        {
            PebblerHyperEdge<A> thatEdge = obj as PebblerHyperEdge<A>;
            if (thatEdge == null) return false;
            foreach (int src in sourceNodes)
            {
                if (!thatEdge.sourceNodes.Contains(src)) return false;
            }
            return targetNode == thatEdge.targetNode;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            String retS = " { ";
            foreach (int node in sourceNodes)
            {
                retS += node + ", ";
            }
            if (sourceNodes.Count != 0) retS = retS.Substring(0, retS.Length - 2);
            retS += " } -> " + targetNode;
            return retS;
        }
    }
}
