using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row5Prob5 : ActualShadedAreaProblem
    {
        public Page6Row5Prob5(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 8.7 * System.Math.Cos(31.5 * System.Math.PI / 180);
            double y = 8.7 * System.Math.Sin(31.5 * System.Math.PI / 180);

            Point a = new Point("A", x, y); points.Add(a);
            Point b = new Point("B", x, -y); points.Add(b);
            Point c = new Point("C", -x, -y); points.Add(c);
            Point d = new Point("D", -x, y); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 8.7));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, o)), 8.7);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(a, o, b)), 63);

            given.Add(new CongruentAngles((Angle)parser.Get(new Angle(a, o, b)), (Angle)parser.Get(new Angle(d, o, c))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 0));
            wanted.Add(new Point("", x + 0.5, 0));
            wanted.Add(new Point("", -1, 0));
            wanted.Add(new Point("", -x - 0.5, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(26.4915 * System.Math.PI);

            problemName = "McDougall Page 6 Row 5 Problem 5";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

