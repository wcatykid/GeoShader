using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometryTutorLib.Pebbler
{
    //
    // Implements a multi-hashtable in which an entry may appear more than once in the table.
    // That is, a path contains potentially many source nodes. For each source node, we hash and add the path to the table.
    // Hence, a path with n source nodes is hashed n times; this allows for fast access
    // Collisions are thus handled by chaining
    //
    public class HyperEdgeMultiMap<A>
    {
        private readonly int TABLE_SIZE;
        private List<PebblerHyperEdge<A>>[] table;
        public int size { get; private set; }

        // The actual hypergraph for reference purposes only when adding edges (check for intrinsic)
        private Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;
        public void SetOriginalHypergraph(Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g) { graph = g; }

        // If the user specifies the size, we will never have to rehash
        public HyperEdgeMultiMap(int sz)
        {
            size = 0;
            TABLE_SIZE = sz;
            
            table = new List<PebblerHyperEdge<A>>[TABLE_SIZE];
        }

        //
        // Add the PebblerHyperEdge to all source node hash values
        //
        public void Put(PebblerHyperEdge<A> edge)
        {
            // Analyze the edge to determine if it is a mixed edge; all edges are
            // such that the target is greater than or less than all source nodes
            // Find the minimum non-intrinsic node (if it exists)
            edge.sourceNodes.Sort();
            int minSrc = edge.sourceNodes.Max();
            foreach (int src in edge.sourceNodes)
            {
                if (!graph.vertices[src].data.IsIntrinsic() || !graph.vertices[src].data.IsAxiomatic())
                {
                    minSrc = src;
                }
            }
            int maxSrc = edge.sourceNodes.Max();
            if (minSrc < edge.targetNode && edge.targetNode < maxSrc)
            {
                throw new ArgumentException("A mixed edge was pebbled as valid: " + edge);
            }

            long hashVal = (edge.targetNode % TABLE_SIZE);

            if (table[hashVal] == null)
            {
                table[hashVal] = new List<PebblerHyperEdge<A>>();
            }

            Utilities.AddUnique<PebblerHyperEdge<A>>(table[hashVal], edge);

            size++;
        }

        // Another option to acquire the pertinent problems
        public List<PebblerHyperEdge<A>> GetBasedOnGoal(int goalNodeIndex)
        {
            if (goalNodeIndex < 0 || goalNodeIndex >= TABLE_SIZE)
            {
                throw new ArgumentException("HyperEdgeMultimap::Get::key(" + goalNodeIndex + ")");
            }

            return table[goalNodeIndex];
        }

        public override string ToString()
        {
            String retS = "";

            for (int ell = 0; ell < TABLE_SIZE; ell++)
            {
                if (table[ell] != null)
                {
                    retS += ell + ":\n";
                    foreach (PebblerHyperEdge<A> PebblerHyperEdge in table[ell])
                    {
                        retS += PebblerHyperEdge.ToString() + "\n";
                    }
                }
            }

            return retS;
        }
    }
}