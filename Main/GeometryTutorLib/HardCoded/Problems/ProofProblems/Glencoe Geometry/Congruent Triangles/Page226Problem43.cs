using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 226 problem 43
    //
    public class Page226Problem43 : CongruentTrianglesProblem
    {
        public Page226Problem43(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 226 Problem 43";

            Point a = new Point("A", 0, 4); points.Add(a);
            Point b = new Point("B", 7, 5); points.Add(b);
            Point c = new Point("C", 11, 10); points.Add(c);
            Point d = new Point("D", 3, 0); points.Add(d);
            Point e = new Point("E", 14, 6); points.Add(e);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ce = new Segment(c, e); segments.Add(ce);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel(ad, ce));
            given.Add(new GeometricCongruentSegments(ad, ce));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(e, b, c)));
        }
    }
}