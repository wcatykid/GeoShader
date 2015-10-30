using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row6Prob29 : ActualShadedAreaProblem
    {
        public Page6Row6Prob29(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 2 * System.Math.Sqrt(3);
            double y = 2;

            Point a = new Point("A", x, y); points.Add(a);
            Point b = new Point("B", x, -y); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", x, 0); points.Add(p);

            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment bo = new Segment(b, o); segments.Add(bo);
            Segment op = new Segment(p, o); segments.Add(op);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(p);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 4);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Arc arc = (MinorArc)parser.Get(new MinorArc(circle, a, b));
            Angle angle = (Angle)parser.Get(new Angle(o, p, b));

            known.AddSegmentLength(op, x);
            known.AddArcMeasureDegree(arc, 60);
            known.AddAngleMeasureDegree(angle, 90);

            given.Add(new GeometricArcEquation(arc, new NumericValue(60)));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", x+0.2, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((8.0/3.0) * System.Math.PI - (4 * System.Math.Sqrt(3)));

            problemName = "McDougall Page 6 Row 6 Problem 29";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
