using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MgGeoPracticeProb6 : ActualShadedAreaProblem
    {
        public MgGeoPracticeProb6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double y = 3;
            double x = 3 * System.Math.Sqrt(3);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 2 * x, 0); points.Add(b);
            Point c = new Point("C", x, y); points.Add(c);
            Point d = new Point("D", x, -y); points.Add(d);

            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment cb = new Segment(c, b); segments.Add(cb);
            Segment db = new Segment(d, b); segments.Add(db);

            Circle left = new Circle(a, 6);
            Circle right = new Circle(b, 6);
            circles.Add(left);
            circles.Add(right);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentCircles(left, right));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, a, d)), (Angle)parser.Get(new Angle(c, b, d))));

            known.AddSegmentLength(ac, 6);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(c, a, d)), 60);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 0));
            wanted.Add(new Point("", b.X - 1, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(36 * System.Math.Sqrt(3) - 12 * System.Math.PI);

            problemName = "Magoosh Geometry Practice Problem 6";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}



