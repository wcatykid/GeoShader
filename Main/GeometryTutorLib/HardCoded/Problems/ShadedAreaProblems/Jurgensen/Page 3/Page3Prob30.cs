using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page3Prob30 : ActualShadedAreaProblem
    {
        public Page3Prob30(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -10, 0); points.Add(a);
            Point b = new Point("B", 0, 10); points.Add(b);
            Point c = new Point("C", 10, 0); points.Add(c);
            Point x = new Point("X", -5, 5); points.Add(x);
            Point y = new Point("Y", 5, 5); points.Add(y);

            Segment ob = new Segment(o, b); segments.Add(ob);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(y);
            pts.Add(c);
            collinear.Add(new Collinear(pts));


            circles.Add(new Circle(o, 10));
            circles.Add(new Circle(x, System.Math.Sqrt(50)));
            circles.Add(new Circle(y, System.Math.Sqrt(50)));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 20);

            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(b, c))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -11, 3));
            wanted.Add(new Point("", 11, 3));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(100);

            problemName = "Jurgensen Page 3 Problem 30";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
