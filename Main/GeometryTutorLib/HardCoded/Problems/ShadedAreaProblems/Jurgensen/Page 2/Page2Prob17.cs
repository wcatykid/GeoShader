using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Prob17 : ActualShadedAreaProblem
    {
        public Page2Prob17(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 2.5, 0); points.Add(a);
            Point b = new Point("B", -2.5, 0); points.Add(b);
            Point p = new Point("P", -0.7, 2.4); points.Add(p);

            Segment pa = new Segment(p, a); segments.Add(pa);
            Segment pb = new Segment(p, b); segments.Add(pb);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2.5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, p, a))));

            known.AddSegmentLength(pb, 3);
            known.AddSegmentLength(pa, 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(9.817477042);

            problemName = "Jurgensen Page 2 Problem 17";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}