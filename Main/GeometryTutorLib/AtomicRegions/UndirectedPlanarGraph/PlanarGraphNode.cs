using System.Collections.Generic;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph
{
    //
    // For atomic region identification
    //
    // public enum NodePointType { REAL, EXTENDED };

    public class PlanarGraphNode
    {
        public List<PlanarGraphEdge> edges { get; private set; }
        public GeometryTutorLib.ConcreteAST.Point thePoint { get; private set; }
        // public NodePointType type { get; private set; }


        public PlanarGraphNode(GeometryTutorLib.ConcreteAST.Point value) // , NodePointType t)
        {
            thePoint = value;
            //type = t;
            edges = new List<PlanarGraphEdge>();
        }

        //
        // Shallow copy constructor
        //
        public PlanarGraphNode(PlanarGraphNode thatNode)
        {
            thePoint = thatNode.thePoint;
            // type = thatNode.type;
            edges = new List<PlanarGraphEdge>();

            foreach (PlanarGraphEdge e in thatNode.edges)
            {
                edges.Add(new PlanarGraphEdge(e));
            }
        }

        public void AddEdge(GeometryTutorLib.ConcreteAST.Point targ, EdgeType type, double c, int initDegree)
        {
            edges.Add(new PlanarGraphEdge(targ, type, c, initDegree));
        }

        public PlanarGraphEdge GetEdge(GeometryTutorLib.ConcreteAST.Point targ)
        {
            int index = edges.IndexOf(new PlanarGraphEdge(targ));

            return index == -1 ? null : edges[index];
        }

        public void RemoveEdge(GeometryTutorLib.ConcreteAST.Point targetNode)
        {
            edges.Remove(new PlanarGraphEdge(targetNode));
        }

        public void MarkEdge(GeometryTutorLib.ConcreteAST.Point targetNode)
        {
            PlanarGraphEdge edge = GetEdge(targetNode);

            if (edge == null) return;

            edge.isCycle = true;
        }

        public bool IsCyclicEdge(GeometryTutorLib.ConcreteAST.Point targetNode)
        {
            PlanarGraphEdge edge = GetEdge(targetNode);

            if (edge == null) return false;

            return edge.isCycle;
        }

        public int NodeDegree()
        {
            return edges.Count;
        }

        public void Clear()
        {
            foreach (PlanarGraphEdge e in edges)
            {
                e.Clear();
            }
        }

        public override int GetHashCode() { return base.GetHashCode(); }
        //
        // Equality is only based on the point in the graph.
        //
        public override bool Equals(object obj)
        {
            PlanarGraphNode node = obj as PlanarGraphNode;
            if (node == null) return false;

            return this.thePoint.Equals(node.thePoint);
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append("<" + thePoint.ToString() + "  (" + edges.Count + "): ");
            foreach (PlanarGraphEdge edge in edges)
            {
                str.Append(edge.ToString() + "\t");
            }

            return str.ToString();
        }
    }
}
