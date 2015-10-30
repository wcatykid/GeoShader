using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page166Problem01 : QuadrilateralsProblem
    {
        // Geometry; Page 166 Problem 01 to 05
        // Demonstrates usage of:
        // 1. Definition of parallelogram
        public Page166Problem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 166 Problem 01";

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

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            //problem 01 - demonstrates (1)
            given.Add(new GeometricParallel(sa, kc));
            given.Add(new GeometricParallel(sk, ac));

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(sk, ac, sa, kc));
            goals.Add(new Strengthened(quad, new Parallelogram(quad)));
        }
    }
}