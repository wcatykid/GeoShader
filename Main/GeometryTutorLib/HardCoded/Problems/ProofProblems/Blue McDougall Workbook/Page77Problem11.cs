using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 77 Problem 11
    //
    public class Page77Problem11 : CongruentTrianglesProblem
    {
        public Page77Problem11(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 77 Problem 11";

            Point x = new Point("X", 0, 8); points.Add(x);
            Point y = new Point("Y", -3, 5); points.Add(y);
            Point w = new Point("W", 3, 5); points.Add(w);
            Point z = new Point("Z", 0, 0); points.Add(z);
            Point o = new Point("O", 0, 5); points.Add(o);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);
            Segment zw = new Segment(z, w); segments.Add(zw);
            Segment xw = new Segment(x, w); segments.Add(xw);

            List<Point> pts = new List<Point>();
            pts.Add(y);
            pts.Add(o);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(o);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new AngleBisector((Angle)parser.Get(new Angle(y, x, w)), (Segment)parser.Get(new Segment(z, x))));
            given.Add(new GeometricCongruentSegments(xy, xw));

            goals.Add(new GeometricCongruentSegments(yz, zw));
        }
    }
}