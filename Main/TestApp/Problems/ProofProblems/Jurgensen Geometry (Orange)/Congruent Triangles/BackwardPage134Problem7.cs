using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 134 #7
    //
    public class BackwardPage134Problem7 : CongruentTrianglesProblem
    {
        public BackwardPage134Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Backward Page 134 Problem 7";

            Point s = new Point("S", 1, 4); points.Add(s);
            Point t = new Point("T", 5, 4); points.Add(t);
            Point o = new Point("O", 3, 2.4); points.Add(o);
            Point r = new Point("R", 0, 0); points.Add(r);
            Point a = new Point("A", 6, 0); points.Add(a);

            Segment st = new Segment(s, t); segments.Add(st);
            Segment sr = new Segment(s, r); segments.Add(sr);
            Segment ta = new Segment(t, a); segments.Add(ta);
            Segment ra = new Segment(r, a); segments.Add(ra);

            List<Point> pts = new List<Point>();
            pts.Add(s);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(o);
            pts.Add(t);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(s, t)), (Segment)parser.Get(new Segment(r, a))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(o, a)), (Segment)parser.Get(new Segment(o, r))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, s)),
                                                     (Segment)parser.Get(new Segment(a, t))));
            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, t)),
                                                     (Segment)parser.Get(new Segment(a, s))));
        }
    }
}