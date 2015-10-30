using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob18 : ActualShadedAreaProblem
    {
        public Page9Prob18(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double r = 16 / System.Math.Sqrt(3);
            double x = r / 2;
            double y = 8;

            Point a = new Point("A", x, y); points.Add(a);
            Point b = new Point("B", x, -y); points.Add(b);
            Point c = new Point("C", -r, 0); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);
            Segment co = new Segment(c, o); segments.Add(co);
            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment bo = new Segment(b, o); segments.Add(bo);

            circles.Add(new Circle(o, r));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 16);

            given.Add(new GeometricCongruentSegments(ab, bc));
            given.Add(new GeometricCongruentSegments(ab, ca));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", x+0.5, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(52.41044047);

            problemName = "Glencoe Page 9 Problem 18";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}