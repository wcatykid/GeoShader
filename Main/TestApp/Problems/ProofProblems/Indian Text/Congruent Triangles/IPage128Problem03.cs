using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Book I: Page 128 Problem 3
    //
    public class IPage128Problem03 : CongruentTrianglesProblem
    {
        public IPage128Problem03(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 128 Problem 3";


            Point a = new Point("A", 2, 3); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 4, -1); points.Add(c);
            Point m = new Point("M", 2, -0.5); points.Add(m);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment am = new Segment(a, m); segments.Add(am);
            Segment ac = new Segment(a, c); segments.Add(ac);

            Point p = new Point("P", 12, 5); points.Add(p);
            Point q = new Point("Q", 10, 2); points.Add(q);
            Point n = new Point("N", 12, 1.5); points.Add(n);
            Point r = new Point("R", 14, 1); points.Add(r);

            Segment pq = new Segment(p, q); segments.Add(pq);
            Segment pn = new Segment(p, n); segments.Add(pn);
            Segment pr = new Segment(p, r); segments.Add(pr);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(n);
            pts.Add(r);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Median(am, (Triangle)parser.Get(new Triangle(a, b, c))));
            given.Add(new Median(pn, (Triangle)parser.Get(new Triangle(p, q, r))));
            given.Add(new GeometricCongruentSegments(ab, pq));
            given.Add(new GeometricCongruentSegments(am, pn));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, m)), (Segment)parser.Get(new Segment(q, n))));

            goals.Add(new GeometricCongruentTriangles((Triangle)parser.Get(new Triangle(a, b, m)), (Triangle)parser.Get(new Triangle(p, q, n))));
            goals.Add(new GeometricCongruentTriangles((Triangle)parser.Get(new Triangle(a, b, c)), (Triangle)parser.Get(new Triangle(p, q, r))));

        }
    }
}