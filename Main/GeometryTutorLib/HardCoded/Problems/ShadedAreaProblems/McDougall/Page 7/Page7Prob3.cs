using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob3 : ActualShadedAreaProblem
    {
        public Page7Prob3(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 3, 0); points.Add(b);
            Point c = new Point("C", 10, 0); points.Add(c);
            Point d = new Point("D", 20, 0); points.Add(d);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(b, 3));
            circles.Add(new Circle(c, 10));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 10);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, 1));
            wanted.Add(new Point("", 3, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(9 * System.Math.PI);


            problemName = "McDougall Page 7 Problem 3";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}