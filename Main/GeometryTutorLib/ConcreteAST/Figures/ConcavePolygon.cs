using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a concave polygon (which consists of n >= 4 segments)
    /// </summary>
    public partial class ConcavePolygon : Polygon
    {
        public ConcavePolygon() { }

        public ConcavePolygon(List<Segment> segs, List<Point> pts, List<Angle> angs) : base()
        {
            orderedSides = segs;
            points = pts;
            angles = angs;

            this.FigureSynthesizerConstructor();
        }

        public List<Area_Based_Analyses.Atomizer.AtomicRegion> Atomize(List<Point> figurePoints)
        {
            //
            // Clear collinearities in preparation for determining intersection points.
            //
            List<Segment> extendedSegments = new List<Segment>();
            foreach (Segment side in orderedSides)
            {
                side.ClearCollinear();
            }

            //
            // Determine if any side intersects a non-adjacent side.
            // If so, track all the intersection points.
            //
            List<Point> imagPts = new List<Point>();
            for (int s1 = 0; s1 < orderedSides.Count - 1; s1++)
            {
                // +2 excludes this side and the adjacent side
                for (int s2 = s1 + 2; s2 < orderedSides.Count; s2++)
                {
                    // Avoid intersecting the first with the last.
                    if (s1 != 0 || s2 != orderedSides.Count - 1)
                    {
                        Point intersection = orderedSides[s1].FindIntersection(orderedSides[s2]);

                        intersection = Utilities.AcquirePoint(figurePoints, intersection);

                        if (intersection != null)
                        {
                            // The point of interest must be on the perimeter of the polygon.
                            if (this.PointLiesOn(intersection) || this.PointLiesInside(intersection))
                            {
                                orderedSides[s1].AddCollinearPoint(intersection);
                                orderedSides[s2].AddCollinearPoint(intersection);

                                // The intersection point may be a vertex; avoid redundant additions.
                                if (!Utilities.HasStructurally<Point>(imagPts, intersection))
                                {
                                    imagPts.Add(intersection);
                                }
                            }
                        }
                    }
                }
            }

            //
            // Add the imaginary points to the list of figure points;
            // this is needed for consistency among all regions / polygons.
            //
            Utilities.AddUniqueList<Point>(figurePoints, imagPts);

            //
            // Construct the Planar graph for atomic region identification.
            //
            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph graph = new Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph();

            // Add all imaginary points and intersection points
            foreach (Point pt in this.points)
            {
                graph.AddNode(pt);
            }

            foreach (Point pt in imagPts)
            {
                graph.AddNode(pt);
            }

            //
            // Cycle through collinearities adding to the graph.
            // Ensure that a connection is interior to this polygon.
            //
            foreach (Segment side in orderedSides)
            {
                for (int p = 0; p < side.collinear.Count - 1; p++)
                {
                    //
                    // Find the midpoint of this segment and determine if it is interior to the polygon.
                    // If it is, then this is a legitimate connection.
                    //
                    Segment thisSegment = new Segment(side.collinear[p], side.collinear[p + 1]);
                    Point midpoint = thisSegment.Midpoint();
                    if (this.PointLiesInOrOn(midpoint))
                    {
                        graph.AddUndirectedEdge(side.collinear[p], side.collinear[p + 1], thisSegment.Length,
                                                Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                    }
                }
            }

            //
            // Convert the planar graph to atomic regions.
            //
            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();
            List<AtomicRegion> atoms = PrimitiveToRegionConverter.Convert(graph, primitives, new List<Circle>()); // No circles here.

            // State ownership
            foreach (AtomicRegion atom in atoms)
            {
                atom.AddOwner(this);
                this.AddAtomicRegion(atom);
            }

            return atoms;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            ConcavePolygon thatPoly = obj as ConcavePolygon;
            if (thatPoly == null) return false;

            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            ConcavePolygon thatPoly = obj as ConcavePolygon;
            if (thatPoly == null) return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Concave(");
            str.Append(base.ToString());
            str.Append(")");

            return str.ToString();
        }
    }
}
