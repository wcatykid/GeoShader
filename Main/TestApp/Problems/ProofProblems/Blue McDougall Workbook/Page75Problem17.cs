using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 75 Problem 17
    //
    public class Page75Problem17 : CongruentTrianglesProblem
    {
        public Page75Problem17(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 75 Problem 17";

            Point v = new Point("V", 3, -7); points.Add(v);
            Point z = new Point("Z", 0, 0); points.Add(z);
            Point y = new Point("Y", 6, 0); points.Add(y);
            Point x = new Point("X", 7, 0); points.Add(x);
            Point u = new Point("U", 10, -7); points.Add(u);
            Point w = new Point("W", 13, 0); points.Add(w);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment vz = new Segment(v, z); segments.Add(vz);
            Segment vy = new Segment(v, y); segments.Add(vy);
            Segment ux = new Segment(u, x); segments.Add(ux);
            Segment uw = new Segment(u, w); segments.Add(uw);

            List<Point> pts = new List<Point>();
            pts.Add(z);
            pts.Add(y);
            pts.Add(x);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(vz, ux));
            given.Add(new GeometricParallel(vy, uw));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(w, x)), (Segment)parser.Get(new Segment(y, z))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(w, x, u), new Triangle(y, z, v)));
        }
    }
}