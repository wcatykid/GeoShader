using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public class FacetCalculator
    {
        // The graph we use as the basis for region identification.
        private UndirectedPlanarGraph.PlanarGraph graph;

        // The list of minimal cycles, filaments, and isolated points.
        private List<Primitive> primitives;
        public List<Primitive> GetPrimitives() { return primitives; }

        public FacetCalculator(UndirectedPlanarGraph.PlanarGraph g)
        {
            graph = g;

            if (Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine(graph);
            }

            primitives = new List<Primitive>();

            ExtractPrimitives();
        }

        //
        // We want our first vector to be downward (-90 degrees std unit circle)
        //
        private Point GetFirstNeighbor(Point currentPt)
        {
            Point imaginaryPrevPt = new Point("", currentPt.X, currentPt.Y + 1);
            Point prevCurrVector = Point.MakeVector(imaginaryPrevPt, currentPt);

            // We want the point that creates the smallest angle w.r.t. to the stdVector

            // Information that will change along with the current candidate next point. 
            double currentAngle = 360; // This will be overwritten
            Point currentNextPoint = null;

            // Index of the current point so we can get its neighbors.
            int currentPtIndex = graph.IndexOf(currentPt);

            foreach (UndirectedPlanarGraph.PlanarGraphEdge edge in graph.nodes[currentPtIndex].edges)
            {
                int neighborIndex = graph.IndexOf(edge.target);
                Point neighbor = graph.nodes[neighborIndex].thePoint;

                // Create a vector of the current point with it's neighbor
                Point currentNeighborVector = Point.MakeVector(currentPt, neighbor);

                // Cross product of the two vectors to determine if we have an angle that is < 180 or > 180.
                double crossProduct = Point.CrossProduct(prevCurrVector, currentNeighborVector);

                double angleMeasure = Point.AngleBetween(prevCurrVector, currentNeighborVector);

                // if (GeometryTutorLib.Utilities.GreaterThan(crossProduct, 0)) angleMeasure = angleMeasure;
                if (GeometryTutorLib.Utilities.CompareValues(crossProduct, 0)) angleMeasure = 180;
                else if (crossProduct < 0) angleMeasure = 360 - angleMeasure;

                // If there are have the same angle, choose the one farther away (it is due to two connections)
                // So these points are collinear with a segment, but indistinguishable with two arcs.
                if (Utilities.CompareValues(angleMeasure, currentAngle))
                {
                    double currentDist = Point.calcDistance(currentPt, currentNextPoint);
                    double candDist = Point.calcDistance(currentPt, neighbor);

                    // Take the farthest point.
                    if (candDist > currentDist)
                    {
                        currentAngle = angleMeasure;
                        currentNextPoint = neighbor;
                    }
                }
                else if (angleMeasure < currentAngle)
                {
                    currentAngle = angleMeasure;
                    currentNextPoint = neighbor;
                }
            }

            return currentNextPoint;
        }

        //
        // With respect to the given vector (based on prevPt and currentPt), return the tightest counter-clockwise neighbor.
        //
        private Point GetTightestCounterClockwiseNeighbor(Point prevPt, Point currentPt)
        {
            Point prevCurrVector = Point.MakeVector(prevPt, currentPt);

            // We want the point that creates the smallest angle w.r.t. to the stdVector

            // Information that will change along with the current candidate next point. 
            double currentAngle = 360; // This will be overwritten
            Point currentNextPoint = null;

            // Index of the current point so we can get its neighbors.
            int prevPtIndex = graph.IndexOf(prevPt);
            int currentPtIndex = graph.IndexOf(currentPt);

            foreach (UndirectedPlanarGraph.PlanarGraphEdge edge in graph.nodes[currentPtIndex].edges)
            {
                int neighborIndex = graph.IndexOf(edge.target);

                if (prevPtIndex != neighborIndex)
                {
                    Point neighbor = graph.nodes[neighborIndex].thePoint;

                    // Create a vector of the current point with it's neighbor
                    Point currentNeighborVector = Point.MakeVector(currentPt, neighbor);

                    // Cross product of the two vectors to determine if we have an angle that is < 180 or > 180.
                    double crossProduct = Point.CrossProduct(prevCurrVector, currentNeighborVector);

                    double angleMeasure = Point.AngleBetween(Point.GetOppositeVector(prevCurrVector), currentNeighborVector);

                    // if (GeometryTutorLib.Utilities.GreaterThan(crossProduct, 0)) angleMeasure = angleMeasure;
                    if (GeometryTutorLib.Utilities.CompareValues(crossProduct, 0))
                    {
                        // Circles create a legitimate situation where we want to walk back in the same 'collinear' path.
                        if (Point.OppositeVectors(prevCurrVector, currentNeighborVector))
                        {
                            throw new System.Exception("FacetCalculator has collinear points in graph, but a cycle in the edges.");
                        }
                        else 
                        {
                            angleMeasure = 180;
                        }
                    }
                    else if (crossProduct < 0) angleMeasure = 360 - angleMeasure;

                    // If there are have the same angle, choose the one farther away (it is due to two connections)
                    // So these points are collinear with a segment, but indistinguishable with two arcs.
                    if (Utilities.CompareValues(angleMeasure, currentAngle))
                    {
                        double currentDist = Point.calcDistance(currentPt, currentNextPoint);
                        double candDist = Point.calcDistance(currentPt, neighbor);

                        // Take the farthest point.
                        if (candDist > currentDist)
                        {
                            currentAngle = angleMeasure;
                            currentNextPoint = neighbor;
                        }
                    }
                    if (angleMeasure < currentAngle)
                    {
                        currentAngle = angleMeasure;
                        currentNextPoint = neighbor;
                    }
                }
            }

            return currentNextPoint;
        }

        private void ExtractPrimitives()
        {
            //
            // Lexicographically sorted heap of all points in the graph.
            //
            OrderedPointList heap = new OrderedPointList();
            for (int gIndex = 0; gIndex < graph.Count; gIndex++)
            {
                heap.Add(graph.nodes[gIndex].thePoint);
            }

            if (Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine(heap);
            }

            //
            // Exhaustively analyze all points in the graph.
            //
            while (!heap.IsEmpty())
            {
                Point v0 = heap.PeekMin();
                int v0Index = graph.IndexOf(v0);

                switch(graph.nodes[v0Index].NodeDegree())
                {
                    case 0:
                        // Isolated point
                        ExtractIsolatedPoint(v0, heap);
                        break;

                    case 1:
                        // Filament: start at this node and indicate the next point is its only neighbor
                        ExtractFilament(v0, graph.nodes[v0Index].edges[0].target, heap);
                        break;

                    default:
                        // filament or minimal cycle
                        ExtractPrimitive(v0, heap);
                        break;
                }
            }
        }

        //
        // Remove the isolated point from the graph and heap; add to list of primitives.
        //
        void ExtractIsolatedPoint (Point v0, OrderedPointList heap)
        {
            heap.Remove(v0);

            graph.RemoveNode(v0);

            primitives.Add(new IsolatedPoint(v0));

            if (Utilities.ATOMIC_REGION_GEN_DEBUG)
            {
                Debug.WriteLine(primitives[primitives.Count - 1].ToString());
            }
        }

        void ExtractFilament (Point v0, Point v1, OrderedPointList heap)
        {
            int v0Index = graph.IndexOf(v0);

            if (graph.IsCycleEdge(v0, v1))
            {
                if (graph.nodes[v0Index].NodeDegree() >= 3)
                {
                    graph.RemoveEdge(v0, v1);
                    v0 = v1;
                    v0Index = graph.IndexOf(v0);
                    if (graph.nodes[v0Index].NodeDegree() == 1)
                    {
                        v1 = graph.nodes[v0Index].edges[0].target;
                    }
                }

                while (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    v1 = graph.nodes[v0Index].edges[0].target;
                    
                    if (graph.IsCycleEdge(v0, v1))
                    {
                        heap.Remove(v0);
                        graph.RemoveEdge(v0, v1);
                        graph.RemoveNode(v0);
                        v0 = v1;
                        v0Index = graph.IndexOf(v0);
                    }
                    else
                    {
                        break;
                    }
                }

                if (graph.nodes[v0Index].NodeDegree() == 0)
                {
                    heap.Remove(v0);
                    graph.RemoveNode(v0);
                }
            }
            else
            {
                Filament primitive = new Filament();

                if (graph.nodes[v0Index].NodeDegree() >= 3)
                {
                    primitive.Add(v0);
                    graph.RemoveEdge(v0,v1);
                    v0 = v1;

                    v0Index = graph.IndexOf(v0);
                    if (graph.nodes[v0Index].NodeDegree() == 1)
                    {
                        v1 = graph.nodes[v0Index].edges[0].target;
                    }
                }

                while (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    primitive.Add(v0);
                    v1 = graph.nodes[v0Index].edges[0].target;
                    heap.Remove(v0);
                    graph.RemoveEdge(v0, v1);
                    graph.RemoveNode(v0);
                    v0 = v1;
                }

                primitive.Add(v0);

                if (graph.nodes[v0Index].NodeDegree() == 0)
                {
                    heap.Remove(v0);
                    graph.RemoveEdge(v0, v1);
                    graph.RemoveNode(v0);
                }
                
                primitives.Add(primitive);

                if (Utilities.ATOMIC_REGION_GEN_DEBUG)
                {
                    Debug.WriteLine(primitive.ToString());
                }
            }
        }

        //
        // Extract a minimal cycle or a filament
        //
        void ExtractPrimitive(Point v0, OrderedPointList heap)
        {
            List<Point> visited = new List<Point>();
            List<Point> sequence = new List<Point>();

            sequence.Add(v0);

            // Create an initial line as (downward) vertical w.r.t. v0; v1 is based on the vertical line through v0
            Point v1 = GetFirstNeighbor(v0); //  GetClockwiseMost(new Point("", v0.X, v0.Y + 1), v0);
            Point vPrev = v0;
            Point vCurr = v1;

            int v0Index = graph.IndexOf(v0);
            int v1Index = graph.IndexOf(v1);

            // Loop until we have a cycle or we have a null (filament)
            while (vCurr != null && !vCurr.Equals(v0) && !visited.Contains(vCurr))
            {
                sequence.Add(vCurr);
                visited.Add(vCurr);
                Point vNext = GetTightestCounterClockwiseNeighbor(vPrev, vCurr);
                vPrev = vCurr;
                vCurr = vNext;
            }

            //
            // Filament: hit an endpoint
            //
            if (vCurr == null)
            {
                // Filament found, not necessarily rooted at v0.
                ExtractFilament(v0, graph.nodes[v0Index].edges[0].target, heap);
            }
            //
            // Minimal cycle found.
            //
            else if (vCurr.Equals(v0))
            {
                MinimalCycle primitive = new MinimalCycle();

                primitive.AddAll(sequence);

                primitives.Add(primitive);

                if (Utilities.ATOMIC_REGION_GEN_DEBUG)
                {
                    Debug.WriteLine(primitive.ToString());
                }

                // Mark that these edges are a part of a cycle
                for (int p = 0; p < sequence.Count; p++)
                {
                    graph.MarkCycleEdge(sequence[p], sequence[p+1 < sequence.Count ? p+1 : 0]);
                }

                graph.RemoveEdge(v0, v1);

                //
                // Check filaments for v0 and v1
                //
                if (graph.nodes[v0Index].NodeDegree() == 1)
                {
                    // Remove the filament rooted at v0.
                    ExtractFilament(v0, graph.nodes[v0Index].edges[0].target, heap);
                }

                //
                // indices may have changed; update.
                //
                v1Index = graph.IndexOf(v1);
                if (v1Index != -1)
                {
                    if (graph.nodes[v1Index].NodeDegree() == 1)
                    {
                        // Remove the filament rooted at v1.
                        ExtractFilament(v1, graph.nodes[v1Index].edges[0].target, heap);
                    }
                }
            }
            //
            // vCurr was visited earlier
            //
            else
            {
                // A cycle has been found, but is not guaranteed to be a minimal
                // cycle. This implies v0 is part of a filament. Locate the
                // starting point for the filament by traversing from v0 away
                // from the initial v1.
                while (graph.nodes[v0Index].NodeDegree() == 2)
                {
                    // Choose between the the two neighbors
                    if (graph.nodes[v0Index].edges[0].target.Equals(v1))
                    {
                        v1 = v0;
                        v0 = graph.nodes[v0Index].edges[1].target;
                    }
                    else
                    {
                        v1 = v0;
                        v0 = graph.nodes[v0Index].edges[0].target;
                    }

                    // Find the next v0 index
                    v0Index = graph.IndexOf(v0);
                }
                ExtractFilament(v0, v1, heap);
            }
        }
    }
}