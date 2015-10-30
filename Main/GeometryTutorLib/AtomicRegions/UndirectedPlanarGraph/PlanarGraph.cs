using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph
{
    public class PlanarGraph
    {
        public List<PlanarGraphNode> nodes { get; private set; }

        public PlanarGraph()
        {
            nodes = new List<PlanarGraphNode>();
        }

        //
        // Shallow copy constructor
        //
        public PlanarGraph(PlanarGraph thatG) : this()
        {
            foreach (PlanarGraphNode node in thatG.nodes)
            {
                nodes.Add(new PlanarGraphNode(node));
            }
        }

        public void AddNode(Point value) // , NodePointType type)
        {
            // Avoid redundant additions.
            if (IndexOf(value) != -1) return;

            AddNode(new PlanarGraphNode(value)); // , type));
        }

        private void AddNode(PlanarGraphNode node)
        {
            nodes.Add(node);
        }

        public int IndexOf(Point pt)
        {
            return nodes.IndexOf(new PlanarGraphNode(pt)); // , NodePointType.REAL));
        }

        //
        // Determine the new, updated edge type.
        //    public enum EdgeType { REAL_ARC, REAL_SEGMENT, REAL_DUAL, EXTENDED_SEGMENT };
        private EdgeType UpdateEdge(EdgeType oldType, EdgeType newType)
        {
            if (oldType == EdgeType.REAL_SEGMENT && newType == EdgeType.REAL_SEGMENT)
            {
                return EdgeType.REAL_SEGMENT;
                //throw new ArgumentException("Cannot have two edges defined by a real segment.");
            }

            if (oldType == EdgeType.EXTENDED_SEGMENT || newType == EdgeType.EXTENDED_SEGMENT)
            {
                return EdgeType.EXTENDED_SEGMENT;
//                throw new ArgumentException("Cannot change an edge to / from an extended segment type.");
            }

            if (newType == EdgeType.REAL_DUAL)
            {
                return EdgeType.REAL_DUAL;

//                throw new ArgumentException("Cannot change an edge to be dual.");
            }

            // DUAL + ARC / SEGMENT = DUAL
            if (oldType == EdgeType.REAL_DUAL) return EdgeType.REAL_DUAL;

            // SEGMENT + ARC = DUAL
            if (oldType == EdgeType.REAL_SEGMENT && newType == EdgeType.REAL_ARC) return EdgeType.REAL_DUAL;

            // ARC + SEGMENT = DUAL
            if (oldType == EdgeType.REAL_ARC && newType == EdgeType.REAL_SEGMENT) return EdgeType.REAL_DUAL;

            // ARC + ARC = ARC
            if (oldType == EdgeType.REAL_ARC && newType == EdgeType.REAL_ARC) return EdgeType.REAL_ARC;

            // default should not be reached.
            return EdgeType.REAL_DUAL;
        }

        public void AddUndirectedEdge(Point from, Point to, double cost, EdgeType eType)
        {
            //
            // Are these nodes in the graph?
            //
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from));
            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to));

            if (fromNodeIndex == -1 || toNodeIndex == -1)
            {
                throw new ArgumentException("Edge uses undefined nodes: " + from + " " + to);
            }

            //
            // Check if the edge already exists
            //
            PlanarGraphEdge fromToEdge = nodes[fromNodeIndex].GetEdge(to);
            if (fromToEdge != null)
            {
                PlanarGraphEdge toFromEdge = nodes[toNodeIndex].GetEdge(from);

                fromToEdge.edgeType = UpdateEdge(fromToEdge.edgeType, eType);
                toFromEdge.edgeType = fromToEdge.edgeType;

                // Increment the degree if it is an arc.
                if (eType == EdgeType.REAL_ARC)
                {
                    fromToEdge.degree++;
                    toFromEdge.degree++;
                }
            }
            //
            // The edge does not exist.
            //
            else
            {
                nodes[fromNodeIndex].AddEdge(to, eType, cost, (eType == EdgeType.REAL_ARC ? 1 : 0));
                nodes[toNodeIndex].AddEdge(from, eType, cost, (eType == EdgeType.REAL_ARC ? 1 : 0));
            }
        }

        public bool Contains(Point value)
        {
            return nodes.Contains(new PlanarGraphNode(value));
        }

        public bool RemoveNode(Point value)
        {
            if (!nodes.Remove(new PlanarGraphNode(value))) return false;

            // enumerate through each node in the nodes, removing edges to this node
            foreach (PlanarGraphNode node in nodes)
            {
                node.RemoveEdge(value);
            }

            return true;
        }

        public bool RemoveEdge(Point from, Point to)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from));
            if (fromNodeIndex == -1) return false;
            nodes[fromNodeIndex].RemoveEdge(to);

            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to));
            if (toNodeIndex == -1) return false;
            nodes[toNodeIndex].RemoveEdge(from);

            return true;
        }

        public PlanarGraphEdge GetEdge(Point from, Point to)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from));
            if (fromNodeIndex == -1) return null;

            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to));
            if (toNodeIndex == -1) return null;

            return nodes[fromNodeIndex].GetEdge(to);
        }

        public EdgeType GetEdgeType(Point from, Point to)
        {
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from));

            return nodes[fromNodeIndex].GetEdge(to).edgeType;
        }

        public void MarkCycleEdge(Point from, Point to)
        {
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from));
            if (fromNodeIndex == -1) return;
            nodes[fromNodeIndex].MarkEdge(to);

            int toNodeIndex = nodes.IndexOf(new PlanarGraphNode(to));
            if (toNodeIndex == -1) return;
            nodes[toNodeIndex].MarkEdge(from);
        }

        public bool IsCycleEdge(Point from, Point to)
        {
            // Does this edge exist already?
            int fromNodeIndex = nodes.IndexOf(new PlanarGraphNode(from));

            if (fromNodeIndex == -1) return false;

            return nodes[fromNodeIndex].IsCyclicEdge(to);
        }

        //
        // Unmark any marked nodes and edges
        //
        public void Reset()
        {
            foreach (PlanarGraphNode node in nodes)
            {
                node.Clear();
            }
        }

        public int Count
        {
            get { return nodes.Count; }
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            foreach (PlanarGraphNode node in nodes)
            {
                str.AppendLine(node.ToString());
            }

            return str.ToString();
        }
    }
}
