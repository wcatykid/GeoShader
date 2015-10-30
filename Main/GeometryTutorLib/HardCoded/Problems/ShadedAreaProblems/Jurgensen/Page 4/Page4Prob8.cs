using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page4Prob8 : ActualShadedAreaProblem
    {
        public Page4Prob8(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -1 * System.Math.Sqrt(27), -3); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", System.Math.Sqrt(27), -3); points.Add(c);
            Point p = new Point("P", 0, 0); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);

            Segment ap = new Segment(a, p); segments.Add(ap);
            Segment bp = new Segment(b, p); segments.Add(bp);
            Segment cp = new Segment(c, p); segments.Add(cp);

            circles.Add(new Circle(p, 6.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Triangle tri = (Triangle)parser.Get(new Triangle(a, b, c));
            given.Add(new Strengthened(tri, new EquilateralTriangle(tri)));

            known.AddSegmentLength(ap, 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, -4));
            wanted.Add(new Point("", -5, 0));
            wanted.Add(new Point("", 5, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(36 * System.Math.PI - 27 * System.Math.Sqrt(3));

            problemName = "Jurgensen Page 4 Problem 8";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}