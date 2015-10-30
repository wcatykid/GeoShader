using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row1Prob27 : ActualShadedAreaProblem
    {
        public Page6Row1Prob27(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 5.2 * System.Math.Cos(54.5 * System.Math.PI / 180);
            double y = 5.2 * System.Math.Sin(54.5 * System.Math.PI / 180);

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

            circles.Add(new Circle(o, 5.2));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(b, o)), 5.2);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(a, o, b)), 109);

            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, o, b)), (Angle)parser.Get(new Angle(d, o, c))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, o, d)), (Angle)parser.Get(new Angle(b, o, c))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 1));
            wanted.Add(new Point("", 0, y + 0.5));
            wanted.Add(new Point("", 0, -1));
            wanted.Add(new Point("", 0, -y - 0.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((3839.68 / 360) * System.Math.PI);

            problemName = "McDougall Page 6 Row 1 Problem 27";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
