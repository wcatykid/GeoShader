using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 73 Problem 8
    //
    public class Page73Problem8 : CongruentTrianglesProblem
    {
        public Page73Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 73 Problem 8";


            Point t = new Point("T", -4, 5); points.Add(t);
            Point n = new Point("N", 0, 5); points.Add(n);
            Point s = new Point("S", 0, 0); points.Add(s);
            Point h = new Point("H", 0, -5); points.Add(h);
            Point u = new Point("U", 4, -5); points.Add(u);

            Segment nt = new Segment(n, t); segments.Add(nt);
            Segment hu = new Segment(h, u); segments.Add(hu);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(u);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(s);
            pts.Add(h);
            collinear.Add(new Collinear(pts));
            
                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(s, u, h)), (Angle)parser.Get(new Angle(n, t, s))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(n, s)), (Segment)parser.Get(new Segment(s, h))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(s, n, t), new Triangle(s, h, u)));
        }
    }
}