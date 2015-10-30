using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page4prob17 : ActualShadedAreaProblem
    {
        public Page4prob17(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -System.Math.Sqrt(27), 3); points.Add(a);
            Point b = new Point("B", System.Math.Sqrt(27), 3); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 0, -6); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment op = new Segment(o, p); segments.Add(op);
            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment bo = new Segment(b, o); segments.Add(bo);

            Circle circle = new Circle(o, 6.0);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(op, 6);
            given.Add(new GeometricArcEquation((MinorArc)parser.Get(new MinorArc(circle, a, b)), new NumericValue(120)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 5, 3.3));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(24 * System.Math.PI + (9 * System.Math.Sqrt(3)));

            problemName = "Jurgensen Page 4 Problem 17";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}