using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 23 Problem 9
    //
    public class Page23Problem9 : CongruentTrianglesProblem
    {
        public Page23Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 23 Problem 9";


            Point u = new Point("U", 0, 0); points.Add(u);
            Point v = new Point("V", 2, 4); points.Add(v);
            Point w = new Point("W", 4, 0); points.Add(w);
            Point x = new Point("X", 7, 0); points.Add(x);
            Point y = new Point("Y", 9, 4); points.Add(y);
            Point z = new Point("Z", 11, 0); points.Add(z);

            Segment uv = new Segment(u, v); segments.Add(uv);
            Segment vw = new Segment(v, w); segments.Add(vw);
            Segment yx = new Segment(y, x); segments.Add(yx);
            Segment yz = new Segment(y, z); segments.Add(yz);

            List<Point> pts = new List<Point>();
            pts.Add(u);
            pts.Add(w);
            pts.Add(x);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff); 

            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(v, u, w)), (Angle)parser.Get(new Angle(u, w, v))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(z, x, y)), (Angle)parser.Get(new Angle(u, w, v))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(z, x, y)), (Angle)parser.Get(new Angle(x, z, y))));

            given.Add(new GeometricCongruentSegments(uv, vw));
            given.Add(new GeometricCongruentSegments(yx, vw));
            given.Add(new GeometricCongruentSegments(yx, yz));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(u, x)), (Segment)parser.Get(new Segment(w, z))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(u, v, w), new Triangle(x, y, z)));
        }
    }
}