using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Prob18 : ActualShadedAreaProblem
    {
        public Page2Prob18(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 0, 4); points.Add(a);
            Point b = new Point("B", 4, 0); points.Add(b);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment bo = new Segment(b, o); segments.Add(bo);

            Circle theCircle = new Circle(o, 4);
            circles.Add(theCircle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            MinorArc arcAB = (MinorArc)parser.Get(new MinorArc(theCircle, a, b));
            given.Add(new GeometricArcEquation(arcAB, new NumericValue(90.0)));
            known.AddArcMeasureDegree(arcAB, 90.0);
            known.AddSegmentLength(ao, 4.0);
            known.AddSegmentLength(bo, 4.0);

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