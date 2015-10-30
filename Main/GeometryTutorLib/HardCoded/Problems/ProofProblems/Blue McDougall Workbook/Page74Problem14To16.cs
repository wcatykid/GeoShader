using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 74 Problem 14-16
    //
    public class Page74Problem14To16 : CongruentTrianglesProblem
    {
        public Page74Problem14To16(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 74 Problem 14-16";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 5, 6); points.Add(b);
            Point c = new Point("C", 10, 0); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);
            Point e = new Point("E", 5, 0); points.Add(e);
            Point f = new Point("F", 4, 0); points.Add(f);

            Segment ba = new Segment(b, a); segments.Add(ba);
            Segment bf = new Segment(b, f); segments.Add(bf);
            Segment be = new Segment(b, e); segments.Add(be);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(e);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(bf, bd));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, f)), (Segment)parser.Get(new Segment(d, c))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(f, e)), (Segment)parser.Get(new Segment(e, d))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(f, a, b)), (Angle)parser.Get(new Angle(a, c, b))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(e, f, b)), (Angle)parser.Get(new Angle(e, d, b))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, f)), (Angle)parser.Get(new Angle(c, b, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(b, e, f), new Triangle(b, e, d)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, d, b), new Triangle(c, f, b)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, f, b), new Triangle(c, d, b)));
        }
    }
}