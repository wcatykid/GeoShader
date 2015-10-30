using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Col1Prob3 : ActualShadedAreaProblem
    {
        public Page2Col1Prob3(bool onoff, bool complete) : base(onoff, complete)
        {
            Point d = new Point("D", 60, 0); points.Add(d);
            Point b = new Point("B", 0, 60); points.Add(b);
            Point o = new Point("O", 0, 0);  points.Add(o);

            Segment bo = new Segment(b, o); segments.Add(bo);
            Segment od = new Segment(o, d); segments.Add(od);

            circles.Add(new Circle(o, 60.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle(new Angle(b, o, d)));

            known.AddSegmentLength(bo, 60);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(8482.300165);

            problemName = "Page 2 Col 1 Problem 3";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}