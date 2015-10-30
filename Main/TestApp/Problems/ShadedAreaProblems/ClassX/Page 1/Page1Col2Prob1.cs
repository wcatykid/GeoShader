using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page1Col2Prob1 : ActualShadedAreaProblem
    {
        public Page1Col2Prob1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, -35); points.Add(a);
            Point b = new Point("B", 35, 0); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 0, 35); points.Add(p);

            Segment ob = new Segment(o, b); segments.Add(ob);
            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ab = new Segment(a, b); segments.Add(ab);

            circles.Add(new Circle(o, 35.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, o, b))));

            known.AddSegmentLength(ob, 35);

            goalRegions.AddRange(parser.implied.GetAllAtomicRegionsWithoutPoint(new Point("", 24.7, -24.7)));

            SetSolutionArea(3498.83825);

            problemName = "Page 1 Col 2 Problem 1";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}