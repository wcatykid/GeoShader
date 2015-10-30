using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row2Prob31 : ActualShadedAreaProblem
    {
        public Page6Row2Prob31(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 1.8, 2.4); points.Add(b);
            Point c = new Point("C", 5, 0); points.Add(c);
            Point d = new Point("D", 1.8, -2.4); points.Add(d);
            Point o = new Point("O", 2.5, 0); points.Add(o);


            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2.5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 3);
            known.AddSegmentLength(cd, 4);

            given.Add(new GeometricCongruentSegments(ab, da));
            given.Add(new GeometricCongruentSegments(bc, cd));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0.6, 1.5));
            wanted.Add(new Point("", 0.6, -1.5));
            wanted.Add(new Point("", 3.5, 1.3));
            wanted.Add(new Point("", 3.5, -1.3));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(6.25 * System.Math.PI - 12);

            problemName = "McDougall Page 6 Row 2 Problem 31";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}