using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public static class AtomComposer
    {
        ////
        //// The given atomic region is the 'outside' perimeter.
        //// All information is based on intersection points inside or on the perimeter.
        ////
        //List<AtomicRegion> Compose(List<Point> figurePoints, AtomicRegion theAtom,
        //                           List<AtomicRegion> intersecting, List<AtomicRegion> contained)
        //{
        //    List<AtomicRegion> atoms = new List<AtomicRegion>();

        //    foreach (AtomicRegion 

        //    return atoms;
        //}


        //
        // Combine two atoms together into a set of atomic regions.
        // toadd refers to atomic regions that are new / modified. 
        // toRemove is a subset of the atoms that may be removed from the worklist since they have been replaced as a non-atomic.
        //
        public static void Compose(List<Point> figurePoints, AtomicRegion thisAtom, AtomicRegion thatAtom, out List<AtomicRegion> toAdd, out List<AtomicRegion> toRemove)
        {
            toAdd = new List<AtomicRegion>();
            toRemove = new List<AtomicRegion>();

            //
            // Do these regions interact at all?
            //
            if (thisAtom.OnExteriorOf(thatAtom)) return;

            //
            // If there is a single inscribed node then this is containment.
            // If there are no intersections then the regions may be (1) disjoint or (2) containment.
            //
            if (thisAtom.StrictlyContains(thatAtom) || thisAtom.ContainsWithOneInscription(thatAtom))
            {
                //AtomicRegion diff = GenerateDifferenceRegion(thisAtom, thatAtom);
                //toAdd(diff);
                //toRemove.Add(thisAtom);
                //return;
            }
            if (thatAtom.StrictlyContains(thisAtom) || thatAtom.ContainsWithOneInscription(thisAtom))
            {
//                return GenerateDifferenceRegion(thatAtom, thisAtom);
            }


            //
            // The atoms overlap.
            //
            Overlap(figurePoints, thisAtom, thatAtom, out toAdd, out toRemove);
        }

        private static List<AtomicRegion> GenerateDifferenceRegion(AtomicRegion outer, AtomicRegion inner)
        {
            return Utilities.MakeList<AtomicRegion>(new DifferenceAtomicRegion(outer, inner));
        }

        //
        // Compose this atomic region with a segment
        // This operation will return a list of atomic regions iff the segment passes through the atomic region; this
        // creates two new atomic regions since the segment divides the atom.
        //
        //public static List<AtomicRegion> Compose(AtomicRegion thisAtom, Segment that, Figure owner)
        //{
        //    List<AtomicRegion> newAtoms = new List<AtomicRegion>();
        //    List<AtomicRegion.IntersectionAgg> intersections = thisAtom.GetIntersections(that);

        //    // If there is only 1 intersection then we may have a 'corner' inside of the atomic region.
        //    if (intersections.Count == 1) return newAtoms;

        //    if (intersections.Count > 2) throw new ArgumentException("More than 3 intersections due to a segment during atomic region composition");

        //    // If there are two intersection points, this atomic region is split into 2 regions.
        //    if (intersections.Count != 2)
        //    {
        //        throw new ArgumentException("Expected 2 intersections due to a segment during atomic region composition; have " + intersections.Count);
        //    }

        //    //
        //    // Split the region into 2 new atomic regions.
        //    //
        //    // (0) Order the connections of this atomic region.
        //    // (1) Make a copy of the list of connections
        //    // (2) Replace 2 intersected connections with (up to) 4 new connections. 
        //    // (3) Add this segment connection
        //    //
        //    thisAtom.OrderConnections();

        //    AtomicRegion newAtom1 = new AtomicRegion();
        //    AtomicRegion newAtom2 = new AtomicRegion();

        //    bool[] marked = new bool[intersections.Count];
        //    bool atom1 = true;
        //    foreach (Connection conn in thisAtom.connections)
        //    {
        //        bool found = false;
        //        for (int i = 0; i < intersections.Count; i++)
        //        {
        //            if (!marked[i])
        //            {
        //                marked[i] = true;
        //                if (conn.Equals(intersections[i].Key))
        //                {
        //                    found = true;

        //                    //
        //                    // How does this new segment intersect this connection?
        //                    //
        //                    // Endpoint
        //                    if (conn.HasPoint(intersections[i].Value))
        //                    {
        //                        // No-op
        //                    }
        //                    // Split this connection in the middle
        //                    else
        //                    {
        //                        Connection newConn1 = new Connection(conn.endpoint1, intersections[i].Value, conn.type, conn.segmentOwner);
        //                        Connection newConn2 = new Connection(conn.endpoint2, intersections[i].Value, conn.type, conn.segmentOwner);

        //                        //
        //                        // Which atomic region owns which new connection.
        //                        //
        //                        if (newAtom1.HasPoint(conn.endpoint1))
        //                        {
        //                            newAtom1.AddConnection(newConn1);
        //                            newAtom2.AddConnection(newConn2);
        //                        }
        //                        else if (newAtom2.HasPoint(conn.endpoint1))
        //                        {
        //                            newAtom2.AddConnection(newConn1);
        //                            newAtom1.AddConnection(newConn2);
        //                        }
        //                        // Neither atomic region has the point (possibly the first connection encountered).
        //                        else
        //                        {
        //                            // Arbitrary assignment
        //                            newAtom1.AddConnection(newConn1);
        //                            newAtom2.AddConnection(newConn2);
        //                        }
        //                    }

        //                    // Shift to the second (new) atomic region.
        //                    atom1 = false;
        //                }
        //            }
        //        }

        //        // This is not a splittable connection so just add it to the list.
        //        if (!found)
        //        {
        //            if (atom1) newAtom1.AddConnection(conn);
        //            else newAtom2.AddConnection(conn);
        //        }
        //    }

        //    //
        //    // Add this new segment as a connection to both atomic regions.
        //    //
        //    newAtom1.AddConnection(intersections[0].Value, intersections[1].Value, ConnectionType.SEGMENT, owner);
        //    newAtom2.AddConnection(intersections[0].Value, intersections[1].Value, ConnectionType.SEGMENT, owner);

        //    // Order the connections in the new regions we created.
        //    newAtom1.OrderConnections();
        //    newAtom2.OrderConnections();
        //    newAtoms.Add(newAtom1);
        //    newAtoms.Add(newAtom2);

        //    return newAtoms;
        //}

        public static void Overlap(List<Point> figurePoints, AtomicRegion thisAtom, AtomicRegion thatAtom, out List<AtomicRegion> toAdd, out List<AtomicRegion> toRemove)
        {
            //
            // Acquire all arcs and segments.
            //
            List<Arc> graphArcs = new List<Arc>();
            List<Segment> graphSegments = new List<Segment>();
            foreach (Connection thisConn in thisAtom.connections)
            {
                if (thisConn.type == ConnectionType.SEGMENT) graphSegments.Add(thisConn.segmentOrArc as Segment);
                else graphArcs.Add(thisConn.segmentOrArc as Arc);
            }

            foreach (Connection thatConn in thatAtom.connections)
            {
                if (thatConn.type == ConnectionType.SEGMENT) graphSegments.Add(thatConn.segmentOrArc as Segment);
                else graphArcs.Add(thatConn.segmentOrArc as Arc);
            }

            //
            // Clear collinearities of all segments / arcs.
            //
            List<Circle> circles = new List<Circle>(); // get the list of applicable circles to these atoms.
            foreach (Segment seg in graphSegments) seg.ClearCollinear();
            foreach (Arc arc in graphArcs)
            {
                Utilities.AddStructurallyUnique<Circle>(circles, arc.theCircle);
                arc.ClearCollinear();
            }

            //
            // All points of interest for these atoms.
            //
            List<Point> allPoints = new List<Point>();
            allPoints.AddRange(thisAtom.GetVertices());
            allPoints.AddRange(thatAtom.GetVertices());

            //
            // Determine 'collinearities' for the intersections.
            //
            List<AtomicRegion.IntersectionAgg> intersections = thisAtom.GetIntersections(figurePoints, thatAtom);

            List<Point> intersectionPts = new List<Point>();
            foreach (AtomicRegion.IntersectionAgg agg in intersections)
            {
                if (agg.intersection1 != null)
                {
                    if (!Utilities.HasStructurally<Point>(allPoints, agg.intersection1)) intersectionPts.Add(agg.intersection1);
                    agg.thisConn.segmentOrArc.AddCollinearPoint(agg.intersection1);
                    agg.thatConn.segmentOrArc.AddCollinearPoint(agg.intersection1);
                }
                if (agg.intersection2 != null)
                {
                    if (!Utilities.HasStructurally<Point>(allPoints, agg.intersection2)) intersectionPts.Add(agg.intersection2);
                    intersectionPts.Add(agg.intersection2);
                    agg.thisConn.segmentOrArc.AddCollinearPoint(agg.intersection2);
                    agg.thatConn.segmentOrArc.AddCollinearPoint(agg.intersection2);
                }
            }

            // Add any unlabeled intersection points.
            allPoints.AddRange(intersectionPts);

            //
            // Construct the Planar graph for atomic region identification.
            //
            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph graph = new Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph();

            // Add the points as nodes in the graph.
            foreach (Point pt in allPoints)
            {
                graph.AddNode(pt);
            }

            //
            // Edges are based on all the collinear relationships.
            //
            foreach (Segment segment in graphSegments)
            {
                for (int p = 0; p < segment.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(segment.collinear[p], segment.collinear[p+1],
                                            new Segment(segment.collinear[p], segment.collinear[p + 1]).Length,
                                            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                }
            }

            foreach (Arc arc in graphArcs)
            {
                for (int p = 0; p < arc.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(arc.collinear[p], arc.collinear[p + 1],
                                            new Segment(arc.collinear[p], arc.collinear[p + 1]).Length,
                                            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_ARC);
                }
            }

            //
            // Convert the planar graph to atomic regions.
            //
            Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();
            List<AtomicRegion> atoms = PrimitiveToRegionConverter.Convert(graph, primitives, circles);

            //
            // Determine ownership of the atomic regions.
            //
            foreach (AtomicRegion atom in atoms)
            {
                if (thisAtom.Contains(atom))
                {
                    atom.AddOwners(thisAtom.owners);
                }
                if (thatAtom.Contains(atom))
                {
                    atom.AddOwners(thatAtom.owners);
                }
            }

            toAdd = atoms;
            toRemove = new List<AtomicRegion>();
            toRemove.Add(thisAtom);
            toRemove.Add(thatAtom);
        }
    }
}