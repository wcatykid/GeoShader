using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 316 problem 42
    //
    public class Page316Problem42 : CongruentTrianglesProblem
    {
        public Page316Problem42(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 316 Problem 42";


            Point p = new Point("P", 10, 0); points.Add(p);
            Point q = new Point("Q", 0, 0); points.Add(q);
            Point l = new Point("L", 0, 6); points.Add(l);
            Point m = new Point("M", 10, 6); points.Add(m);
            Point n = new Point("N", 3, 3); points.Add(n);

            Segment lm = new Segment(l, m); segments.Add(lm);
            Segment ln = new Segment(l, n); segments.Add(ln);
            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment np = new Segment(n, p); segments.Add(np);
            Segment nq = new Segment(n, q); segments.Add(nq);
            Segment pq = new Segment(p, q); segments.Add(pq);
            
            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(m, l, n)), (Angle)parser.Get(new Angle(n, q, p))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(n, m, l)), (Angle)parser.Get(new Angle(n, p, q))));
            given.Add(new GeometricCongruentSegments(ln, nq));

            goals.Add(new GeometricCongruentTriangles(new Triangle(l, m, n), new Triangle(q, p, n)));
        }
    }
}