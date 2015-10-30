using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.TutorParser
{
    /// <summary>
    /// Live Geometry does not define ALL components of the figure, we must acquire those implied components.
    /// </summary>
    public class DrawingPointCalculator
    {
        private List<GeometryTutorLib.ConcreteAST.Point> knownPoints;
        private List<GeometryTutorLib.ConcreteAST.Segment> segments;
        private List<GeometryTutorLib.ConcreteAST.Circle> circles;

        public DrawingPointCalculator(List<GeometryTutorLib.ConcreteAST.Point> known,
                                      List<GeometryTutorLib.ConcreteAST.Segment> segs,
                                      List<GeometryTutorLib.ConcreteAST.Circle> circs)
        {
            knownPoints = known;
            segments = segs;
            circles = circs;

            unlabeled = new List<Point>();
        }

        // The set of points which was not labeled by the UI; we are populating this list.
        private List<GeometryTutorLib.ConcreteAST.Point> unlabeled;


        public List<GeometryTutorLib.ConcreteAST.Point> GetUnlabeledPoints()
        {
            FindUnlabeledCircleCirclePoints();
            FindUnlabeledSegmentSegmentPoints();
            FindUnlabeledCircleSegmentPoints();

            return unlabeled;
        }

        /// <summary>
        /// Calculate all points of intersection between circles.
        /// </summary>
        private void FindUnlabeledCircleCirclePoints()
        {
            for (int c1 = 0; c1 < circles.Count - 1; c1++)
            {
                for (int c2 = c1 + 1; c2 < circles.Count; c2++)
                {
                    //
                    // Find any intersection points between the circle and the segment;
                    // the intersection MUST be between the segment endpoints
                    //
                    Point inter1 = null;
                    Point inter2 = null;
                    circles[c1].FindIntersection(circles[c2], out inter1, out inter2);

                    // Add them to the list (possibly)
                    HandleIntersectionPoint(knownPoints, unlabeled, inter1);
                    HandleIntersectionPoint(knownPoints, unlabeled, inter2);
                }
            }
        }

        //
        // Find every point of intersection among segments (if they are not labeled in the UI) -- name them.
        //
        private void FindUnlabeledSegmentSegmentPoints()
        {
            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    // If there exists a point of intersection that is between the endpoints of both segments
                    Point inter = segments[s1].FindIntersection(segments[s2]);

                    // Avoid parallel line intersections at infinity
                    if (inter != null && !double.IsInfinity(inter.X) && !double.IsInfinity(inter.Y) && !double.IsNaN(inter.X) && !double.IsNaN(inter.Y))
                    {
                        if (segments[s1].PointLiesOnAndExactlyBetweenEndpoints(inter) && segments[s2].PointLiesOnAndExactlyBetweenEndpoints(inter))
                        {
                            HandleIntersectionPoint(knownPoints, unlabeled, inter);
                        }
                        // This is an extended point (beyond the two segments; the intersection is not apparent in the drawing); not interested.
                        else
                        {
                            // No-Op
                        }
                    }
                }
            }
        }

        //
        // Find every point of intersection among segments (if they are not labeled in the UI) -- name them.
        //
        private void FindUnlabeledCircleSegmentPoints()
        {
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
                foreach (GeometryTutorLib.ConcreteAST.Segment segment in segments)
                {
                    //
                    // Find any intersection points between the circle and the segment;
                    // the intersection MUST be between the segment endpoints
                    //
                    Point inter1 = null;
                    Point inter2 = null;
                    circle.FindIntersection(segment, out inter1, out inter2);

                    // Add them to the list (possibly)
                    inter1 = HandleIntersectionPoint(knownPoints, unlabeled, segment, inter1);
                    inter2 = HandleIntersectionPoint(knownPoints, unlabeled, segment, inter2);
                }
            }
        }

        // Simple function for creating a point (if needed since it is not labeled by the UI).
        private Point HandleIntersectionPoint(List<Point> containment, List<Point> toAdd, GeometryTutorLib.ConcreteAST.Segment segment, Point pt)
        {
            if (pt == null) return null;

            // The point must be between the endpoints of the segment
            if (!segment.PointLiesOnAndBetweenEndpoints(pt)) return null;

            return HandleIntersectionPoint(containment, toAdd, pt);
        }

        // Simple function for creating a point (if needed since it is not labeled by the UI).
        private Point HandleIntersectionPoint(List<Point> containment, List<Point> toAdd, Point pt)
        {
            if (pt == null) return null;

            // If this point was defined by the UI, do nothing
            Point uiPoint = GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Point>(containment, pt);
            if (uiPoint != null) return uiPoint;

            // else create the point.
            return GeometryTutorLib.Utilities.AcquirePoint(containment, pt);
            //Point newPoint = GeometryTutorLib.PointFactory.GeneratePoint(pt.X, pt.Y);
            //GeometryTutorLib.Utilities.AddStructurallyUnique<GeometryTutorLib.ConcreteAST.Point>(toAdd, newPoint);
            //return newPoint;
        }
    }
}