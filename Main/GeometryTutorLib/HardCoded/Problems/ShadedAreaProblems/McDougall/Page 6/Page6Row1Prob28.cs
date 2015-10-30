using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row1Prob28 : ActualShadedAreaProblem
    {
        public Page6Row1Prob28(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 20); points.Add(b);
            Point c = new Point("C", 20, 20); points.Add(c);
            Point d = new Point("D", 20, 0); points.Add(d);
            Point o = new Point("O", 5, 5); points.Add(o);
            Point p = new Point("P", 5, 15); points.Add(p);
            Point q = new Point("Q", 15, 15); points.Add(q);
            Point r = new Point("R", 15, 5); points.Add(r);
            Point s = new Point("S", 10, 5); points.Add(s);
            Point t = new Point("T", 10, 15); points.Add(t);
            Point u = new Point("U", 20, 15); points.Add(u);
            Point v = new Point("V", 20, 5); points.Add(v);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment pt = new Segment(p, t); segments.Add(pt);
            Segment os = new Segment(o, s); segments.Add(os);
            Segment qu = new Segment(q, u); segments.Add(qu);
            Segment rv = new Segment(r, v); segments.Add(rv);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(u);
            pts.Add(v);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, bc, cd, da));
            given.Add(new Strengthened(quad, new Square(quad)));

            given.Add(new GeometricCongruentSegments(pt, os));
            given.Add(new GeometricCongruentSegments(pt, qu));
            given.Add(new GeometricCongruentSegments(pt, rv));

            known.AddSegmentLength(bc, 20);
            known.AddSegmentLength(ab, 20);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 10, 10));
            wanted.Add(new Point("", 2, 10));
            wanted.Add(new Point("", 18, 10));
            wanted.Add(new Point("", 0.5, 0.5));
            wanted.Add(new Point("", 10, 0.5));
            wanted.Add(new Point("", 19, 0.5));
            wanted.Add(new Point("", 0.5, 19));
            wanted.Add(new Point("", 10, 19));
            wanted.Add(new Point("", 19.5, 19));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);


            SetSolutionArea(400 - 100 * System.Math.PI);
        }
    }
}