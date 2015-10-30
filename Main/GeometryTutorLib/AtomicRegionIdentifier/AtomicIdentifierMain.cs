using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.TutorParser;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.AtomicRegionIdentifier
{
    /// <summary>
    /// Identifies all atomic regions in the figure.
    /// </summary>
    public static class AtomicIdentifierMain
    {
        public static List<AtomicRegion> GetAtomicRegions(List<Point> figurePoints,
                                                          List<Circle> circles,
                                                          List<Polygon>[] polygons)
        {
            List<AtomicRegion> originalAtoms = new List<AtomicRegion>();
            Dictionary<Circle, int> circGranularity = new Dictionary<Circle, int>();

            //
            // Convert all circles to atomic regions identifying their chord / radii substructure.
            //
            if (circles.Any())
            {
                // Construct the granularity for when we construct arcs.
                circles = new List<Circle>(circles.OrderBy(c => c.radius));
                double currRadius = circles[0].radius;
                int gran = 1;

                foreach (Circle circle in circles)
                {
                    List<AtomicRegion> circleAtoms = circle.Atomize(figurePoints);

                    // Make this circle the owner of the atomic regions.
                    foreach (AtomicRegion atom in circleAtoms)
                    {
                        atom.AddOwner(circle);
                        circle.AddAtomicRegion(atom);
                    }
                    originalAtoms.AddRange(circleAtoms);

                    //
                    // Granularity
                    //
                    if (circle.radius > currRadius) gran++;
                    circGranularity[circle] = gran;
                }
            }

            //
            // Make all of the polygons an atomic region.
            // Also, convert any concave polygon into atoms by extending sides inward. 
            //
            for (int n = Polygon.MIN_POLY_INDEX; n < Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                foreach (Polygon poly in polygons[n])
                {
                    // Handle any concave polygons.
                    ConcavePolygon concave = poly as ConcavePolygon;
                    if (concave != null)
                    {
                        List<AtomicRegion> concaveAtoms = concave.Atomize(figurePoints);
                        originalAtoms.AddRange(concaveAtoms);
                        poly.AddAtomicRegions(concaveAtoms);
                    }

                    // Basic polygon: make it a shape atomic region.
                    else
                    {
                        ShapeAtomicRegion shapeAtom = new ShapeAtomicRegion(poly);
                        shapeAtom.AddOwner(poly);
                        originalAtoms.Add(shapeAtom);
                    }
                }
            }

            //
            // Since circles and Concave Polygons were atomized, there may be duplicate atomic regions.
            //
            originalAtoms = RemoveDuplicates(originalAtoms);
            List<AtomicRegion> workingAtoms = new List<AtomicRegion>(originalAtoms);

            //
            // Combine all of the atomic regions together.
            //
            List<AtomicRegion> composed = ComposeAllRegions(figurePoints, workingAtoms, circGranularity);
            composed = RemoveContained(composed);

            //
            // Run the graph-based algorithm one last time to identify any pathological regions (exterior to all shapes).
            //
            // Identify those pathological regions as well as any lost (major arcs).
            //
            List<AtomicRegion> lost = new List<AtomicRegion>();
            List<AtomicRegion> pathological = new List<AtomicRegion>();
            List<AtomicRegion> pathologicalID = IdentifyPathological(figurePoints, composed, circles, circGranularity);
            pathologicalID = RemoveRedundantSemicircle(pathologicalID);
            foreach (AtomicRegion atom in composed)
            {
                if (!pathologicalID.Contains(atom))
                {
                    bool containment = false;
                    foreach (AtomicRegion pathAtom in pathologicalID)
                    {
                        if (atom.Contains(pathAtom))
                        {
                            containment = true;
                            break;
                        }
                    }
                    if (!containment) lost.Add(atom);
                }
            }

            foreach (AtomicRegion atom in pathologicalID)
            {
                if (!composed.Contains(atom)) pathological.Add(atom);
            }

            List<AtomicRegion> finalAtomSet = new List<AtomicRegion>();
            finalAtomSet.AddRange(composed);
            finalAtomSet.AddRange(pathological);

            return finalAtomSet;
        }

        //
        // Remove any semicircles that are not truly atomic.
        //
        private static List<AtomicRegion> RemoveContained(List<AtomicRegion> originalAtoms)
        {
            List<AtomicRegion> trueAtoms = new List<AtomicRegion>();
            for (int a1 = 0; a1 < originalAtoms.Count; a1++)
            {
                bool containment = false;
                for (int a2 = 0; a2 < originalAtoms.Count; a2++)
                {
                    if (a1 != a2)
                    if (originalAtoms[a1].Contains(originalAtoms[a2]))
                    {
                        containment = true;
                        break;
                    }
                }
                if (!containment) trueAtoms.Add(originalAtoms[a1]);
            }

            return trueAtoms;
        }

        private static List<AtomicRegion> RemoveRedundantSemicircle(List<AtomicRegion> originalAtoms)
        {
            List<AtomicRegion> trueAtoms = new List<AtomicRegion>();
            foreach (AtomicRegion atom in originalAtoms)
            {
                bool add = true;

                ShapeAtomicRegion shapeAtom = atom as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    Sector sector = shapeAtom.shape as Sector;
                    if (sector != null && sector.theArc is Semicircle)
                    {
                        foreach (AtomicRegion checkAtom in originalAtoms)
                        {
                            if (!atom.Equals(checkAtom) && shapeAtom.Contains(checkAtom))
                            {
                                add = false;
                                break;
                            }
                        }
                    }
                }

                if (add) trueAtoms.Add(atom);
            }

            return trueAtoms;
        }

        //
        // Remove all duplicate regions; this transfers all ownerships of the region to the persistent region.
        //
        private static List<AtomicRegion> RemoveDuplicates(List<AtomicRegion> atoms)
        {
            List<AtomicRegion> unique = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atoms)
            {
                // If this atom exists in the list, transfer ownership; this is NOT hierarchical.
                int index = unique.IndexOf(atom);

                if (index == -1) unique.Add(atom);
                else
                {
                    unique[index].AddOwners(atom.owners);
                }
            }

            return unique;
        }

        //
        // Find all regions that overlap this region.
        //
        private static void GetInterestingRegions(List<AtomicRegion> atoms, AtomicRegion theAtom,
                                                  out List<AtomicRegion> intersecting, out List<AtomicRegion> contained)
        {
            intersecting = new List<AtomicRegion>();
            contained = new List<AtomicRegion>();

            foreach (AtomicRegion atom in atoms)
            {
                // An atom should not intersect itself.
                if (!theAtom.Equals(atom))
                {
                    if (theAtom.Contains(atom))
                    {
                        contained.Add(atom);
                    }
                    //else if (theAtom.StrictlyContains(atom))
                    //{
                    //    contained.Add(theAtom);
                    //}
                    else if (theAtom.OverlapsWith(atom))
                    {
                        intersecting.Add(atom);
                    }
                }
            }
        }

        //
        // Main routine traversing the list of all shapes to determine if the given shape is an outer shape. If it is, process it.
        //
        private static List<AtomicRegion> ComposeAllRegions(List<Point> figurePoints, List<AtomicRegion> givenAtoms, Dictionary<Circle, int> circGranularity)
        {
            // Clear the list of known atomic regions.
            List<AtomicRegion> knownAtomicRegions = new List<AtomicRegion>();
            List<AtomicRegion> knownNonAtomicRegions = new List<AtomicRegion>();
            List<List<AtomicRegion>> setsForNonAtomicRegions = new List<List<AtomicRegion>>();

            // Reset the given atoms to have unknown atomic status (and no contained atoms).
            givenAtoms.ForEach(a => a.Clear());

            // Order the set of atoms by size; largest last.
            givenAtoms = new List<AtomicRegion>(givenAtoms.OrderBy(g => g.connections.Count));
            
            //
            // Combine all regions using the given atoms as a boundary.
            //
            for (int g = 0; g < givenAtoms.Count; g++)
            {
                //
                // Perform the actual composition to find ALL atomic regions contained within this atom.
                //
                List<AtomicRegion> newBoundedAtoms = ComposeSingleRegion(figurePoints, givenAtoms[g], givenAtoms.GetRange(g+1, givenAtoms.Count - g - 1),
                                                                         knownAtomicRegions, knownNonAtomicRegions, setsForNonAtomicRegions, circGranularity);

                //
                // This is a true atomic region that cannot be split.
                //
                if (newBoundedAtoms.Count == 1) AddAtom(knownAtomicRegions, givenAtoms[g]);
                //
                // The boundary atom is replaced by all of the newAtoms
                else
                {
                    // Save all of the contained atomic regions for this atom.
                    if (AddAtom(knownNonAtomicRegions, givenAtoms[g]))
                    {
                        setsForNonAtomicRegions.Add(newBoundedAtoms);
                    }

                    // Indicate all found regions are truly atomic
                    AddRange(knownAtomicRegions, newBoundedAtoms);
                }
            }

            return knownAtomicRegions;
        }

        private static bool AddAtom(List<AtomicRegion> atoms, AtomicRegion atom)
        {
            if (atoms.Contains(atom)) return false;

            atoms.Add(atom);

            return true;
        }

        private static void AddRange(List<AtomicRegion> atoms, List<AtomicRegion> newAtoms)
        {
            newAtoms.ForEach(a => AddAtom(atoms, a));
        }

        //
        // Using a single atomic region as a set of bounds:
        //    (1) Find all interesting regions (contained and intersecting)
        //    (2) Recursively compose all contained regions.
        //    (3) Combine all into the original boundary region. 
        //
        private static List<AtomicRegion> ComposeSingleRegion(List<Point> figurePoints, AtomicRegion outerBounds, List<AtomicRegion> allRegions, 
                                                              List<AtomicRegion> knownAtomicRegions, List<AtomicRegion> knownNonAtomicRegions,
                                                              List<List<AtomicRegion>> setsForNonAtomicRegions, Dictionary<Circle, int> circGranularity)
        {
            //
            // Base cases: we have already processed this atom as a sub-atom in a previous iteration.
            //
            if (knownAtomicRegions.Contains(outerBounds)) return Utilities.MakeList<AtomicRegion>(outerBounds);
            // We've processed this atom already.
            int index = knownNonAtomicRegions.IndexOf(outerBounds);
            if (index != -1) return setsForNonAtomicRegions[index];

            //
            // Acquire the current set of regions under consideration.
            //
            List<AtomicRegion> currentAtoms = new List<AtomicRegion>(allRegions);
            AddRange(currentAtoms, knownAtomicRegions);

            //
            // Collect all interesting regions for this region: those that intersect with it and those that are contained inside.
            //
            List<AtomicRegion> intersectingSet = null;
            List<AtomicRegion> containedSet = null;
            GetInterestingRegions(currentAtoms, outerBounds, out intersectingSet, out containedSet);

            // If we have have no interactions, this is a truly atomic region.
            if (!intersectingSet.Any() && !containedSet.Any()) return Utilities.MakeList<AtomicRegion>(outerBounds);

            //
            // Recur on all containing regions.
            //
            List<AtomicRegion> newContainedAtoms = new List<AtomicRegion>();
            foreach (AtomicRegion containedAtom in containedSet)
            {
                if (knownAtomicRegions.Contains(containedAtom)) AddAtom(newContainedAtoms, containedAtom);
                else if (knownNonAtomicRegions.Contains(containedAtom))
                {
                    AddRange(newContainedAtoms, setsForNonAtomicRegions[knownNonAtomicRegions.IndexOf(containedAtom)]);
                }
                else
                {
                    // Get all regions using containedAtom as the boundary region. 
                    List<AtomicRegion> newContainedBoundedAtoms = ComposeSingleRegion(figurePoints, containedAtom, currentAtoms,
                                                                                      knownAtomicRegions, knownNonAtomicRegions, setsForNonAtomicRegions, circGranularity);
                    
                    AddRange(newContainedAtoms, newContainedBoundedAtoms);

                    //
                    // This is a true atomic region that cannot be split.
                    //
                    if (newContainedBoundedAtoms.Count == 1) AddAtom(knownAtomicRegions, containedAtom);
                    //
                    // The boundary atom is replaced by all of the newAtoms
                    else
                    {
                        // Save all of the contained atomic regions for this atom.
                        if (AddAtom(knownNonAtomicRegions, containedAtom))
                        {
                            setsForNonAtomicRegions.Add(newContainedBoundedAtoms);
                        }

                        // Indicate all found regions are truly atomic
                        AddRange(knownAtomicRegions, newContainedBoundedAtoms);
                    }
                }
            }

            //
            // Now that all contained regions are atomized, combine ALL intersections and atomic regions.
            //
            //  Collect all segments and arcs (with explicit endpoints).
            //  Extend only if they do not touch the sides of the boundaries.
            //

            // inside of the boundaries; determine all intersection points.
            //
            //  (1) All intersecting regions.
            //      (a) For all vertices inside the boundaries, extend to the closest atom.
            //      (b) For all sides that pass through determine any intersections.
            //  (2) All contained atoms
            //      (a) For each side of a region, extend to the closest region.
            //      (b) If a single circle or concentric circles, extend a diameter from the closest point inside the region, through the center. 
            //      (c) If several non-intersecting circles, extend diameters through the centers of each pair.
            //
            List<Point> points = new List<Point>();
            List<Segment> segments = new List<Segment>();
            List<Arc> arcs = new List<Arc>();

            //
            // Add the outer boundaries.
            //
            points.AddRange(outerBounds.GetVertices());
            foreach (Connection boundaryConn in outerBounds.connections)
            {
                if (boundaryConn.type == ConnectionType.ARC)
                {
                    arcs.Add(boundaryConn.segmentOrArc as Arc);
                }
                if (boundaryConn.type == ConnectionType.SEGMENT)
                {
                    segments.Add(boundaryConn.segmentOrArc as Segment);
                }
            }

            //
            // Regions that intersect the boundaries; selectively take connections.
            //
            foreach (AtomicRegion intersecting in intersectingSet)
            {
                List<AtomicRegion.IntersectionAgg> intersections = outerBounds.GetIntersections(figurePoints, intersecting);

                // Determine which intersections are interior to the boundaries.
                foreach (AtomicRegion.IntersectionAgg agg in intersections)
                {
                    if (agg.overlap) { /* No-op */ }
                    else
                    {
                        if (agg.intersection1 != null)
                        {
                            if (outerBounds.PointLiesOnOrInside(agg.intersection1))
                            {
                                if (!outerBounds.NotInteriorTo(agg.thatConn))
                                {
                                    if (agg.thatConn.type == ConnectionType.ARC) GeometryTutorLib.Utilities.AddUnique<Arc>(arcs, agg.thatConn.segmentOrArc as Arc);
                                    if (agg.thatConn.type == ConnectionType.SEGMENT)
                                        GeometryTutorLib.Utilities.AddUnique<Segment>(segments, agg.thatConn.segmentOrArc as Segment);
                                }
                                GeometryTutorLib.Utilities.AddUnique<Point>(points, agg.intersection1);
                            }
                        }
                        if (agg.intersection2 != null)
                        {
                            if (outerBounds.PointLiesOnOrInside(agg.intersection2))
                            {
                                GeometryTutorLib.Utilities.AddUnique<Point>(points, agg.intersection2);
                            }
                        }
                    }
                }
            }

            //
            // Deal with contained regions.
            //
            // TO BE COMPLETED: Deal with isolated circles.
            foreach (AtomicRegion contained in newContainedAtoms)
            {
                List<Point> verts = contained.GetVertices();
                GeometryTutorLib.Utilities.AddUniqueList<Point>(points, verts);

                foreach (Connection conn in contained.connections)
                {
                    if (conn.type == ConnectionType.ARC) Utilities.AddUnique<Arc>(arcs, conn.segmentOrArc as Arc);
                    if (conn.type == ConnectionType.SEGMENT)
                    {
                        Utilities.AddUnique<Segment>(segments, conn.segmentOrArc as Segment);
                    }
                }
            }

            //
            // Find all intersections...among segments and arcs.
            //
            foreach (Segment segment in segments)
            {
                segment.ClearCollinear();
            }
            foreach (Arc arc in arcs)
            {
                arc.ClearCollinear();
            }

            HandleSegmentSegmentIntersections(figurePoints, points, segments, outerBounds);
            HandleSegmentArcIntersections(figurePoints, points, segments, arcs, outerBounds);
            HandleArcArcIntersections(figurePoints, points, arcs, outerBounds);
            // Returns the list of maximal segments.
            segments = HandleCollinearSubSegments(segments);
            // HandleCollinearSubArcs(arcs);

            //
            // Construct the Planar graph for atomic region identification.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph graph = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph();

            // Add the points as nodes in the graph.
            foreach (Point pt in points)
            {
                graph.AddNode(pt);
            }

            //
            // Edges are based on all the collinear relationships.
            // To ensure we are taking ONLY the closest extended intersections, choose ONLY the 1 point around the actual endpoints of the arc or segment.
            //
            foreach (Segment segment in segments)
            {
                for (int p = 0; p < segment.collinear.Count - 1; p++)
                {
                    if (outerBounds.PointLiesInOrOn(segment.collinear[p]) && outerBounds.PointLiesInOrOn(segment.collinear[p + 1]))
                    {
                        graph.AddUndirectedEdge(segment.collinear[p], segment.collinear[p + 1],
                                                new Segment(segment.collinear[p], segment.collinear[p + 1]).Length,
                                                GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                    }
                }
            }

            foreach (Arc arc in arcs)
            {                
                List<Point> applicWithMidpoints = GetArcPoints(arc, circGranularity);

                // Add the points to the graph; preprocess to see if all points are inside the region.
                bool[] inOrOn = new bool[applicWithMidpoints.Count];
                for (int p = 0; p < applicWithMidpoints.Count; p++)
                {
                    if (outerBounds.PointLiesInOrOn(applicWithMidpoints[p]))
                    {
                        graph.AddNode(applicWithMidpoints[p]);
                        inOrOn[p] = true;
                    }
                }

                for (int p = 0; p < applicWithMidpoints.Count - 1; p++)
                {
                    if (inOrOn[p] && inOrOn[p + 1])
                    {
                        graph.AddUndirectedEdge(applicWithMidpoints[p], applicWithMidpoints[p + 1],
                                                new Segment(applicWithMidpoints[p], applicWithMidpoints[p + 1]).Length,
                                                GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_ARC);
                    }
                }
            }

            //
            // Collect the circles from the arcs.
            //
            List<Circle> circles = new List<Circle>();
            foreach (Arc arc in arcs)
            {
                GeometryTutorLib.Utilities.AddStructurallyUnique<Circle>(circles, arc.theCircle);
            }

            //
            // Convert the planar graph to atomic regions.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();
            List<AtomicRegion> boundedAtoms = PrimitiveToRegionConverter.Convert(graph, primitives, circles);

            ////
            //// Realign this set of atoms with the current set of working atoms; this guarantees we are looking at the same atom objects.
            ////
            //List<AtomicRegion> finalBoundedAtoms = new List<AtomicRegion>();
            //foreach (AtomicRegion boundedAtom in boundedAtoms)
            //{
            //    int tempIndex = currentAtoms.IndexOf(boundedAtom);
            //    if (tempIndex == -1) finalBoundedAtoms.Add(boundedAtom);
            //    else finalBoundedAtoms.Add(currentAtoms[tempIndex]);
            //}

            //
            // Determine ownership of the atomic regions.
            //
            foreach (AtomicRegion boundedAtom in boundedAtoms)
            {
                boundedAtom.AddOwners(outerBounds.owners);
                
                // Indicate that the given boundary shape owns all of the new regions within.
                ShapeAtomicRegion shapeAtom = outerBounds as ShapeAtomicRegion;
                if (shapeAtom != null)
                {
                    shapeAtom.shape.AddAtomicRegion(boundedAtom);
                }
            }

            return boundedAtoms;
        }

        private static List<Point> GetArcPoints(Arc theArc, Dictionary<Circle, int> granularity)
        {
            List<Point> applicWithMidpoints = theArc.GetOrderedByEndpointsWithMidpoints(theArc.collinear);

            for (int g = 1; g < granularity[theArc.theCircle]; g++)
            {
                applicWithMidpoints = theArc.GetOrderedByEndpointsWithMidpoints(applicWithMidpoints);
            }

            return applicWithMidpoints;
        }

        public static List<AtomicRegion> IdentifyPathological(List<Point> figurePoints, List<AtomicRegion> atoms,
                                                              List<Circle> circles, Dictionary<Circle, int> circGranularity)
        {
            List<Point> points = new List<Point>();
            List<Segment> segments = new List<Segment>();
            List<Arc> arcs = new List<Arc>();

            // Collect segments and arcs.
            foreach (AtomicRegion atom in atoms)
            {
                foreach (Connection conn in atom.connections)
                {
                    if (conn.type == ConnectionType.SEGMENT)
                    {
                        Segment seg = conn.segmentOrArc as Segment;
                        if (!GeometryTutorLib.Utilities.HasStructurally<Segment>(segments, seg))
                        {
                            seg.ClearCollinear();
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
                            arc.ClearCollinear();
                            arcs.Add(arc);
                            GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, arc.endpoint1);
                            GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, arc.endpoint2);
                        }
                    }
                }
            }

            // Combine all points into a single list.
            // Now doing these AFTER finding all intersections; unique intersection points will now be added to the list
            GeometryTutorLib.Utilities.AddUniqueList<Point>(points, figurePoints);

            //
            //
            // Find all intersections...among segments and arcs.
            //
            //
            // Segment-Segment
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    Point intersection = segments[s1].FindIntersection(segments[s2]);
                    intersection = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, intersection, segments[s1], segments[s2]);

                    if (intersection != null)
                    {
                        segments[s1].AddCollinearPoint(intersection);
                        segments[s2].AddCollinearPoint(intersection);
                        //If intersection is not already part of the points list, add it (this is to avoid creating edges with undefined nodes)
                        //GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, intersection);
                    }
                }
            }

            // Segment-Arc
            foreach (Segment segment in segments)
            {
                foreach (Arc arc in arcs)
                {
                    Point pt1 = null;
                    Point pt2 = null;
                    arc.theCircle.FindIntersection(segment, out pt1, out pt2);
                    pt1 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt1, segment, arc);
                    pt2 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt2, segment, arc);

                    if (pt1 != null)
                    {
                        segment.AddCollinearPoint(pt1);
                        arc.AddCollinearPoint(pt1);
                        //GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, pt1);
                    }
                    if (pt2 != null)
                    {
                        segment.AddCollinearPoint(pt2);
                        arc.AddCollinearPoint(pt2);
                        //GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, pt2);
                    }
                }
            }

            // Arc-Arc
            for (int a1 = 0; a1 < arcs.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < arcs.Count; a2++)
                {
                    Point pt1 = null;
                    Point pt2 = null;
                    arcs[a1].theCircle.FindIntersection(arcs[a2].theCircle, out pt1, out pt2);
                    pt1 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt1, arcs[a1], arcs[a2]);
                    pt2 = GeometryTutorLib.Utilities.AcquireRestrictedPoint(points, pt2, arcs[a1], arcs[a2]);

                    if (pt1 != null)
                    {
                        arcs[a1].AddCollinearPoint(pt1);
                        arcs[a2].AddCollinearPoint(pt1);
                        //GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, pt1);
                    }
                    if (pt2 != null)
                    {
                        arcs[a1].AddCollinearPoint(pt2);
                        arcs[a2].AddCollinearPoint(pt2);
                        //GeometryTutorLib.Utilities.AddStructurallyUnique<Point>(points, pt1);
                    }
                }
            }

            // Combine all points into a single list.
            //GeometryTutorLib.Utilities.AddUniqueList<Point>(points, figurePoints);

            segments = HandleCollinearSubSegments(segments);

            List <AtomicRegion> ars = AcquireAtomicRegionsFromGraph(points, segments, arcs, circles, circGranularity);

            //
            // The following code should not be required (in theory).
            //
            List <AtomicRegion> trueAtoms = new List<AtomicRegion>();

            for (int a1 = 0; a1 < ars.Count; a1++)
            {
                bool trueAtom = true;
                for (int a2 = 0; a2 < ars.Count; a2++)
                {
                    if (a1 != a2)
                    {
                        if (ars[a1].Contains(ars[a2]))
                        {
                            trueAtom = false;
                            break;
                        }

                    }
                }

                if (trueAtom) trueAtoms.Add(ars[a1]);
            }

            return trueAtoms;
        }

        public static List<AtomicRegion> AcquireAtomicRegionsFromGraph(List<Point> points, List<Segment> segments,
                                                                        List<Arc> arcs, List<Circle> circles, Dictionary<Circle, int> circGranularity)
        {
            //
            // Construct the Planar graph for atomic region identification.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph graph = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph();

            // Add the points as nodes in the graph.
            foreach (Point pt in points)
            {
                graph.AddNode(pt);
            }

            //
            // Edges are based on all the collinear relationships.
            //
            foreach (Segment segment in segments)
            {
                for (int p = 0; p < segment.collinear.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(segment.collinear[p], segment.collinear[p + 1],
                                            new Segment(segment.collinear[p], segment.collinear[p + 1]).Length,
                                            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
                }
            }

            foreach (Arc arc in arcs)
            {
                List<Point> applicWithMidpoints = GetArcPoints(arc, circGranularity);

                // Add the points to the graph
                foreach (Point pt in applicWithMidpoints)
                {
                    graph.AddNode(pt);
                }

                for (int p = 0; p < applicWithMidpoints.Count - 1; p++)
                {
                    graph.AddUndirectedEdge(applicWithMidpoints[p], applicWithMidpoints[p + 1],
                                            new Segment(applicWithMidpoints[p], applicWithMidpoints[p + 1]).Length,
                                            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.EdgeType.REAL_ARC);
                }
            }

            //
            // Convert the planar graph to atomic regions.
            //
            GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph copy = new GeometryTutorLib.Area_Based_Analyses.Atomizer.UndirectedPlanarGraph.PlanarGraph(graph);
            FacetCalculator atomFinder = new FacetCalculator(copy);
            List<Primitive> primitives = atomFinder.GetPrimitives();

            return PrimitiveToRegionConverter.Convert(graph, primitives, circles);
        }

        private static void HandleSegmentSegmentIntersections(List<Point> figurePoints, List<Point> points, List<Segment> segments, AtomicRegion outerBounds)
        {
            // Segment-Segment
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    Point intersection = segments[s1].FindIntersection(segments[s2]);
                    intersection = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, intersection);

                    if (intersection != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(intersection))
                        {
                            segments[s1].AddCollinearPoint(intersection);
                            segments[s2].AddCollinearPoint(intersection);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, intersection);
                        }
                    }
                }
            }
        }

        private static void HandleSegmentArcIntersections(List<Point> figurePoints, List<Point> points, List<Segment> segments, List<Arc> arcs, AtomicRegion outerBounds)
        {
            // Segment-Arc
            foreach (Segment segment in segments)
            {
                foreach (Arc arc in arcs)
                {
                    Point pt1 = null;
                    Point pt2 = null;
                    arc.theCircle.FindIntersection(segment, out pt1, out pt2);
                    pt1 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt1);
                    pt2 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt2);

                    if (pt1 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt1))
                        {
                            segment.AddCollinearPoint(pt1);
                            arc.AddCollinearPoint(pt1);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt1);
                        }
                    }
                    if (pt2 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt2))
                        {
                            segment.AddCollinearPoint(pt2);
                            arc.AddCollinearPoint(pt2);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt2);
                        }
                    }
                }
            }
        }

        private static void HandleArcArcIntersections(List<Point> figurePoints, List<Point> points, List<Arc> arcs, AtomicRegion outerBounds)
        {
            // Arc-Arc
            for (int a1 = 0; a1 < arcs.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < arcs.Count; a2++)
                {
                    Point pt1 = null;
                    Point pt2 = null;
                    arcs[a1].theCircle.FindIntersection(arcs[a2].theCircle, out pt1, out pt2);
                    pt1 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt1);
                    pt2 = GeometryTutorLib.Utilities.AcquirePoint(figurePoints, pt2);

                    if (pt1 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt1))
                        {
                            arcs[a1].AddCollinearPoint(pt1);
                            arcs[a2].AddCollinearPoint(pt1);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt1);
                        }
                    }
                    if (pt2 != null)
                    {
                        if (outerBounds.PointLiesOnOrInside(pt2))
                        {
                            arcs[a1].AddCollinearPoint(pt2);
                            arcs[a2].AddCollinearPoint(pt2);

                            // It may be the case that this intersection was due to a completely contained region.
                            GeometryTutorLib.Utilities.AddUnique<Point>(points, pt2);
                        }
                    }
                }
            }
        }

        private static List<Segment> HandleCollinearSubSegments(List<Segment> segments)
        {
            List<Segment> maximalSegments = new List<Segment>();

            // Look for collinear, subsegment situations.
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool maximal = true;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s2].HasSubSegment(segments[s1]))
                        {
                            maximal = false;
                            break;
                        }
                        if (segments[s1].HasSubSegment(segments[s2]))
                        {
                            segments[s1].AddCollinearPoints(segments[s2].collinear);
                        }
                    }
                }
                if (maximal) maximalSegments.Add(segments[s1]);
            }

            // To ensure proper points for all maximal segments; share among collinear
            for (int s1 = 0; s1 < maximalSegments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < maximalSegments.Count; s2++)
                {
                    if (maximalSegments[s1].IsCollinearWith(maximalSegments[s2]) && maximalSegments[s1].SharedVertex(maximalSegments[s2]) != null)
                    {
                        //segments[s1].AddCollinearPoints(segments[s2].collinear);
                        //segments[s2].AddCollinearPoints(segments[s1].collinear);

                        maximalSegments[s1].AddCollinearPoints(maximalSegments[s2].collinear);
                        maximalSegments[s2].AddCollinearPoints(maximalSegments[s1].collinear);
                    }
                }
            }

            return maximalSegments;
        }

        private static void HandleCollinearSubArcs(List<Arc> arcs)
        {
            // Look for collinear, subsegment situations.
            for (int a1 = 0; a1 < arcs.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < arcs.Count; a2++)
                {
                    if (arcs[a1].HasSubArc(arcs[a2]) || arcs[a2].HasSubArc(arcs[a1]))
                    {
                        arcs[a1].AddCollinearPoints(arcs[a2].collinear);
                        arcs[a2].AddCollinearPoints(arcs[a1].collinear);
                    }
                }
            }
        }
    }
}