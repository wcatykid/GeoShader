using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 229 Problem 05
    //
    public class Page229Problem05 : CongruentTrianglesProblem
    {
        public Page229Problem05(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 229 Problem 05";


            Point a = new Point("A", 13.0 / 2.0, 3.0); points.Add(a);
            Point b = new Point("B", 0, 3); points.Add(b);
            Point c = new Point("C", 2, 0); points.Add(c);
            Point e = new Point("E", 13.0 / 3.0, 3); points.Add(e);
            Point f = new Point("F", 5, 2); points.Add(f);

            Segment bc = new Segment(c, b); segments.Add(bc);
            Segment ef = new Segment(e, f); segments.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(e);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
        }
    }
}