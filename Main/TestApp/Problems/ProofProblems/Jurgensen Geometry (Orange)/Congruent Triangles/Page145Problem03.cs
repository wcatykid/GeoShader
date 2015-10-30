using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 145 Problem 3
    //
    public class Page145Problem03: CongruentTrianglesProblem
    {
        public Page145Problem03(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 145 Problem 3";


            Point g = new Point("G", 0, 10); points.Add(g);
            Point r = new Point("R", 1, 12); points.Add(r);
            Point j = new Point("J", 1, 10); points.Add(j);
            Point a = new Point("A", 3, 10); points.Add(a);
            Point k = new Point("K", 5, 10); points.Add(k);
            Point n = new Point("N", 5, 8); points.Add(n);
            Point t = new Point("T", 6, 10); points.Add(t);

            Segment gr = new Segment(g, r); segments.Add(gr);
            Segment jr = new Segment(j, r); segments.Add(jr);
            Segment kn = new Segment(k, n); segments.Add(kn);
            Segment nt = new Segment(n, t); segments.Add(nt);

            List<Point> pts = new List<Point>();
            pts.Add(g);
            pts.Add(j);
            pts.Add(a);
            pts.Add(k);
            pts.Add(t);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(a);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(g, j)), (Segment)parser.Get(new Segment(k, t))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, r)), (Segment)parser.Get(new Segment(a, n))));
            given.Add(new RightAngle(r, j, a));
            given.Add(new RightAngle(n, k, a));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(j, g, r)), (Angle)parser.Get(new Angle(n, t, k))));
        }
    }
}