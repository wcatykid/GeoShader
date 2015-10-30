using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page166Problem13 : CongruentTrianglesProblem
    {
        //
        // Geometry; Page 166 Problem 13
        //
        public Page166Problem13(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 166 Problem 13";


            Point a = new Point("A", -6, -3); points.Add(a);
            Point b = new Point("B", 6, -3); points.Add(b);
            Point c = new Point("C", 8, 3); points.Add(c);
            Point d = new Point("D", -4, 3); points.Add(d);
            Point m = new Point("M", 0, -3); points.Add(m);
            Point n = new Point("N", 2, 3); points.Add(n);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment an = new Segment(a, n); segments.Add(an);
            Segment mc = new Segment(m, c); segments.Add(mc);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(n);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            List<Point> pts2 = new List<Point>();
            pts2.Add(a);
            pts2.Add(m);
            pts2.Add(b);
            collinear.Add(new Collinear(pts2));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral givenQuad = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, (Segment)parser.Get(new Segment(d, c)), (Segment)parser.Get(new Segment(a, b))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(n, (Segment)parser.Get(new Segment(d, c))))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(m, (Segment)parser.Get(new Segment(a, b))))));
            given.Add(new Strengthened(givenQuad, new Parallelogram(givenQuad)));

            Quadrilateral targetQuad = (Quadrilateral)parser.Get(new Quadrilateral(an, mc, (Segment)parser.Get(new Segment(n, c)), (Segment)parser.Get(new Segment(a, m))));
            goals.Add(new Strengthened(targetQuad, new Parallelogram(targetQuad)));
        }
    }
}