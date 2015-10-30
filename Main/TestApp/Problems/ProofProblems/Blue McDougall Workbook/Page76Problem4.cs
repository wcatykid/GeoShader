using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 76 Problem 4
    //
    public class Page76Problem4 : CongruentTrianglesProblem
    {
        public Page76Problem4(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 76 Problem 4";


            Point k = new Point("K", 0, 8); points.Add(k);
            Point i = new Point("I", 3.5, 2.4); points.Add(i);
            Point n = new Point("N", 0, 0); points.Add(n);
            Point g = new Point("G", 2, 0); points.Add(g);
            Point h = new Point("H", 5, 0); points.Add(h);
            Point t = new Point("T", 7, 0); points.Add(t);
            Point m = new Point("M", 7, 8); points.Add(m);

//     System.Diagnostics.Debug.WriteLine(new Segment(k, h).FindIntersection(new Segment(m, g)));

            Segment kn = new Segment(k, n); segments.Add(kn);
            Segment mt = new Segment(m, t); segments.Add(mt);

            List<Point> pts = new List<Point>();
            pts.Add(k);
            pts.Add(i);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(i);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(g);
            pts.Add(h);
            pts.Add(t);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(n, k, h)), (Angle)parser.Get(new Angle(t, m, g))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(n, g)), (Segment)parser.Get(new Segment(h, t))));
            given.Add(new RightAngle(new Angle(k, n, t)));
            given.Add(new RightAngle(new Angle(m, t, n)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(k, h, n)), (Angle)parser.Get(new Angle(m, g, t))));
        }
    }
}