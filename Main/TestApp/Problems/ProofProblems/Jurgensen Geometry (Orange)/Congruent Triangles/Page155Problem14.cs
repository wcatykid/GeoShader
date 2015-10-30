using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 155 Problem 14
    //
    public class Page155Problem14 : CongruentTrianglesProblem
    {
        public Page155Problem14(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 155 Problem 14";


            Point r = new Point("R", 2, 5); points.Add(r);
            Point s = new Point("S", 0, 0); points.Add(s);
            Point t = new Point("T", 3.2, 2); points.Add(t);
            Point z = new Point("Z", 0.8, 2); points.Add(z);
            Point w = new Point("W", 4, 0); points.Add(w);

            Point x = new Point("X", 2, 1.25); points.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(r);
            pts.Add(z);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(t);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(z);
            pts.Add(x);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(t);
            pts.Add(x);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, z)),
                                                     (Segment)parser.Get(new Segment(r, t))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(z, s)),
                                                     (Segment)parser.Get(new Segment(t, w))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(r, s, t), new Triangle(r, w, z)));
        }
    }
}