using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.TutorParser
{
    /// <summary>
    /// Determine all points of intersection among shapes (circles and polygons)
    /// </summary>
    public class ShapeIntersectionCalculator
    {
        private ImpliedComponentCalculator implied;

        public ShapeIntersectionCalculator(ImpliedComponentCalculator imp)
        {
            implied = imp;
        }

        /// <summary>
        /// Calculate all points of intersection between circles.
        /// Updates segments by creating segments.
        /// </summary>
        public void CalcCircleCircleIntersections(out List<GeometryTutorLib.ConcreteAST.CircleCircleIntersection> ccIntersections)
        {
            ccIntersections = new List<CircleCircleIntersection>();
            
            for (int c1 = 0; c1 < implied.circles.Count - 1; c1++)
            {
                for (int c2 = c1 + 1; c2 < implied.circles.Count; c2++)
                {
                    //
                    // Find any intersection points between the circle and the segment;
                    // the intersection MUST be between the segment endpoints
                    //
                    Point inter1 = null;
                    Point inter2 = null;
                    implied.circles[c1].FindIntersection(implied.circles[c2], out inter1, out inter2);

                    List<Point> intersectionPts = new List<Point>();
                    if (inter1 != null) intersectionPts.Add(inter1);
                    if (inter2 != null) intersectionPts.Add(inter2);

                    // normalized to drawing point (names)
                    intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                    // and add to each figure (circle and polygon).
                    implied.circles[c1].AddIntersectingPoints(intersectionPts);
                    implied.circles[c2].AddIntersectingPoints(intersectionPts);

                    //
                    // Construct the intersections
                    //
                    CircleCircleIntersection ccInter = null;

                    if (inter1 != null)
                    {
                        ccInter = new CircleCircleIntersection(inter1, implied.circles[c1], implied.circles[c2]);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleCircleIntersection>(ccIntersections, ccInter);
                    }
                    if (inter2 != null)
                    {
                        ccInter = new CircleCircleIntersection(inter2, implied.circles[c1], implied.circles[c2]);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleCircleIntersection>(ccIntersections, ccInter);
                    }

                    // Add an implied collinear relationship so that the appropriate segments are generated.
                    // ONLY for tangent situations.
                    if (inter1 != null && inter2 == null)
                    {
                        //Determine the endpoints (the intersection point could be an endpoint if the tangency is internal)
                        Point e1, e2, m;
                        Point center1 = implied.circles[c1].center;
                        Point center2 = implied.circles[c2].center;
                        if (GeometryTutorLib.ConcreteAST.Segment.Between(inter1, center1, center2))
                        {
                            e1 = center1;
                            e2 = center2;
                            m = inter1;
                        }
                        else if (GeometryTutorLib.ConcreteAST.Segment.Between(center1, inter1, center2))
                        {
                            e1 = inter1;
                            e2 = center2;
                            m = center1;
                        }
                        else
                        {
                            e1 = inter1;
                            e2 = center1;
                            m = center2;
                        }
                        Collinear coll = new Collinear();
                        coll.AddCollinearPoint(e1);
                        coll.AddCollinearPoint(m);
                        coll.AddCollinearPoint(e2);
                        implied.collinear.Add(coll);
                    }
                }
            }

            // Construct any radii and chords.
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                AddImpliedSegments(circle);
            }

        }

        //
        // If no radii are drawn, construct them as well as the chords connecting them.
        //
        private void AddImpliedSegments(GeometryTutorLib.ConcreteAST.Circle circle)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> constructedChords = new List<GeometryTutorLib.ConcreteAST.Segment>();
            List<GeometryTutorLib.ConcreteAST.Segment> constructedRadii = new List<GeometryTutorLib.ConcreteAST.Segment>();
            List<Point> imagPoints = new List<Point>();

            List<GeometryTutorLib.ConcreteAST.Point> interPts = circle.GetIntersectingPoints();

            // If there are no points of interest, the circle is the atomic region.
            if (!interPts.Any()) return;

            // Construct the radii
            foreach (Point interPt in interPts)
            {
                GeometryTutorLib.Utilities.AddStructurallyUnique<GeometryTutorLib.ConcreteAST.Segment>(implied.segments,
                                                                                                       new GeometryTutorLib.ConcreteAST.Segment(circle.center, interPt));
            }

            // Construct the chords
            for (int p1 = 0; p1 < interPts.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < interPts.Count; p2++)
                {
                    GeometryTutorLib.Utilities.AddStructurallyUnique<GeometryTutorLib.ConcreteAST.Segment>(implied.segments,
                                                                                                           new GeometryTutorLib.ConcreteAST.Segment(interPts[p1], interPts[p2]));
                }
            }
        }

        /// <summary>
        /// Calculate all points of intersection between circles and polygons.
        /// </summary>
        public void CalcCirclePolygonIntersectionPoints()
        {
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                // Iterate over all polygons
                for (int sidesIndex = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                     sidesIndex < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                     sidesIndex++)
                {
                    foreach (GeometryTutorLib.ConcreteAST.Polygon poly in implied.polygons[sidesIndex])
                    {
                        // Get the list of intersection points
                        List<Point> intersectionPts = poly.FindIntersections(circle);

                        // normalized to drawing point (names)
                        intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                        // and add to each figure (circle and polygon).
                        poly.AddIntersectingPoints(intersectionPts);
                        circle.AddIntersectingPoints(intersectionPts);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all points of intersection between polygons and polygons.
        /// </summary>
        public void CalcPolygonPolygonIntersectionPoints()
        {
            for (int s1 = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                 s1 < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                 s1++)
            {
                for (int p1 = 0; p1 < implied.polygons[s1].Count; p1++)
                {
                    for (int s2 = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                         s2 < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                         s2++)
                    {
                        for (int p2 = 0; p2 < implied.polygons[s2].Count; p2++)
                        {
                            if (s1 != s2 || p1 != p2)
                            {
                                // Get the list of intersection points
                                List<Point> intersectionPts = implied.polygons[s1][p1].FindIntersections(implied.polygons[s2][p2]);

                                // normalized to drawing point (names)
                                intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                                // and add to each figure (circle and polygon).
                                implied.polygons[s1][p1].AddIntersectingPoints(intersectionPts);
                                implied.polygons[s2][p2].AddIntersectingPoints(intersectionPts);
                            }
                        }
                    }
                }
            }
        }

        //
        // Find every point of intersection among segments (if they are not labeled in the UI) -- name them.
        //
        public List<CircleSegmentIntersection> FindCircleSegmentIntersections()
        {
            List<CircleSegmentIntersection> intersections = new List<CircleSegmentIntersection>();

            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.segments)
                {
                    Point inter1 = null;
                    Point inter2 = null;
                    circle.FindIntersection(segment, out inter1, out inter2);

                    if (!segment.PointLiesOnAndBetweenEndpoints(inter1)) inter1 = null;
                    if (!segment.PointLiesOnAndBetweenEndpoints(inter2)) inter2 = null;

                    // Analyze this segment w.r.t. to this circle: tangent, secant, chord.
                    if (inter1 != null || inter2 != null)
                    {
                        circle.AnalyzeSegment(segment, implied.allFigurePoints);
                    }
                }
            }

            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                foreach (GeometryTutorLib.ConcreteAST.Segment segment in implied.maximalSegments)
                {
                    //
                    // Find any intersection points between the circle and the segment;
                    // the intersection MUST be between the segment endpoints
                    //
                    Point inter1 = null;
                    Point inter2 = null;
                    circle.FindIntersection(segment, out inter1, out inter2);
                    if (!segment.PointLiesOnAndBetweenEndpoints(inter1)) inter1 = null;
                    if (!segment.PointLiesOnAndBetweenEndpoints(inter2)) inter2 = null;

                    // Add them to the list (possibly)
                    List<Point> intersectionPts = new List<Point>();
                    if (inter1 != null) intersectionPts.Add(inter1);
                    if (inter2 != null) intersectionPts.Add(inter2);

                    // normalized to drawing point (names)
                    intersectionPts = implied.NormalizePointsToDrawing(intersectionPts);

                    // and add to each figure (circle and polygon).
                    circle.AddIntersectingPoints(intersectionPts);

                    //
                    // Construct the intersections
                    //
                    CircleSegmentIntersection csInter = null;

                    if (intersectionPts.Any())
                    {
                        csInter = new CircleSegmentIntersection(intersectionPts[0], circle, segment);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleSegmentIntersection>(intersections, csInter);
                    }
                    if (intersectionPts.Count > 1)
                    {
                        csInter = new CircleSegmentIntersection(intersectionPts[1], circle, segment);
                        GeometryTutorLib.Utilities.AddStructurallyUnique<CircleSegmentIntersection>(intersections, csInter);
                    }
                }

                // Complete any processing attributed to the circle and all the segments.
                circle.CleanUp();
            }

            return intersections;
        }
    }
}