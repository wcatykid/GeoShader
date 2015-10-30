using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page174Theorem416_1 : QuadrilateralsProblem
    {
        // Simple demonstration of theorem "Median of a trapezoid is parallel to the bases"
        public Page174Theorem416_1(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 174 Theorem 4-16 Part 1";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 11, 0); points.Add(b);
            Point c = new Point("C", 7, 4); points.Add(c);
            Point d = new Point("D", 2, 4); points.Add(d);
            Point m = new Point("M", 1, 2); points.Add(m);
            Point n = new Point("N", 9, 2); points.Add(n);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment mn = new Segment(m, n); segments.Add(mn);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(n);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ad = (Segment)parser.Get(new Segment(a, d));
            Segment bc = (Segment)parser.Get(new Segment(b, c));
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, cd, ab));
            //given.Add(new Strengthened(quad, new Trapezoid(ad, bc, cd, ab)));
            given.Add(new Parallel(ab, cd));
            given.Add(new SegmentBisector(parser.GetIntersection(mn, ad), mn));
            given.Add(new SegmentBisector(parser.GetIntersection(mn, bc), mn));

            goals.Add(new Parallel(mn, cd));
            goals.Add(new Parallel(mn, ab));
        }
    }
}
