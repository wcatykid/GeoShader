using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 90 Problem 22
    //
    public class Page90Problem22 : CongruentTrianglesProblem
    {
        public Page90Problem22(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 90 Problem 22";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 2.5, 20); points.Add(c);
            Point d = new Point("D", 2.5, 0); points.Add(d);

            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment cb = new Segment(c, b); segments.Add(cb);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new PerpendicularBisector(parser.GetIntersection(cd, new Segment(a, b)), cd));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, c, d), new Triangle(b, c, d)));
        }
    }
}