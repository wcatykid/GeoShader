using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row5Prob6 : ActualShadedAreaProblem
    {
        public Page6Row5Prob6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 0, 3); points.Add(p);
            Point q = new Point("Q", 0, -3); points.Add(q);
            Point a = new Point("A", -3, 0); points.Add(a);
            Point b = new Point("B", 0, 1.5); points.Add(b);
            Point c = new Point("C", 3, 0); points.Add(c);
            Point d = new Point("D", 0, -1.5); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p);
            pts.Add(b);
            pts.Add(o);
            pts.Add(d);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 3));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 6);

            Segment pb = (Segment)parser.Get(new Segment(p, b));
            given.Add(new GeometricCongruentSegments(pb, (Segment)parser.Get(new Segment(b, o))));
            given.Add(new GeometricCongruentSegments(pb, (Segment)parser.Get(new Segment(o, d))));
            given.Add(new GeometricCongruentSegments(pb, (Segment)parser.Get(new Segment(d, q))));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0.5, 0.2));
            unwanted.Add(new Point("", 0.5, -0.2));
            unwanted.Add(new Point("", -0.5, -0.2));
            unwanted.Add(new Point("", -0.5, 0.2));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(9 * System.Math.PI - 9);

            problemName = "McDougall Page 6 Row 5 Problem 6";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}