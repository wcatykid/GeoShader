using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 229 Problem 03
    //
    public class Page229Problem03 : CongruentTrianglesProblem
    {
        public Page229Problem03(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 229 Problem 03";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 12, 0); points.Add(b);
            //Point c = new Point("C", 153.0 / 32.0, 63.0 * 3.8729833462074168851792653997824 / 32.0); points.Add(c);  // square root of 15 = 3.8729833462074168851792653997824
            Point c = new Point("C", 153.0 / 32.0, 63.0 * Math.Sqrt(15.0) / 32.0); points.Add(c);  
            Point x = new Point("X", 18, 0); points.Add(x);
            Point r = new Point("R", 26, 0); points.Add(r);
            Point n = new Point("N", 339.0 / 16.0, 21.0 * Math.Sqrt(15.0) / 16.0); points.Add(n);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment nx = new Segment(n, x); segments.Add(nx);
            Segment nr = new Segment(n, r); segments.Add(nr);
            Segment rx = new Segment(r, x); segments.Add(rx);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
        }
    }
}