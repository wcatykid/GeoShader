using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Prob28 : ActualShadedAreaProblem
    {
        public Page2Prob28(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 3, 6 * System.Math.Sqrt(3.0) / 2.0); points.Add(a);
            Point b = new Point("B", 9, 2 * System.Math.Sqrt(3.0) / 2.0); points.Add(b);
            Point x = new Point("X", 0, 0); points.Add(x);
            Point y = new Point("Y", 8, 0); points.Add(y);

            Segment ax = new Segment(a, x); segments.Add(ax);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment by = new Segment(b, y); segments.Add(by);
            Segment xy = new Segment(x, y); segments.Add(xy);

            Circle circleX = new Circle(x, 6);
            Circle circleY = new Circle(y, 2);
            circles.Add(circleX);
            circles.Add(circleY);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            CircleIntersection inter = (CircleCircleIntersection)parser.Get(new CircleCircleIntersection(new Point("", 6, 0), circleX, circleY));
            given.Add(new Tangent(inter));

            inter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(a, circleX, ab));
            given.Add(new Tangent(inter));

            inter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(b, circleY, ab));
            given.Add(new Tangent(inter));

            known.AddSegmentLength(ax, 6);
            known.AddSegmentLength(by, 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 6, 2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(4.674466795);
        }
    }
}