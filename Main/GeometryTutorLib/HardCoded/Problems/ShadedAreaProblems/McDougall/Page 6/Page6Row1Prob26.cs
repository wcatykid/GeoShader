using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row1Prob26 : ActualShadedAreaProblem
    {
        public Page6Row1Prob26(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double r = 5 / System.Math.Sqrt(2);
            Point a = new Point("A", -r, 0); points.Add(a);
            Point b = new Point("B", 0, r); points.Add(b);
            Point c = new Point("C", r, 0); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bo = new Segment(b, o); segments.Add(bo);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, r));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(bc, 5);
            Angle angle = (Angle)parser.Get(new Angle(b, o, a));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -2, 2));
            wanted.Add(new Point("", 2, 2));
            wanted.Add(new Point("", 0, -2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(12.5 * System.Math.PI - 12.5);

            problemName = "McDougall Page 6 Row 1 Problem 26";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}