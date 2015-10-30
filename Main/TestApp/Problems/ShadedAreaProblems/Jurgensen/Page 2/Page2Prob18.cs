using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Prob18 : ActualShadedAreaProblem
    {
        public Page2Prob18(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 4, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);

            Segment ab = new Segment(a, b); segments.Add(ab);

            Circle theCircle = new Circle(o, 4);
            circles.Add(theCircle);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddArcMeasureDegree(new MinorArc(theCircle, a, b), 90);
            known.AddSegmentLength(new Segment(o, b), 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, -1));
            wanted.Add(new Point("", 1, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(45.69911184);

            problemName = "Jurgensen Page 2 Problem 18";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}