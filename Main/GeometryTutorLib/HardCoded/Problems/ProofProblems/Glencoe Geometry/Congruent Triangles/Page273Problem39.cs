using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 273 problem 39
    //
    public class Page273Problem39 : CongruentTrianglesProblem
    {
        public Page273Problem39(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 273 Problem 39";

            Point a = new Point("A", 1, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 3, 3); points.Add(c);
            Point d = new Point("D", 5, 6); points.Add(d);
            Point e = new Point("E", 6, 1); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment de = new Segment(d, e); segments.Add(de);

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
            
            given.Add(new SegmentBisector(parser.GetIntersection(new Segment(a, d), new Segment(b, e)), (Segment)parser.Get(new Segment(a, d))));
            given.Add(new GeometricParallel(ab, de));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c)));
        }
    }
}