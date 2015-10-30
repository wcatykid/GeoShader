using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.Hypergraph
{
    //
    // Provides representation of a predecessor edge
    //
    public class TransposeHyperEdge<A>
    {
        // Allows us to note how the edge was derived
        public A annotation { get; private set; }

        public List<int> targetNodes;
        public int source;
        public bool visited;

        public TransposeHyperEdge(int src, List<int> targets, A annot)
        {
            targetNodes = targets;
            source = src;
            visited = false;
            annotation = annot;
        }

        public PebblerTransposeHyperEdge CreatePebblerTransposeEdge()
        {
            return new PebblerTransposeHyperEdge(source, targetNodes);
        }

        public override string ToString()
        {
            String retS = source + " -> { ";
            foreach (int node in targetNodes)
            {
                retS += node + ",";
            }
            if (targetNodes.Count != 0) retS = retS.Substring(0, retS.Length - 1);
            retS += " } ";
            return retS;
        }
    }
}
