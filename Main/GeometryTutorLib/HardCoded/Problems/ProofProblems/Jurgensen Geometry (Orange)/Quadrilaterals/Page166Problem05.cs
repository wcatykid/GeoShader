using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page166Problem05 : QuadrilateralsProblem
    {
        // Geometry; Page 166 Problem 01 to 05
        // Demonstrates usage of:
        // 5. If both pairs of opposite angles of a quad are congruent, then the quad is a parallelogram
        public Page166Problem05(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 166 Problem 05";

            Point a = new Point("A", 6, 2); points.Add(a);
            Point c = new Point("C", 5, -2); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point s = new Point("S", -5, 2); points.Add(s);
            Point k = new Point("K", -6, -2); points.Add(k);

            Segment sa = new Segment(s, a); segments.Add(sa);
            Segment kc = new Segment(k, c); segments.Add(kc);
            Segment sk = new Segment(s, k); segments.Add(sk);
            Segment ac = new Segment(a, c); segments.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(k);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(s);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(s, k, c)), (Angle)parser.Get(new Angle(c, a, s))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(k, c, a)), (Angle)parser.Get(new Angle(a, s, k))));

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(sk, ac, sa, kc));
            goals.Add(new Strengthened(quad, new Parallelogram(quad)));
        }
    }
}