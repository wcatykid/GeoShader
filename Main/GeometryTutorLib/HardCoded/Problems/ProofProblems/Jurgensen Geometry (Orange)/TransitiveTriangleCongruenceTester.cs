using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TranstiveTriangleCongruenceTester : TransversalsProblem
    {
        public TranstiveTriangleCongruenceTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4);  points.Add(b);
            Point c = new Point("C", 2, 0);  points.Add(c);

            Point d = new Point("D", 5, 0); points.Add(d);
            Point e = new Point("E", 5, 4); points.Add(e);
            Point f = new Point("F", 7, 0); points.Add(f);

            Point m = new Point("M", 12, 0); points.Add(m);
            Point n = new Point("N", 12, 4); points.Add(n);
            Point p = new Point("P", 14, 0); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ac = new Segment(a, c); segments.Add(ac);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment df = new Segment(d, f); segments.Add(df);

            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment np = new Segment(n, p); segments.Add(np);
            Segment mp = new Segment(m, p); segments.Add(mp);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, f)));
            given.Add(new GeometricCongruentTriangles(new Triangle(f, d, e), new Triangle(p, m, n)));

            goals.Add(new AlgebraicCongruentTriangles(new Triangle(a, b, c), new Triangle(m, n, p)));
        }
    }
}