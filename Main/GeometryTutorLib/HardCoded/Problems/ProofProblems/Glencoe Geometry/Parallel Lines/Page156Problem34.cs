using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 156 problem 34
    //
    public class Page156Problem34 : ParallelLinesProblem
    {
        public Page156Problem34(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 156 Problem 34";

            Point s = new Point("S", 0, 10); points.Add(s);
            Point t = new Point("T", 0, 0); points.Add(t);
            Point u = new Point("U", 10, 0); points.Add(u);
            Point v = new Point("V", 10, 10); points.Add(v);
            Point w = new Point("W", 5, 5); points.Add(w);

            Segment st = new Segment(s, t); segments.Add(st);
            Segment uv = new Segment(u, v); segments.Add(uv);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(w);
            pts.Add(v);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(s);
            pts.Add(w);
            pts.Add(u);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(w, v, u)), (Angle)parser.Get(new Angle(w, s, t))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(w, t, s)), (Angle)parser.Get(new Angle(w, s, t))));

            goals.Add(new GeometricParallel(st, uv));
        }
    }
}