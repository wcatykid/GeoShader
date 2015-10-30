using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page4Prob15 : ActualShadedAreaProblem
    {
        public Page4Prob15(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 8); points.Add(a);
            Point b = new Point("B", 8, 0); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment bo = new Segment(b, o); segments.Add(bo);

            Circle circle = new Circle(o, 8.0);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle aob = (Angle)parser.Get(new Angle(a, o, b));
            given.Add(new Strengthened(aob, new RightAngle(aob)));

            known.AddAngleMeasureDegree(aob, 90);
            known.AddSegmentLength(bo, 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7.5, 2.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(16 * System.Math.PI - 32);

            problemName = "Jurgensen Page 4 Problem 15";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
