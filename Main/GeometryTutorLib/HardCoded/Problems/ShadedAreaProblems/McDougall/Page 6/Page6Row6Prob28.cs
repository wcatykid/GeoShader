using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row6Prob28 : ActualShadedAreaProblem
    {
        public Page6Row6Prob28(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 4 * System.Math.Sqrt(3);
            double y = 4;

            Point a = new Point("A", -x, -y); points.Add(a);
            Point b = new Point("B", 0, 8); points.Add(b);
            Point c = new Point("C", x, -y); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);
            Segment oc = new Segment(o, c); segments.Add(oc);

            circles.Add(new Circle(o, 8.7));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(oc, 8);

            given.Add(new GeometricCongruentSegments(ab, bc));
            given.Add(new GeometricCongruentSegments(ab, ca));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", x, 0));
            wanted.Add(new Point("", -x, 0));
            wanted.Add(new Point("", 0, -y-0.2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(26.4915 * System.Math.PI);

            problemName = "McDougall Page 6 Row 6 Problem 28";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}