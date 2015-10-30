using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 73 Problem 9
    //
    public class Page73Problem9 : CongruentTrianglesProblem
    {
        public Page73Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 73 Problem 9";


            Point g = new Point("G", 5, 5); points.Add(g);
            Point h = new Point("H", 5, 3); points.Add(h);
            Point k = new Point("K", 0, 0); points.Add(k);
            Point n = new Point("N", 5, 0); points.Add(n);
            Point l = new Point("L", 8, 0); points.Add(l);

//     System.Diagnostics.Debug.WriteLine(new Segment(k, h).FindIntersection(new Segment(m, g)));

            Segment kh = new Segment(k, h); segments.Add(kh);
            Segment lg = new Segment(l, g); segments.Add(lg);

            List<Point> pts = new List<Point>();
            pts.Add(g);
            pts.Add(h);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(n);
            pts.Add(l);
            collinear.Add(new Collinear(pts));
            
                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(n, h, k)), (Angle)parser.Get(new Angle(n, l, g))));
            given.Add(new GeometricCongruentSegments(lg, kh));
            given.Add(new RightAngle(new Angle(g, n, k)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(k, n, h), new Triangle(g, n, l)));
        }
    }
}