using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Col1Prob1 : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public Page2Col1Prob1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point b = new Point("B", 0, 0); points.Add(b);
            Point o = new Point("O", 5, 12); points.Add(o);
            Point a = new Point("A", 10, 24); points.Add(a);
            Point c = new Point("C", 10, 0); points.Add(c);

            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 13));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ac, 24);
            known.AddSegmentLength(bc, 10);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 5, -0.5));
            wanted.Add(new Point("", 11, 12));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(145.4645792);

            problemName = "Page 2 Col 1 Problem 1";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}