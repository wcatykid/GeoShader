using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Prob15 : ActualShadedAreaProblem
    {
        public Page2Prob15(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 0, 4); points.Add(a);
            Point b = new Point("B", -4, 0); points.Add(b);

            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ob = new Segment(o, b); segments.Add(ob);

            Segment ab = new Segment(a, b); segments.Add(ab);

            circles.Add(new Circle(o, 4.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, o, b))));

            known.AddSegmentLength(oa, 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -2.828, 2.8));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(4.566370614);

            problemName = "Jurgensen Page 2 Problem 15";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}