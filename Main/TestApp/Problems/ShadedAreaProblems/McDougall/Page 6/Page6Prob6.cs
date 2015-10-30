using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page6Prob6 : ActualShadedAreaProblem
    {
        public Page6Prob6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 0, 6); points.Add(p);
            Point q = new Point("Q", 0, -6); points.Add(q);
            Point a = new Point("A", -6, 0); points.Add(a);
            Point b = new Point("B", 0, 3/2); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 0, -3/2); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment pb = new Segment(p, b); segments.Add(pb);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts.Add(p);
            pts.Add(b);
            pts.Add(o);
            pts.Add(d);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 3));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 6);
            given.Add(new GeometricCongruentSegments(pb, (Segment)parser.Get(new Segment(b, o))));
            given.Add(new GeometricCongruentSegments(pb, (Segment)parser.Get(new Segment(o, d))));
            given.Add(new GeometricCongruentSegments(pb, (Segment)parser.Get(new Segment(d, q))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 0.5));
            wanted.Add(new Point("", 2, -0.5));
            wanted.Add(new Point("", -2, -0.5));
            wanted.Add(new Point("", -2, 0.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(9 * System.Math.PI - 9);
        }
    }
}