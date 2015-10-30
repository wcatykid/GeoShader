using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 242 Problem 21
    //
    public class Page242Problem21 : CongruentTrianglesProblem
    {
        public Page242Problem21(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 242 Problem 21";


            Point t = new Point("T", 0, 0); points.Add(t);
            Point s = new Point("S", 6, 8); points.Add(s);
            Point o = new Point("O", 9, 12); points.Add(o);
            Point v = new Point("V", 13, 8); points.Add(v);
            Point w = new Point("W", 21, 0); points.Add(w);

            Segment tw = new Segment(t, w); segments.Add(tw);
            Segment sv = new Segment(s, v); segments.Add(sv);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(o);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(o);
            pts.Add(v);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(tw), (Segment)parser.Get(sv)));
        }
    }
}