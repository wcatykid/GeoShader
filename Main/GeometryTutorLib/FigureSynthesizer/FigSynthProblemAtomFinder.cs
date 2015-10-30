using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract partial class FigSynthProblem
    {
        //
        // Acquire the set of atomic regions and OMIT the region given by the inner shape.
        //
        public static List<AtomicRegion> AcquireOpenAtomicRegions(List<Connection> connections, List<Point> innerShapePoints, Figure innerShape)
        {
            List<AtomicRegion> atoms = ConstructAtomicRegions(connections, innerShapePoints, innerShape);

            int foundIndex = -1;
            for (int a = 0; a < atoms.Count; a++)
            {
                ShapeAtomicRegion shapeAtom = atoms[a] as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    Polygon shapeAtomPoly = shapeAtom.shape as Polygon;
                    Polygon innerShapePoly = innerShape as Polygon;
                    if (shapeAtomPoly != null && innerShapePoly != null)
                    {
                        if (shapeAtomPoly.HasSamePoints(innerShapePoly)) foundIndex = a;
                    }
                    else if (shapeAtom.shape.StructurallyEquals(innerShape))
                    {
                        foundIndex = a;
                    }
                }
            }

            if (foundIndex == -1)
            {
                throw new Exception("Expected to find the original shape " + innerShape.ToString() + " as atomic region; did not.");
            }

            atoms.RemoveAt(foundIndex);

            return atoms;
        }

        //
        // All innerShape connections are among the points that lie on the set of given connections.
        // Determine which of this innerShape endpoints lie between the 'outer' set of connections.
        //
        public static List<AtomicRegion> ConstructAtomicRegions(List<Connection> connections, List<Point> innerShapePoints, Figure innerShape)
        {
            List<Point> points;
            List<Segment> segments;
            List<Arc> arcs;
            List<Circle> circles;

            CollectConstituentElements(connections, innerShapePoints, innerShape, out points, out segments, out arcs, out circles);

            // Acquire circle granularity for atomic region finding.
            Dictionary<Circle, int> circGranularity = Circle.AcquireCircleGranularity(circles);

            // Extract the atomic regions.
            return GeometryTutorLib.AtomicRegionIdentifier.AtomicIdentifierMain.AcquireAtomicRegionsFromGraph(points, segments, arcs, circles, circGranularity);
        }

        private static void CollectConstituentElements(List<Connection> connections,
                                                       List<Point> innerShapePoints,
                                                       Figure innerShape,
                                                       out List<Point> points,
                                                       out List<Segment> segments,
                                                       out List<Arc> arcs,
                                                       out List<Circle> circles)
        {
            points = new List<Point>();
            segments = new List<Segment>();
            arcs = new List<Arc>();
            circles = new List<Circle>();

            //
            // Handle the collinear points resulting from the inner shape.
            //
            HandleCollinearConnections(connections, innerShapePoints);

            // Add the inner shape connections to the list of connections.
            List<Connection> combinedConnections = new List<Connection>();
            combinedConnections.AddRange(connections);
            combinedConnections.AddRange(innerShape.MakeAtomicConnections());

            //
            // Convert all connections to segments and arcs for atomic region identification.
            //
            foreach (Connection conn in combinedConnections)
            {
                if (conn.type == ConnectionType.SEGMENT)
                {
                    Segment seg = conn.segmentOrArc as Segment;
                    if (!GeometryTutorLib.Utilities.HasStructurally<Segment>(segments, seg))
                    {
                        segments.Add(seg);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, seg.Point1);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, seg.Point2);
                    }
                }
                else if (conn.type == ConnectionType.ARC)
                {
                    Arc arc = conn.segmentOrArc as Arc;
                    if (!GeometryTutorLib.Utilities.HasStructurally<Arc>(arcs, arc))
                    {
                        arcs.Add(arc);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, arc.endpoint1);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, arc.endpoint2);

                        // Accumulate the list of circles.
                        GeometryTutorLib.Utilities.AddStructurallyUnique<Circle>(circles, arc.theCircle);
                    }
                }
            }
        }

        //
        // Determine which of the inner Shape points apply to the given connection; add them as collinear points.
        //
        private static void HandleCollinearConnections(List<Connection> connections, List<Point> innerShapePoints)
        {
            foreach (Connection conn in connections)
            {
                conn.segmentOrArc.ClearCollinear();
                foreach (Point pt in innerShapePoints)
                {
                    if (conn.segmentOrArc.PointLiesOn(pt))
                    {
                        conn.segmentOrArc.AddCollinearPoint(pt);
                    }
                }
            }
        }

        //
        //
        // Appending
        //
        //
        //
        // Use the pathological atomic region identification code to id the atoms.
        //
        public static List<AtomicRegion> AcquireOpenAtomicRegions(List<AtomicRegion> atoms)
        {
            // Collect all the points from the atoms.
            List<Point> points = new List<Point>();
            foreach (AtomicRegion atom in atoms)
            {
                points.AddRange(atom.GetVertices());
            }

            return AtomicRegionIdentifier.AtomicIdentifierMain.IdentifyPathological(points, atoms, new List<Circle>(), new Dictionary<Circle,int>());
        }
    }
}