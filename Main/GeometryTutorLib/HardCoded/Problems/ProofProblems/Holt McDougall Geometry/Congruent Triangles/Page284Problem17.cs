using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 284 problem 17
    //
    public class Page284Problem17 : CongruentTrianglesProblem
    {
        public Page284Problem17(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 284 Problem 17";


            Point q = new Point("Q", 0, 6); points.Add(q);
            Point r = new Point("R", 0, 0); points.Add(r);
            Point s = new Point("S", 4, 3); points.Add(s);
            Point t = new Point("T", 8, 6); points.Add(t);
            Point u = new Point("U", 8, 0); points.Add(u);

            Segment qr = new Segment(q, r); segments.Add(qr);
            Segment tu = new Segment(t, u); segments.Add(tu);

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(s);
            pts.Add(t);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(s);
            pts.Add(u);
            collinear.Add(new Collinear(pts));
           
            // There are more congruences implied.
                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(new Segment(q, s), new Segment(s, u)));
            given.Add(new GeometricCongruentSegments(new Segment(r, s), new Segment(s, t)));
            given.Add(new GeometricCongruentSegments(new Segment(r, s), new Segment(s, u)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(q, r, s), new Triangle(t, u, s)));
        }
    }
}