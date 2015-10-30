using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 231 problem 14
    //
    public class Page231Problem14 : CongruentTrianglesProblem
    {
        public Page231Problem14(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 231 Problem 14";

            Point j = new Point("J", 0, 0); points.Add(j);
            Point k = new Point("K", 3, 2); points.Add(k);
            Point l = new Point("L", 9, 0); points.Add(l);
            Point m = new Point("M", 5, 0); points.Add(m);
            Point n = new Point("N", 3, -2); points.Add(n);

            Segment jk = new Segment(j, k); segments.Add(jk);
            Segment jn = new Segment(j, n); segments.Add(jn);
            Segment kl = new Segment(k, l); segments.Add(kl);
            Segment km = new Segment(k, m); segments.Add(km);
            Segment ln = new Segment(l, n); segments.Add(ln);
            Segment mn = new Segment(m, n); segments.Add(mn);

            List<Point> pts = new List<Point>();
            pts.Add(j);
            pts.Add(m);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentTriangles(new Triangle(j, k, m), new Triangle(j, n, m)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(j, k, l), new Triangle(j, n, l)));
        }
    }
}