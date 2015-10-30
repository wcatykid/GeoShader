using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 286 problem 8
    //
    public class Page286Problem8 : CongruentTrianglesProblem
    {
        public Page286Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 286 Problem 8";


            Point a = new Point("A", 0, 8); points.Add(a);
            Point b = new Point("B", 8, 8); points.Add(b);
            Point c = new Point("C", 4, 4); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);
            Point e = new Point("E", 8, 0); points.Add(e);
            
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment de = new Segment(d, e); segments.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(c);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(c, e))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(c, b))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(c, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(e, d, c)));
        }
    }
}