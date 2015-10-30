using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public class PlanarGraphConstructor
    {
        ////
        //// The graph we use as the basis for region identification.
        ////
        //private UndirectedPlanarGraph.PlanarGraph graph; // graph that may be modified by the algorithm.
        //public UndirectedPlanarGraph.PlanarGraph GetGraph() { return graph; }

        //// This provides access to all of the required knowledge of the figure.
        //private ImpliedComponentCalculator implied;

        ////
        //// Make the graph.
        ////   (1) The nodes of the graph are all evident points in the graph and all extended points.
        ////   (2) Edges refer to the physical, planar connections between nodes.   
        ////
        //public PlanarGraphConstructor(ImpliedComponentCalculator impl)
        //{
        //    implied = impl;
        //    graph = new UndirectedPlanarGraph.PlanarGraph();

        //    //
        //    // Populate the planar graph.
        //    //
        //    List<Point> allPoints = new List<Point>();
        //    ConstructGraphNodes(allPoints); // allPoints populated here
        //    ConstructGraphEdges(allPoints);
        //}

        ////
        //// Add all real and imaginary points as nodes
        ////
        //private void ConstructGraphNodes(List<Point> allPoints)
        //{
        //    //
        //    // All important points on the original drawing, intersections, etc.
        //    //
        //    foreach (Point realPt in implied.allFigurePoints)
        //    {
        //        graph.AddNode(realPt);
        //        allPoints.Add(realPt);
        //    }

        //    //
        //    // Intersections attributed to constructed components.
        //    //
        //    foreach (ImaginaryPoint imagPt in implied.imagPoints)
        //    {
        //        graph.AddNode(imagPt);
        //        allPoints.Add(imagPt);
        //    }

        //    //
        //    // Points from extending diameters in circles
        //    //
        //    foreach (Point pt in implied.extendedCirclePoints)
        //    {
        //        graph.AddNode(pt);
        //        allPoints.Add(pt);
        //    }
        //}

        ////
        //// Construct all edges:
        ////
        ////  Real:
        ////    (1) Direct segments
        ////    (2) Circle arcs
        ////    (3) Chords
        ////  Extended:
        ////    (1) Segments connecting to imaginary points.
        ////
        //private void ConstructGraphEdges(List<Point> allPoints)
        //{
        //    AddSegments(allPoints);
        //    AddArcs();
        //}

        ////
        //// Given two points on a segment, add this connection to the graph in a manner such that
        //// if there exists a point between the points which is collinear, add two connections instead.
        ////
        //private void AddSegmentEdge(List<Point> allPoints, GeometryTutorLib.ConcreteAST.Segment segment, UndirectedPlanarGraph.EdgeType type)
        //{
        //    GeometryTutorLib.ConcreteAST.Segment copy = new GeometryTutorLib.ConcreteAST.Segment(segment);

        //    //
        //    // Check the set of all points to see if there exists a set of collinear points between the endpoints.
        //    //
        //    foreach (Point thePoint in allPoints)
        //    {
        //        if (copy.PointLiesOnAndExactlyBetweenEndpoints(thePoint))
        //        {
        //            copy.AddCollinearPoint(thePoint);
        //        }
        //    }

        //    //
        //    // For all collinear points with the segment, generate planar edges.
        //    // Do not cycle over the points since that is what we are trying to avoid.
        //    //
        //    List<Point> collinear = copy.collinear;
        //    if (collinear.Count > 2)
        //    {
        //        for (int p = 0; p < collinear.Count - 1; p++)
        //        {
        //            graph.AddUndirectedEdge(collinear[p], collinear[p + 1], Point.calcDistance(collinear[p], collinear[p + 1]), type);
        //        }
        //    }
        //    // This is a minimal situation, where there are no points in between.
        //    else
        //    {
        //        graph.AddUndirectedEdge(collinear[0], collinear[1], Point.calcDistance(collinear[0], collinear[1]), type);
        //    }
        //}

        ////
        //// Segments
        ////
        //// We only want to consider the smallest collinear segments: A---M---B would have 3 segments AM, MB, AB. We only want AM and  MB
        ////
        //public void AddSegments(List<Point> allPoints)
        //{
        //    //
        //    // Regular Segments (minimal, in this case)
        //    //
        //    foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.minimalSegments)
        //    {
        //        AddSegmentEdge(allPoints, segment, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
        //    }

        //    //
        //    // Implied Chords
        //    //
        //    foreach (KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, List<GeometryTutorLib.ConcreteAST.Circle>> chordPair in implied.impliedChords)
        //    {
        //        AddSegmentEdge(allPoints, chordPair.Key, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
        //    }

        //    //
        //    // Implied Radii
        //    //
        //    foreach (GeometryTutorLib.ConcreteAST.Segment radius in implied.extendedRealRadii)
        //    {
        //        AddSegmentEdge(allPoints, radius, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
        //    }

        //    //
        //    // Extended (non-real) Radii
        //    //
        //    foreach (GeometryTutorLib.ConcreteAST.Segment radius in implied.extendedNonRealRadii)
        //    {
        //        AddSegmentEdge(allPoints, radius, UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT);
        //    }


        //    //
        //    // Extended Diameters (in the form of Radii)
        //    //
        //    // Do we have two radii which overlap (Circle-Circle Intersection creates this)?
        //    // Create one list, and seek overlap.
        //    //List<KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, bool>> segmentsAndTypes = new List<KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, bool>>();
        //    //implied.extendedRadii.ForEach(r => segmentsAndTypes.Add(new KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, bool>(r, false)));
        //    //implied.extendedRealRadii.ForEach(r => segmentsAndTypes.Add(new KeyValuePair<GeometryTutorLib.ConcreteAST.Segment, bool>(r, true)));
        //    //bool[] marked = new bool[segmentsAndTypes.Count];

        //    //for (int s1 = 0; s1 < segmentsAndTypes.Count - 1; s1++)
        //    //{
        //    //    if (!marked[s1])
        //    //    {
        //    //        int s2Index = -1;
        //    //        Point intPoint = null;
        //    //        for (int s2 = s1 + 1; s2 < segmentsAndTypes.Count; s2++)
        //    //        {
        //    //            // The radii should be from different circles
        //    //            if (segmentsAndTypes[s1].Key.SharedVertex(segmentsAndTypes[s2].Key) == null)
        //    //            {
        //    //                // Is there an overlap?
        //    //                if (segmentsAndTypes[s1].Key.PointLiesOnAndExactlyBetweenEndpoints(segmentsAndTypes[s2].Key.Point1))
        //    //                {
        //    //                    intPoint = segmentsAndTypes[s2].Key.Point1;
        //    //                    s2Index = s2;
        //    //                }
        //    //                else if (segmentsAndTypes[s1].Key.PointLiesOnAndExactlyBetweenEndpoints(segmentsAndTypes[s2].Key.Point2))
        //    //                {
        //    //                    intPoint = segmentsAndTypes[s2].Key.Point2;
        //    //                    s2Index = s2;
        //    //                }
        //    //            }

        //    //            if (s2Index != -1) break;
        //    //        }

        //    //        // No overlap
        //    //        if (s2Index == -1)
        //    //        {
        //    //            graph.AddUndirectedEdge(segmentsAndTypes[s1].Key.Point1,
        //    //                                           segmentsAndTypes[s1].Key.Point2,
        //    //                                           segmentsAndTypes[s1].Key.Length,
        //    //                                           segmentsAndTypes[s1].Value ? UndirectedPlanarGraph.EdgeType.REAL_SEGMENT : UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT);
        //    //        }
        //    //        //
        //    //        // overlap
        //    //        //
        //    //        else
        //    //        {
        //    //            marked[s2Index] = true;
        //    //            Collinear coll = new Collinear();
        //    //            coll.AddCollinearPoint(segmentsAndTypes[s1].Key.Point1);
        //    //            coll.AddCollinearPoint(segmentsAndTypes[s1].Key.Point2);
        //    //            coll.AddCollinearPoint(segmentsAndTypes[s2Index].Key.Point1);
        //    //            coll.AddCollinearPoint(segmentsAndTypes[s2Index].Key.Point2);

        //    //            // Create the graph edges.
        //    //            for (int p = 0; p < coll.points.Count - 1; p++)
        //    //            {
        //    //                graph.AddUndirectedEdge(coll.points[p], coll.points[p + 1], Point.calcDistance(coll.points[p], coll.points[p + 1]),
        //    //                                               segmentsAndTypes[s1].Value ? UndirectedPlanarGraph.EdgeType.REAL_SEGMENT : UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.extendedRadii)
        //    //{
        //    //    graph.AddUndirectedEdge(segment.Point1, segment.Point2, segment.Length, UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT);
        //    //}
        //    //foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.extendedRealRadii)
        //    //{
        //    //    graph.AddUndirectedEdge(segment.Point1, segment.Point2, segment.Length, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
        //    //}




        //    // Determine the actual set of minimal segments; check if any circle implied points are in the middle of the minimal segments.
        //    //foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.minimalSegments)
        //    //{
        //    //    // Find the subset of applicable points; ensure they are ordered.
        //    //    foreach (Point impliedCircPt in implied.impliedCirclePoints)
        //    //    {
        //    //        if (segment.PointLiesOnAndExactlyBetweenEndpoints(impliedCircPt)) segment.AddCollinearPoint(impliedCircPt);
        //    //    }

        //    //    // Generate the actual minimal segments.
        //    //    for (int p = 0; p < segment.collinear.Count-1; p++)
        //    //    {
        //    //        GeometryTutorLib.ConcreteAST.Segment temp = new GeometryTutorLib.ConcreteAST.Segment(segment.collinear[p], segment.collinear[p + 1]);
        //    //        if (!GeometryTutorLib.Utilities.HasStructurally<GeometryTutorLib.ConcreteAST.Segment>(implied.extendedRealRadii, temp))
        //    //        {
        //    //            graph.AddUndirectedEdge(temp.Point1, temp.Point2, temp.Length, UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
        //    //        }
        //    //    }
        //    //}
        //}

        ////
        //// Arcs
        ////
        //// Same as Segments, but with Arcs:
        //// We only want to consider the smallest collinear Arcs: A---M---B would have 3 arcs AM, MB, AB. We only want AM and  MB
        ////
        //public void AddArcs()
        //{
        //    //
        //    // For each circle:
        //    //   (1) Make a copy of the circle.
        //    //   (2) Add the applicable extended points to the circle
        //    //   (3) Traverse the set of circle points to add edges to the graph
        //    //
        //    foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
        //    {
        //        GeometryTutorLib.ConcreteAST.Circle copy = new GeometryTutorLib.ConcreteAST.Circle(circle.center, circle.radius);
        //        List<Point> pts = new List<Point>(circle.pointsOnCircle);

        //        //
        //        // Add any applicable extended circle points.
        //        //
        //        foreach (Point extendedPt in implied.extendedCirclePoints)
        //        {
        //            if (copy.PointLiesOn(extendedPt)) pts.Add(extendedPt);
        //        }

        //        // The setting of points orders them appropriately.
        //        copy.SetPointsOnCircle(pts);

        //        // Add all of the following edges in sequence.
        //        List<Point> circPts = copy.pointsOnCircle;
        //        for (int p = 0; p < circPts.Count; p++)
        //        {
        //            GeometryTutorLib.ConcreteAST.Segment temp = new GeometryTutorLib.ConcreteAST.Segment(circPts[p], circPts[(p+1) % circPts.Count]);
        //            graph.AddUndirectedEdge(temp.Point1, temp.Point2, temp.Length, UndirectedPlanarGraph.EdgeType.REAL_ARC); 
        //        }
        //    }
        //}
    }
}