using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Hypergraph
{
    //
    // Provides representation of a predecessor edge
    //
    public class PebblerTransposeHyperEdge
    {
        public List<int> targetNodes;
        public int source;

        public bool visited;

        public PebblerTransposeHyperEdge(int src, List<int> targets)
        {
            targetNodes = targets;
            source = src;
            visited = false;
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
