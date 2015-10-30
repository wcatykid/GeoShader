using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row5Prob4 : ActualShadedAreaProblem
    {
        public Page6Row5Prob4(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 0, 11); points.Add(a);
            Point b = new Point("B", 0, -33); points.Add(b);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(o, 11));
            circles.Add(new Circle(o, 33));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, o)), 11);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, b)), 33);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 20, 1));
            wanted.Add(new Point("", -20, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(968 * System.Math.PI);

            problemName = "McDougall Page 6 Row 5 Problem 4";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}