using System.Collections.Generic;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph
{
    //
    // For atomic region identification
    //
    public enum EdgeType { REAL_ARC, REAL_SEGMENT, REAL_DUAL, EXTENDED_SEGMENT };

    public class PlanarGraphEdge
    {
        public GeometryTutorLib.ConcreteAST.Point target { get; private set; }
        public double cost { get; private set; }
        public EdgeType edgeType { get; set; }
        public int degree { get; set; } // The number of connections between two nodes that are ARCS.
        public bool isCycle { get; set; }

        public PlanarGraphEdge(GeometryTutorLib.ConcreteAST.Point targ, EdgeType type, double c, int initDegree)
        {
            this.target = targ;
            edgeType = type;

            cost = c;
            degree = initDegree;
            isCycle = false;
        }

        // For quick construction only
        public PlanarGraphEdge(GeometryTutorLib.ConcreteAST.Point targ)
        {
            this.target = targ;
        }

        //
        // Shallow copy constructor
        //
        public PlanarGraphEdge(PlanarGraphEdge thatEdge) : this(thatEdge.target, thatEdge.edgeType, thatEdge.cost, thatEdge.degree) { }

        public void Clear()
        {
            isCycle = false;
        }

        public override int GetHashCode() { return base.GetHashCode(); }
        //
        // Equality is only based on the point in the graph.
        //
        public override bool Equals(object obj)
        {
            PlanarGraphEdge thatEdge = obj as PlanarGraphEdge;
            if (thatEdge == null) return false;

            return this.target.Equals(thatEdge.target);
        }

        public override string ToString()
        {
            return target.ToString() + "(" + edgeType + ")";
        }

    }
}
