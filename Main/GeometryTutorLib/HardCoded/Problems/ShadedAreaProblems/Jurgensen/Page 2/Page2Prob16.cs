using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Prob16 : ActualShadedAreaProblem
    {
        public Page2Prob16(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 1.5, 3 * System.Math.Sin(System.Math.PI / 3)); points.Add(a);
            Point b = new Point("B", 3, 0); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ob = new Segment(o, b); segments.Add(ob);
            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ab = new Segment(a, b); segments.Add(ab);

            circles.Add(new Circle(o, 3.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(a, o, b)), 60);

            known.AddSegmentLength(ob, 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2.8, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((1.5 * System.Math.PI) - ((9 * System.Math.Sqrt(3)) / 4));

            problemName = "Jurgensen Page 2 Problem 16";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}