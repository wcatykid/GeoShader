using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 144 Problem 1
    //
    public class Page144Problem01 : CongruentTrianglesProblem
    {
        public Page144Problem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Problem 1";


            Point n = new Point("N", 0, 0); points.Add(n);
            Point r = new Point("R", 1, 4); points.Add(r);
            Point x = new Point("X", 3, 0); points.Add(x);
            Point e = new Point("E", 2, 2); points.Add(e);
            
            Point o = new Point("O", 10, 0); points.Add(o);
            Point l = new Point("L", 11, 4); points.Add(l);
            Point y = new Point("Y", 13, 0); points.Add(y);
            Point s = new Point("S", 12, 2); points.Add(s);

            Segment nr = new Segment(n, r); segments.Add(nr);
            Segment nx = new Segment(n, x); segments.Add(nx);
            Segment ne = new Segment(n, e); segments.Add(ne);
            
            Segment lo = new Segment(l, o); segments.Add(lo);
            Segment oy = new Segment(o, y); segments.Add(oy);
            Segment os = new Segment(o, s); segments.Add(os);
            

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(e);
            pts.Add(x);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(s);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, e)), (Segment)parser.Get(new Segment(e, x))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, e)), (Segment)parser.Get(new Segment(s, y))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, e)), (Segment)parser.Get(new Segment(l, s))));
            given.Add(new GeometricCongruentSegments(nr, lo));
            given.Add(new GeometricCongruentSegments(nx, oy));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(n, e)), (Segment)parser.Get(new Segment(o, s))));
        }
    }
}