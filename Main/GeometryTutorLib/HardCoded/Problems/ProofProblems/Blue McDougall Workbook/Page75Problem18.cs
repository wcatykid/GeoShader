using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 75 Problem 18
    //
    public class Page75Problem18 : CongruentTrianglesProblem
    {
        public Page75Problem18(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 75 Problem 18";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 2, 4); points.Add(b);
            Point c = new Point("C", 3, 3); points.Add(c);
            Point d = new Point("D", 4, 4); points.Add(d);
            Point e = new Point("E", 6, 0); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ae = new Segment(a, e); segments.Add(ae);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, e)), (Angle)parser.Get(new Angle(a, d, e))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(c, e))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(e, d, c)));
        }
    }
}