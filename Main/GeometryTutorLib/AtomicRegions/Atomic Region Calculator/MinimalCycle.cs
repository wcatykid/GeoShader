using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using System.Linq;
using GeometryTutorLib.ConcreteAST;
using System;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class MinimalCycle : Primitive
    {
        // These points were ordered by the minimal basis algorithm; calculates facets.
        public List<Point> points;

        public MinimalCycle()
        {
            points = new List<Point>();
        }

        public void Add(Point pt)
        {
            points.Add(pt);
        }

        public void AddAll(List<Point> pts)
        {
            points.AddRange(pts);
        }

        public bool HasExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph)
        {
            return GetExtendedSegment(graph) != null;
        }

        public Segment GetExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph)
        {
            for (int p = 0; p < points.Count; p++)
            {
                if (graph.GetEdgeType(points[p], points[(p + 1) % points.Count]) == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT)
                {
                    return new Segment(points[p], points[p + 1 < points.Count ? p + 1 : 0]);
                }
            }

            return null;
        }

        public bool HasThisExtendedSegment(UndirectedPlanarGraph.PlanarGraph graph, Segment segment)
        {
            if (!points.Contains(segment.Point1)) return false;
            if (!points.Contains(segment.Point2)) return false;

            return graph.GetEdgeType(segment.Point1, segment.Point2) == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT;
        }

        private List<Point> GetPointsBookEndedBySegment(Segment segment)
        {
            int index1 = points.IndexOf(segment.Point1);
            int index2 = points.IndexOf(segment.Point2);

            // Are the points book-ended properly already?
            if (index1 == 0 && index2 == points.Count - 1) return new List<Point>(points);

            // The set to be returned.
            List<Point> ordered = new List<Point>();

            // Are the points book-ended in reverse?
            if (index1 == points.Count - 1 && index2 == 0)
            {
                for (int p = points.Count - 1; p >= 0; p--)
                {
                    ordered.Add(points[p]);
                }

                return ordered;
            }

            // The order is the same as specified by the segment; just cycle the points.
            if (index1 < index2)
            {
                for (int p = 0; p < points.Count; p++)
                {
                    int tempIndex = (index1 - p) < 0 ? points.Count + (index1 - p) : (index1 - p);
                    ordered.Add(points[(index1 - p) < 0 ? points.Count + (index1 - p) : (index1 - p)]);
                }
            }
            // The order is NOT the same as specified by the segment (it's reversed).
            else
            {
                for (int p = 0; p < points.Count; p++)
                {
                    int tempIndex = (index1 + p) % points.Count;
                    ordered.Add(points[(index1 + p) % points.Count]);
                }
            }

            return ordered;
        }

        public MinimalCycle Compose(MinimalCycle thatCycle, Segment extended)
        {
            MinimalCycle composed = new MinimalCycle();

            List<Point> thisPts = this.GetPointsBookEndedBySegment(extended);
            List<Point> thatPts = thatCycle.GetPointsBookEndedBySegment(extended);

            // Add all points from this;
            composed.AddAll(thisPts);

            // Add all points from that (excluding endpoints)
            for (int p = thatPts.Count - 2; p > 0; p--)
            {
                composed.Add(thatPts[p]);
            }

            return composed;
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append("Cycle { ");
            for (int p = 0; p < points.Count; p++)
            {
                str.Append(points[p].ToString());
                if (p < points.Count - 1) str.Append(", ");
            }
            str.Append(" }");

            return str.ToString();
        }

        //
        //
        // Create the actual set of atomic regions for this cycle.
        //
        //   We need to check to see if any of the cycle segments are based on arcs.
        //   We have to handle the degree of each segment: do many circles intersect at these points?
        //
        public List<Atomizer.AtomicRegion> ConstructAtomicRegions(List<Circle> circles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<Atomizer.AtomicRegion> regions = new List<Atomizer.AtomicRegion>();

            Atomizer.AtomicRegion region = null;

            //
            // Check for a direct polygon (no arcs).
            //
            region = PolygonDefinesRegion(graph);
            if (region != null)
            {
                regions.Add(region);
                return regions;
            }

            //
            // Does this region define a sector? 
            //
            List<AtomicRegion> sectors = SectorOrTruncationDefinesRegion(circles, graph);
            if (sectors != null && sectors.Any())
            {
                regions.AddRange(sectors);
                return regions;
            }

            //
            // Do we have a set of regions defined by a polygon in which circle(s) cut out some of that region? 
            //
            regions.AddRange(MixedArcChordedRegion(circles, graph));

            return regions;
        }

        private Atomizer.AtomicRegion PolygonDefinesRegion(UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<Segment> sides = new List<Segment>();

            //
            // All connections between adjacent connections MUST be segments.
            //
            for (int p = 0; p < points.Count; p++)
            {
                Segment segment = new Segment(points[p], points[(p + 1) % points.Count]);

                sides.Add(segment);

                if (graph.GetEdge(points[p], points[(p + 1) % points.Count]).edgeType != UndirectedPlanarGraph.EdgeType.REAL_SEGMENT) return null;
            }

            //
            // All iterative connections cannot be arcs.
            //
            for (int p1 = 0; p1 < points.Count - 1; p1++)
            {
                // We want to check for a direct cycle, therefore, p2 starts at p1 not p1 + 1
                for (int p2 = p1; p2 < points.Count; p2++)
                {
                    UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p1], points[(p2 + 1) % points.Count]);

                    if (edge != null)
                    {
                        if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_ARC) return null;
                    }
                }
            }

            //
            // Make the Polygon
            //
            Polygon poly = Polygon.MakePolygon(sides);

            if (poly == null) throw new ArgumentException("Real segments should define a polygon; they did not.");

            return new ShapeAtomicRegion(poly);
        }

        private List<Atomizer.AtomicRegion> SectorOrTruncationDefinesRegion(List<Circle> circles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            //
            // Do there exist any real-dual edges or extended segments? If so, this is not a sector.
            //
            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.EXTENDED_SEGMENT) return null;
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_DUAL) return null;
            }

            //
            // Collect all segments; split into two collinear lists.
            //
            List<Segment> segments = CollectSegments(graph);
            List<List<Segment>> collinearSegmentSet = SplitSegmentsIntoCollinearSequences(segments);

            // A sector requires one (semicircl) or two sets of segments ('normal' arc).
            if (collinearSegmentSet.Count > 2) return null;

            //
            // Collect all arcs.
            //
            List<MinorArc> arcs = CollectStrictArcs(circles, graph);
            List<List<MinorArc>> collinearArcSet = SplitArcsIntoCollinearSequences(arcs);

            // A sector requires one set of arcs (no more, no less).
            if (collinearArcSet.Count != 1) return null;

            // Semicircle has one set of sides
            if (collinearSegmentSet.Count == 1) return ConvertToTruncationOrSemicircle(collinearSegmentSet[0], collinearArcSet[0]);

            // Pacman shape created with a circle results in Sector
            return ConvertToGeneralSector(collinearSegmentSet[0], collinearSegmentSet[1], collinearArcSet[0]);
        }

        //
        // Collect all segments attributed to this this cycle
        //
        private List<Segment> CollectSegments(UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<Segment> segments = new List<Segment>();

            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_SEGMENT)
                {
                    segments.Add(new Segment(points[p], points[(p + 1) % points.Count]));
                }
            }

            return segments;
        }

        //
        // Split the segments into sets of collinear segments.
        // NOTE: This code assumes an input ordering of segments and returns sets of ordered collinear segments.
        //
        private List<List<Segment>> SplitSegmentsIntoCollinearSequences(List<Segment> segments)
        {
            List<List<Segment>> collinearSet = new List<List<Segment>>();

            foreach (Segment segment in segments)
            {
                bool collinearFound = false;
                foreach (List<Segment> collinear in collinearSet)
                {
                    // Find the set of collinear segments
                    if (segment.IsCollinearWith(collinear[0]))
                    {
                        collinearFound = true;
                        int i = 0;
                        for (i = 0; i < collinear.Count; i++)
                        {
                            if (segment.Point2.StructurallyEquals(collinear[i].Point1)) break;
                        }
                        collinear.Insert(i, segment);
                    }
                }

                if (!collinearFound) collinearSet.Add(Utilities.MakeList<Segment>(segment));
            }

            return collinearSet;
        }

        //
        // Collect all arcs attributed to this this cycle; 
        //
        private List<MinorArc> CollectStrictArcs(List<Circle> circles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<MinorArc> minors = new List<MinorArc>();

            for (int p = 0; p < points.Count; p++)
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);

                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_ARC)
                {
                    // Find the applicable circle.
                    Circle theCircle = null;
                    foreach (Circle circle in circles)
                    {
                        if (circle.HasArc(points[p], points[(p + 1) % points.Count]))
                        {
                            theCircle = circle;
                            break;
                        }
                    }

                    minors.Add(new MinorArc(theCircle, points[p], points[(p + 1) % points.Count]));
                }
            }

            return minors;
        }

        //
        // Split the segments into sets of collinear segments.
        // NOTE: This code assumes an input ordering of segments and returns sets of ordered collinear segments.
        //
        private List<List<MinorArc>> SplitArcsIntoCollinearSequences(List<MinorArc> minors)
        {
            List<List<MinorArc>> collinearSet = new List<List<MinorArc>>();

            //
            // Collect all the related arcs
            //
            foreach (MinorArc minor in minors)
            {
                bool collinearFound = false;
                foreach (List<MinorArc> collinear in collinearSet)
                {
                    // Do the arcs belong to the same circle?
                    if (minor.theCircle.StructurallyEquals(collinear[0].theCircle))
                    {
                        collinearFound = true;
                        int i = 0;
                        for (i = 0; i < collinear.Count; i++)
                        {
                            if (minor.endpoint2.StructurallyEquals(collinear[i].endpoint1)) break;
                        }
                        collinear.Insert(i, minor);
                    }
                }

                if (!collinearFound) collinearSet.Add(Utilities.MakeList<MinorArc>(minor));
            }

            //
            // Sort each arc set.
            //
            for (int arcSetIndex = 0; arcSetIndex < collinearSet.Count; arcSetIndex++)
            {
                collinearSet[arcSetIndex] = SortArcSet(collinearSet[arcSetIndex]);
            }

            return collinearSet;
        }

        //
        // Order the arcs so the endpoints are clear in the first in last positions.
        //
        private List<MinorArc> SortArcSet(List<MinorArc> arcs)
        {
            if (arcs.Count <= 2) return arcs;

            bool[] marked = new bool[arcs.Count];
            List<MinorArc> sorted = new List<MinorArc>();

            //
            // Find the 'first' endpoint of the arc.
            //
            int sharedCount = 0;
            int arcIndex = -1;
            for (int a1 = 0; a1 < arcs.Count; a1++)
            {
                sharedCount = 0;
                for (int a2 = 0; a2 < arcs.Count; a2++)
                {
                    if (a1 != a2)
                    {
                        if (arcs[a1].SharedEndpoint(arcs[a2]) != null) sharedCount++;
                    }
                }
                arcIndex = a1;
                if (sharedCount == 1) break;
            }

            // An 'end'-arc found; book-ends of list.
            switch(sharedCount)
            {
                case 0:
                    throw new Exception("Expected a shared count of 1 or 2, not 0");
                case 1:
                    sorted.Add(arcs[arcIndex]);
                    marked[arcIndex] = true;
                    break;
                case 2:
                    // Middle arc
                    break;
                default:
                    throw new Exception("Expected a shared count of 1 or 2, not (" + sharedCount + ")");
            }

            MinorArc working = sorted[0];
            while (marked.Contains(false))
            {
                Point shared;
                for (arcIndex = 0; arcIndex < arcs.Count; arcIndex++)
                {
                    if (!marked[arcIndex])
                    {
                        shared = working.SharedEndpoint(arcs[arcIndex]);
                        if (shared != null) break;
                    }
                }
                marked[arcIndex] = true;
                sorted.Add(arcs[arcIndex]);
                working = arcs[arcIndex];
            }

            return sorted;
        }

        private List<Atomizer.AtomicRegion> ConvertToGeneralSector(List<Segment> sideSet1, List<Segment> sideSet2, List<MinorArc> arcs)
        {
            Segment side1 = ComposeSegmentsIntoSegment(sideSet1);
            Segment side2 = ComposeSegmentsIntoSegment(sideSet2);
            Arc theArc = ComposeArcsIntoArc(arcs);

            //
            // Verify that both sides of the sector contains the center.
            // And many other tests to ensure proper sector acquisition.
            //
            if (!side1.HasPoint(theArc.theCircle.center)) return null;
            if (!side2.HasPoint(theArc.theCircle.center)) return null;

            Point sharedCenter = side1.SharedVertex(side2);
            if (sharedCenter == null)
            {
                throw new Exception("Sides do not share a vertex as expected; they share " + sharedCenter);
            }

            if (!sharedCenter.StructurallyEquals(theArc.theCircle.center))
            {
                throw new Exception("Center and deduced center do not equate: " + sharedCenter + " " + theArc.theCircle.center);
            }

            Point segEndpoint1 = side1.OtherPoint(sharedCenter);
            Point segEndpoint2 = side2.OtherPoint(sharedCenter);

            if (!theArc.HasEndpoint(segEndpoint1) || !theArc.HasEndpoint(segEndpoint2))
            {
                throw new Exception("Side endpoints do not equate to the arc endpoints");
            }

            // Satisfied constraints, create the actual sector.
            Sector sector = new Sector(theArc);

            return Utilities.MakeList<AtomicRegion>(new ShapeAtomicRegion(sector));
        }

        private List<Atomizer.AtomicRegion> ConvertToTruncationOrSemicircle(List<Segment> sideSet, List<MinorArc> arcs)
        {
            Segment side = ComposeSegmentsIntoSegment(sideSet);
            Arc theArc = ComposeArcsIntoArc(arcs);

            // Verification Step 1.
            if (!theArc.HasEndpoint(side.Point1) || !theArc.HasEndpoint(side.Point2))
            {
                throw new Exception("Semicircle / Truncation: Side endpoints do not equate to the arc endpoints");
            }

            if (theArc is Semicircle) return ConvertToSemicircle(side, theArc as Semicircle);
            return ConvertToTruncation(side, theArc as MinorArc);
        }

        private List<Atomizer.AtomicRegion> ConvertToTruncation(Segment chord, MinorArc arc)
        {
            AtomicRegion atom = new AtomicRegion();

            atom.AddConnection(new Connection(chord.Point1, chord.Point2, ConnectionType.SEGMENT, chord));

            atom.AddConnection(new Connection(chord.Point1, chord.Point2, ConnectionType.ARC, arc));

            return Utilities.MakeList<AtomicRegion>(atom);
        }

        private List<Atomizer.AtomicRegion> ConvertToSemicircle(Segment diameter, Semicircle semi)
        {
            // Verification Step 2.
            if (!diameter.PointLiesOnAndExactlyBetweenEndpoints(semi.theCircle.center))
            {
                throw new Exception("Semicircle: expected center between endpoints.");
            }

            Sector sector = new Sector(semi);

            return Utilities.MakeList<AtomicRegion>(new ShapeAtomicRegion(sector));
        }

        private Segment ComposeSegmentsIntoSegment(List<Segment> segments)
        {
            return new Segment(segments[0].Point1, segments[segments.Count - 1].Point2);
        }

        private Arc ComposeArcsIntoArc(List<MinorArc> minors)
        {
            // if (minors.Count == 1) return minors[0];

            // Determine what type of arc to create.
            double arcMeasure = 0;
            foreach (MinorArc minor in minors)
            {
                arcMeasure += minor.minorMeasure;
            }

            //
            // Create the arc
            //

            // Determine the proper endpoints.
            Point endpt1 = minors[0].OtherEndpoint(minors[0].SharedEndpoint(minors[1]));
            Point endpt2 = minors[minors.Count-1].OtherEndpoint(minors[minors.Count-1].SharedEndpoint(minors[minors.Count-2]));

            // Create the proper arc.
            Circle theCircle = minors[0].theCircle;

            if (Utilities.CompareValues(arcMeasure, 180))
            {
                Segment diameter = new Segment(endpt1, endpt2);

                // Get the midpoint that is on the same side.
                Point midpt = theCircle.Midpoint(diameter.Point1, diameter.Point2, minors[0].endpoint2);
                return new Semicircle(minors[0].theCircle, diameter.Point1, diameter.Point2, midpt, diameter);
            }
            else if (arcMeasure < 180) return new MinorArc(theCircle, endpt1, endpt2);
            else if (arcMeasure > 180) return new MajorArc(theCircle, endpt1, endpt2);

            return null;
        }

        private List<Circle> GetAllApplicableCircles(List<Circle> circles, Point pt1, Point pt2)
        {
            List<Circle> applicCircs = new List<Circle>();

            foreach (Circle circle in circles)
            {
                if (circle.PointLiesOn(pt1) && circle.PointLiesOn(pt2))
                {
                    applicCircs.Add(circle);
                }
            }

            return applicCircs;
        }

        private List<Atomizer.AtomicRegion> MixedArcChordedRegion(List<Circle> thatCircles, UndirectedPlanarGraph.PlanarGraph graph)
        {
            List<AtomicRegion> regions = new List<AtomicRegion>();

            // Every segment may be have a set of circles. (on each side) surrounding it.
            // Keep parallel lists of: (1) segments, (2) (real) arcs, (3) left outer circles, and (4) right outer circles
            Segment[] regionsSegments = new Segment[points.Count];
            Arc[] arcSegments = new Arc[points.Count];
            Circle[] leftOuterCircles = new Circle[points.Count];
            Circle[] rightOuterCircles = new Circle[points.Count];

            //
            // Populate the parallel arrays.
            //
            int currCounter = 0;
            for (int p = 0; p < points.Count; )
            {
                UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(points[p], points[(p + 1) % points.Count]);
                Segment currSegment = new Segment(points[p], points[(p + 1) % points.Count]);

                //
                // If a known segment, seek a sequence of collinear segments.
                //
                if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_SEGMENT)
                {
                    Segment actualSeg = currSegment;

                    bool collinearExists = false;
                    int prevPtIndex;
                    for (prevPtIndex = p + 1; prevPtIndex < points.Count; prevPtIndex++)
                    {
                        // Make another segment with the next point.
                        Segment nextSeg = new Segment(points[p], points[(prevPtIndex + 1) % points.Count]);

                        // CTA: This criteria seems invalid in some cases....; may not have collinearity

                        // We hit the end of the line of collinear segments.
                        if (!currSegment.IsCollinearWith(nextSeg)) break;

                        collinearExists = true;
                        actualSeg = nextSeg;
                    }

                    // If there exists an arc over the actual segment, we have an embedded circle to consider.
                    regionsSegments[currCounter] = actualSeg;

                    if (collinearExists)
                    {
                        UndirectedPlanarGraph.PlanarGraphEdge collEdge = graph.GetEdge(actualSeg.Point1, actualSeg.Point2);
                        if (collEdge != null)
                        {
                            if (collEdge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_ARC)
                            {
                                // Find all applicable circles
                                List<Circle> circles = GetAllApplicableCircles(thatCircles, actualSeg.Point1, actualSeg.Point2);

                                // Get the exact outer circles for this segment (and create any embedded regions).
                                regions.AddRange(ConvertToCircleCircle(actualSeg, circles, out leftOuterCircles[currCounter], out rightOuterCircles[currCounter]));
                            }
                        }
                    }

                    currCounter++;
                    p = prevPtIndex;
                }
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_DUAL)
                {
                    regionsSegments[currCounter] = new Segment(points[p], points[(p + 1) % points.Count]);

                    // Get the exact chord and set of circles
                    Segment chord = regionsSegments[currCounter];

                    // Find all applicable circles
                    List<Circle> circles = GetAllApplicableCircles(thatCircles, points[p], points[(p + 1) % points.Count]);

                    // Get the exact outer circles for this segment (and create any embedded regions).
                    regions.AddRange(ConvertToCircleCircle(chord, circles, out leftOuterCircles[currCounter], out rightOuterCircles[currCounter]));

                    currCounter++;
                    p++;
                }
                else if (edge.edgeType == UndirectedPlanarGraph.EdgeType.REAL_ARC)
                {
                    //
                    // Find the unique circle that contains these two points.
                    // (if more than one circle has these points, we would have had more intersections and it would be a direct chorded region)
                    //
                    List<Circle> circles = GetAllApplicableCircles(thatCircles, points[p], points[(p + 1) % points.Count]);

                    if (circles.Count != 1) throw new Exception("Need ONLY 1 circle for REAL_ARC atom id; found (" + circles.Count + ")");

                    arcSegments[currCounter++] = new MinorArc(circles[0], points[p], points[(p + 1) % points.Count]);

                    p++;
                }
            }

            //
            // Check to see if this is a region in which some connections are segments and some are arcs.
            // This means there were no REAL_DUAL edges.
            //
            List<AtomicRegion> generalRegions = GeneralAtomicRegion(regionsSegments, arcSegments);
            if (generalRegions.Any()) return generalRegions;

            // Copy the segments into a list (ensuring no nulls)
            List<Segment> actSegments = new List<Segment>();
            foreach (Segment side in regionsSegments)
            {
                if (side != null) actSegments.Add(side);
            }

            // Construct a polygon out of the straight-up segments
            // This might be a polygon that defines a pathological region.
            Polygon poly = Polygon.MakePolygon(actSegments);

            // Determine which outermost circles apply inside of this polygon.
            Circle[] circlesCutInsidePoly = new Circle[actSegments.Count];
            for (int p = 0; p < actSegments.Count; p++)
            {
                if (leftOuterCircles[p] != null && rightOuterCircles[p] == null)
                {
                    circlesCutInsidePoly[p] = CheckCircleCutInsidePolygon(poly, leftOuterCircles[p], actSegments[p].Point1, actSegments[p].Point2);
                }
                else if (leftOuterCircles[p] == null && rightOuterCircles[p] != null)
                {
                    circlesCutInsidePoly[p] = CheckCircleCutInsidePolygon(poly, rightOuterCircles[p], actSegments[p].Point1, actSegments[p].Point2);
                }
                else if (leftOuterCircles[p] != null && rightOuterCircles[p] != null)
                {
                    circlesCutInsidePoly[p] = CheckCircleCutInsidePolygon(poly, leftOuterCircles[p], actSegments[p].Point1, actSegments[p].Point2);

                    if (circlesCutInsidePoly[p] == null) circlesCutInsidePoly[p] = rightOuterCircles[p];
                }
                else
                {
                    circlesCutInsidePoly[p] = null;
                }
            }

            bool isStrictPoly = true;
            for (int p = 0; p < actSegments.Count; p++)
            {
                if (circlesCutInsidePoly[p] != null || arcSegments[p] != null)
                {
                    isStrictPoly = false;
                    break;
                }
            }

            // This is just a normal shape region: polygon.
            if (isStrictPoly)
            {
                regions.Add(new ShapeAtomicRegion(poly));
            }
            // A circle cuts into the polygon.
            else
            {
                //
                // Now that all interior arcs have been identified, construct the atomic (probably pathological) region
                //
                AtomicRegion pathological = new AtomicRegion();
                for (int p = 0; p < actSegments.Count; p++)
                {
                    //
                    // A circle cutting inside the polygon
                    //
                    if (circlesCutInsidePoly[p] != null)
                    {
                        Arc theArc = null;

                        if (circlesCutInsidePoly[p].DefinesDiameter(regionsSegments[p]))
                        {
                            Point midpt = circlesCutInsidePoly[p].Midpoint(regionsSegments[p].Point1, regionsSegments[p].Point2);

                            if (!poly.IsInPolygon(midpt)) midpt = circlesCutInsidePoly[p].OppositePoint(midpt);

                            theArc = new Semicircle(circlesCutInsidePoly[p], regionsSegments[p].Point1, regionsSegments[p].Point2, midpt, regionsSegments[p]);
                        }
                        else
                        {
                            theArc = new MinorArc(circlesCutInsidePoly[p], regionsSegments[p].Point1, regionsSegments[p].Point2);
                        }

                        pathological.AddConnection(regionsSegments[p].Point1, regionsSegments[p].Point2, ConnectionType.ARC, theArc);
                    }
                    //
                    else
                    {
                        // We have a direct arc
                        if (arcSegments[p] != null)
                        {
                            pathological.AddConnection(regionsSegments[p].Point1, regionsSegments[p].Point2,
                                                       ConnectionType.ARC, arcSegments[p]);
                        }
                        // Use the segment
                        else
                        {
                            pathological.AddConnection(regionsSegments[p].Point1, regionsSegments[p].Point2,
                                                       ConnectionType.SEGMENT, regionsSegments[p]);
                        }
                    }
                }

                regions.Add(pathological);
            }


            return regions;
        }

        //
        // Determine if this is a true polygon situation or if it is a sequence of segments and arcs.
        //
        private List<AtomicRegion> GeneralAtomicRegion(Segment[] segments, Arc[] arcs)
        {
            List<AtomicRegion> regions = new List<AtomicRegion>();

            //
            // Determine if the parts are all segments.
            // Concurrently determine the proper starting point in the sequence to construct the atomic region.
            //
            bool hasArc = false;
            bool hasSegment = false;
            int startIndex = 0;
            for (int i = 0; i < segments.Length && i < arcs.Length; i++)
            {
                // Both an arc and a segment.
                if (segments[i] != null && arcs[i] != null) return regions;

                // Determine if we have an arc and/or a segment.
                if (segments[i] != null) hasSegment = true;
                if (arcs[i] != null) hasArc = true;

                // A solid starting point is an arc right after a null.
                if (arcs[i] == null && arcs[(i + 1) % arcs.Length] != null)
                {
                    // Assign only once to the startIndex
                    if (startIndex == 0) startIndex = (i + 1) % arcs.Length;
                }
            }

            // If only segments, we have a polygon.
            if (hasSegment && !hasArc) return regions;

            //
            // If the set ONLY consists of arcs, ensure we have a good starting point.
            //
            if (hasArc && !hasSegment)
            {
                // Seek the first index where a change among arcs occurs.
                for (int i = 0; i < arcs.Length; i++)
                {
                    // A solid starting point is an arc right after a null.
                    if (!arcs[i].theCircle.StructurallyEquals(arcs[(i + 1) % arcs.Length].theCircle))
                    {
                        startIndex = (i + 1) % arcs.Length;
                        break;
                    }
                }
            }

            AtomicRegion theRegion = new AtomicRegion();
            for (int i = 0; i < segments.Length && i < arcs.Length; i++)
            {
                int currIndex = (i + startIndex) % arcs.Length;

                if (segments[currIndex] == null && arcs[currIndex] == null) { /* No-Op */ }

                if (segments[currIndex] != null)
                {
                    theRegion.AddConnection(new Connection(segments[currIndex].Point1,
                                                           segments[currIndex].Point2, ConnectionType.SEGMENT, segments[currIndex]));
                }
                else if (arcs[currIndex] != null)
                {
                    //
                    // Compose the arcs (from a single circle) together.
                    //
                    List<MinorArc> sequentialArcs = new List<MinorArc>();
                    sequentialArcs.Add(arcs[currIndex] as MinorArc);

                    int seqIndex;
                    for (seqIndex = (currIndex + 1) % arcs.Length; ; seqIndex = (seqIndex + 1) % arcs.Length, i++)
                    {
                        if (arcs[seqIndex] == null) break;

                        if (arcs[currIndex].theCircle.StructurallyEquals(arcs[seqIndex].theCircle))
                        {
                            sequentialArcs.Add(arcs[seqIndex] as MinorArc);
                        }
                        else break;
                    }

                    Arc composed;
                    if (sequentialArcs.Count > 1) composed = this.ComposeArcsIntoArc(sequentialArcs);
                    else composed = arcs[currIndex];

                    //
                    // Add the connection.
                    //
                    theRegion.AddConnection(new Connection(composed.endpoint1, composed.endpoint2, ConnectionType.ARC, composed));
                }
            }

            return Utilities.MakeList<AtomicRegion>(theRegion);
        }

        private Circle CheckCircleCutInsidePolygon(Polygon poly, Circle circle, Point pt1, Point pt2)
        {
            Segment diameter = new Segment(pt1, pt2);

            // A semicircle always cuts into the polygon.
            if (circle.DefinesDiameter(diameter)) return circle;
            else
            {
                // Is the midpoint on the interior of the polygon?
                Point midpt = circle.Midpoint(pt1, pt2);

                // Is this point in the interior of this polygon?
                if (poly.IsInPolygon(midpt)) return circle;
            }

            return null;
        }

        //
        // This is a complex situation because we need to identify situations where circles intersect with the resultant regions:
        //    (|     (|)
        //   ( |    ( | )
        //  (  |   (  |  )
        //   ( |    ( | )
        //    (|     (|)
        //
        // Note: There will always be a chord because of our implied construction.
        // We are interested in only minor arcs of the given circles.
        //
        private List<Atomizer.AtomicRegion> ConvertToCircleCircle(Segment chord,
                                                                  List<Circle> circles,
                                                                  out Circle leftOuterCircle,
                                                                  out Circle rightOuterCircle)
        {
            List<Atomizer.AtomicRegion> regions = new List<Atomizer.AtomicRegion>();
            leftOuterCircle = null;
            rightOuterCircle = null;

            //
            // Simple cases that require no special attention.
            //
            if (!circles.Any()) return null;
            if (circles.Count == 1)
            {
                leftOuterCircle = circles[0];

                regions.AddRange(ConstructBasicLineCircleRegion(chord, circles[0]));

                return regions;
            }

            // All circles that are on each side of the chord 
            List<Circle> leftSide = new List<Circle>();
            List<Circle> rightSide = new List<Circle>();

            // For now, assume max, one circle per side.
            // Construct a collinear list of points that includes all circle centers as well as the single intersection point between the chord and the line passing through all circle centers.
            // This orders the sides and provides implied sizes.

            Segment centerLine = new Segment(circles[0].center, circles[1].center);
            for (int c = 2; c < circles.Count; c++)
            {
                centerLine.AddCollinearPoint(circles[c].center);
            }
            // Find the intersection between the center-line and the chord; add that to the list.
            Point intersection = centerLine.FindIntersection(chord);
            centerLine.AddCollinearPoint(intersection);

            List<Point> collPoints = centerLine.collinear;
            int interIndex = collPoints.IndexOf(intersection);

            for (int i = 0; i < collPoints.Count; i++)
            {
                // find the circle based on center
                int c;
                for (c = 0; c < circles.Count; c++)
                {
                    if (circles[c].center.StructurallyEquals(collPoints[i])) break;
                }

                // Add the circle in order
                if (i < interIndex) leftSide.Add(circles[c]);
                else if (i > interIndex) rightSide.Add(circles[c]);
            }

            // the outermost circle is first in the left list and last in the right list.
            if (leftSide.Any()) leftOuterCircle = leftSide[0];
            if (rightSide.Any()) rightOuterCircle = rightSide[rightSide.Count - 1];

            //
            // Main combining algorithm:
            //     Assume: Increasing Arc sequence A \in A_1, A_2, ..., A_n and the single chord C
            //
            //     Construct region B = (C, A_1)
            //     For the increasing Arc sequence (k subscript)  A_2, A_3, ..., A_n
            //         B = Construct ((C, A_k) \ B)
            //         
            // Alternatively:
            //     Construct(C, A_1)
            //     for each pair Construct (A_k, A_{k+1})
            //
            //
            // Handle each side: left and right.
            //
            if (leftSide.Any()) regions.AddRange(ConstructBasicLineCircleRegion(chord, leftSide[leftSide.Count - 1]));
            for (int ell = 0; ell < leftSide.Count - 2; ell++)
            {
                regions.Add(ConstructBasicCircleCircleRegion(chord, leftSide[ell], leftSide[ell + 1]));
            }

            if (rightSide.Any()) regions.AddRange(ConstructBasicLineCircleRegion(chord, rightSide[0]));
            for (int r = 1; r < rightSide.Count - 1; r++)
            {
                regions.Add(ConstructBasicCircleCircleRegion(chord, rightSide[r], rightSide[r + 1]));
            }

            return regions;
        }

        // Construct the region between a chord and the circle arc:
        //    (|
        //   ( |
        //  (  |
        //   ( |
        //    (|
        //
        private List<AtomicRegion> ConstructBasicLineCircleRegion(Segment chord, Circle circle)
        {
            //
            // Standard
            //
            if (!circle.DefinesDiameter(chord))
            {
                AtomicRegion region = new AtomicRegion();

                Arc theArc = new MinorArc(circle, chord.Point1, chord.Point2);

                region.AddConnection(chord.Point1, chord.Point2, ConnectionType.ARC, theArc);

                region.AddConnection(chord.Point1, chord.Point2, ConnectionType.SEGMENT, chord);

                return Utilities.MakeList<AtomicRegion>(region);
            }

            //
            // Semi-circles
            //

            Point midpt = circle.Midpoint(chord.Point1, chord.Point2);
            Arc semi1 = new Semicircle(circle, chord.Point1, chord.Point2, midpt, chord);
            ShapeAtomicRegion region1 = new ShapeAtomicRegion(new Sector(semi1));

            Point opp = circle.OppositePoint(midpt);
            Arc semi2 = new Semicircle(circle, chord.Point1, chord.Point2, opp, chord);
            ShapeAtomicRegion region2 = new ShapeAtomicRegion(new Sector(semi2));

            List<AtomicRegion> regions = new List<AtomicRegion>();
            regions.Add(region1);
            regions.Add(region2);

            return regions;
        }

        // Construct the region between a circle and circle:
        //     __
        //    ( (
        //   ( ( 
        //  ( (  
        //   ( ( 
        //    ( (
        //     --
        private Atomizer.AtomicRegion ConstructBasicCircleCircleRegion(Segment chord, Circle smaller, Circle larger)
        {
            AtomicRegion region = new AtomicRegion();

            Arc arc1 = null;
            if (smaller.DefinesDiameter(chord))
            {
                Point midpt = smaller.Midpoint(chord.Point1, chord.Point2, larger.Midpoint(chord.Point1, chord.Point2));

                arc1 = new Semicircle(smaller, chord.Point1, chord.Point2, midpt, chord);
            }
            else
            {
                arc1 = new MinorArc(smaller, chord.Point1, chord.Point2);
            }

            MinorArc arc2 = new MinorArc(larger, chord.Point1, chord.Point2);

            region.AddConnection(chord.Point1, chord.Point2, ConnectionType.ARC, arc1);

            region.AddConnection(chord.Point1, chord.Point2, ConnectionType.ARC, arc2);

            return region;
        }
    }
}