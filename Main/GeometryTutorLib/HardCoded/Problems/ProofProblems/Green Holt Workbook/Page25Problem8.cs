using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 25 Problem 8
    //
    public class Page25Problem8 : TransversalsProblem
    {
        public Page25Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 25 Problem 8";


            Point p = new Point("P", -5, 5); points.Add(p);
            Point q = new Point("Q", -4, 4); points.Add(q);
            Point r = new Point("R", 0, 0); points.Add(r);
            Point s = new Point("S", 4, 4); points.Add(s);
            Point t = new Point("T", 6, 6); points.Add(t);
            Point u = new Point("U", 0, 4); points.Add(u);

            Segment ur = new Segment(u, r); segments.Add(ur);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(q);
            pts.Add(r);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(u);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(r);
            pts.Add(s);
            pts.Add(t);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(p, q, u)), (Angle)parser.Get(new Angle(t, s, u))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(q, u, r))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(s, u, r))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(r, u, q), new Triangle(r, u, s)));
        }
    }
}