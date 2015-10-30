using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 316 problem 44
    //
    public class Page316Problem44 : CongruentTrianglesProblem
    {
        public Page316Problem44(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 316 Problem 44";


            Point v = new Point("V", -3, 0); points.Add(v);
            Point w = new Point("W", 0, 3); points.Add(w);
            Point x = new Point("X", 100, 0); points.Add(x);
            Point y = new Point("Y", 0, -3); points.Add(y);
            Point z = new Point("Z", 0, 0); points.Add(z);

            Segment vw = new Segment(v, w); segments.Add(vw);
            Segment vy = new Segment(v, y); segments.Add(vy);
            Segment wx = new Segment(w, x); segments.Add(wx);
            Segment xy = new Segment(x, y); segments.Add(xy);

            List<Point> pts = new List<Point>();
            pts.Add(v);
            pts.Add(z);
            pts.Add(x);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(w);
            pts.Add(z);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(w, v, z)), (Angle)parser.Get(new Angle(y, v, z))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(w, x, z)), (Angle)parser.Get(new Angle(y, x, z))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(z, w, x), new Triangle(z, y, x)));
        }
    }
}