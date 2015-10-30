using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 229 Problem 07
    //
    public class Page229Problem07 : CongruentTrianglesProblem
    {
        public Page229Problem07(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 229 Problem 07";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 6, 0); points.Add(b);
            Point c = new Point("C", 6, 8); points.Add(c);
            Point p = new Point("P", 0, 10); points.Add(p);
            Point k = new Point("K", 9, 10); points.Add(k);
            Point n = new Point("N", 9, 12); points.Add(n);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment kp = new Segment(k, p); segments.Add(kp);
            Segment kn = new Segment(k, n); segments.Add(kn);
            Segment pn = new Segment(p, n); segments.Add(pn);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
        }
    }
}