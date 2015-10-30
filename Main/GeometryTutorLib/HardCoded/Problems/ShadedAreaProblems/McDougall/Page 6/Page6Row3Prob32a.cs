using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row3Prob32a : ActualShadedAreaProblem
    {
        public Page6Row3Prob32a(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 8 * System.Math.Cos(36 * System.Math.PI / 180);
            double y = 8 * System.Math.Sin(36 * System.Math.PI / 180);
            Point r = new Point("R", -x, y); points.Add(r);
            Point p = new Point("P", 0, 0); points.Add(p);
            Point s = new Point("S", x, y); points.Add(s);
            Point q = new Point("Q", 0, -4); points.Add(q);

            Segment rp = new Segment(r, p); segments.Add(rp);
            Segment ps = new Segment(p, s); segments.Add(ps);
            Segment pq = new Segment(p, q); segments.Add(pq);

            Circle outer = new Circle(p, 8);
            Circle inner = new Circle(q, 4);
            circles.Add(outer);
            circles.Add(inner);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            MinorArc m = (MinorArc)parser.Get(new MinorArc(outer, r, s));
            known.AddSegmentLength(pq, 4);
            known.AddArcMeasureDegree(m, 108);

            given.Add(new GeometricArcEquation(m, new NumericValue(108)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 1));
            wanted.Add(new Point("", 0, y + 0.2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(19.2 * System.Math.PI);

            problemName = "McDougall Page 6 Row 3 Problem 32a";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}