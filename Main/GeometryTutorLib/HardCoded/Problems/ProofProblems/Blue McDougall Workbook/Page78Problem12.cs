using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 78 Problem 12
    //
    public class Page78Problem12 : CongruentTrianglesProblem
    {
        public Page78Problem12(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 78 Problem 12";


            Point m = new Point("M", 2, 5); points.Add(m);
            Point n = new Point("N", 0, 0); points.Add(n);
            Point q = new Point("Q", 12, 5); points.Add(q);
            Point t = new Point("T", 10, 0); points.Add(t);

            Segment mq = new Segment(m, q); segments.Add(mq);
            Segment mt = new Segment(m, t); segments.Add(mt);
            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment nt = new Segment(n, t); segments.Add(nt);
            Segment tq = new Segment(t, q); segments.Add(tq);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(mq, nt));
            given.Add(new GeometricCongruentSegments(mq, nt));

            goals.Add(new GeometricCongruentSegments(mn, tq));
        }
    }
}