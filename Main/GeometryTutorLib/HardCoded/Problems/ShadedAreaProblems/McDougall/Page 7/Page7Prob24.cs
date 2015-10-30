using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob24 : ActualShadedAreaProblem
    {
        public Page7Prob24(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 6 * System.Math.Cos(77.5 * System.Math.PI / 180);
            double y = 6 * System.Math.Sin(77.5 * System.Math.PI / 180);

            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -x, y); points.Add(a);
            Point b = new Point("B", x, y); points.Add(b);
            Point p = new Point("P", -6, 0); points.Add(p);
            Point q = new Point("Q", 6, 0); points.Add(q);

            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ob = new Segment(o, b); segments.Add(ob);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(o);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 6));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(p, q)), 12);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(a, o, b)), 25);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 1));
            wanted.Add(new Point("", 0, y + 0.1));
            wanted.Add(new Point("", -0.9, 4.1));
            wanted.Add(new Point("", 0, 5));
            wanted.Add(new Point("", 0.9, 4.1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(2.5 * System.Math.PI);


            problemName = "McDougall Page 7 Problem 24";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}