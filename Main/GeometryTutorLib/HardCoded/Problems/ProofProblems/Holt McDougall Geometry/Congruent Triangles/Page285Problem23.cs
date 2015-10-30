using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 285 problem 23
    //
    public class Page285Problem23 : CongruentTrianglesProblem
    {
        public Page285Problem23(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 285 Problem 23";


            Point q = new Point("Q", 0, 5); points.Add(q);
            Point t = new Point("T", 2.4, 1.8); points.Add(t);
            Point v = new Point("V", 0, 0); points.Add(v);
            Point s = new Point("S", -2.4, 1.8); points.Add(s);
            Point p = new Point("P", -4, 3); points.Add(p);
            Point u = new Point("U", -3.75, 0); points.Add(u);
            Point r = new Point("R", 4, 3); points.Add(r);
            Point w = new Point("W", 3.75, 0); points.Add(w);

            Segment pu = new Segment(p, u); segments.Add(pu);
            Segment rw = new Segment(r, w); segments.Add(rw);
            Segment qv = new Segment(q, v); segments.Add(qv);

            List<Point> pts = new List<Point>();
            pts.Add(u);
            pts.Add(s);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p);
            pts.Add(s);
            pts.Add(v);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(t);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(v);
            pts.Add(t);
            pts.Add(r);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(s, v)), (Segment)parser.Get(new Segment(t, v))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(s, q)), (Segment)parser.Get(new Segment(t, q))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(p, s, u)), (Angle)parser.Get(new Angle(r, t, w))));
        }
    }
}