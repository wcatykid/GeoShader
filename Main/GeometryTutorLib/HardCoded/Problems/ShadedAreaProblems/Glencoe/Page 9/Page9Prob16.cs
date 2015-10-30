using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob16 : ActualShadedAreaProblem
    {
        public Page9Prob16(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 1, 0); points.Add(b);
            Point c = new Point("C", 2, 0); points.Add(c);
            Point d = new Point("D", 3, 0); points.Add(d);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(a, 1.0));
            circles.Add(new Circle(a, 2.0));
            circles.Add(new Circle(a, 3.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 1);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 1);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 1);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 0.5));
            wanted.Add(new Point("", 0, -0.5));
            wanted.Add(new Point("", 0, 2.5));
            wanted.Add(new Point("", 0, -2.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(6 * System.Math.PI);

            problemName = "Glencoe Page 9 Problem 16";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}