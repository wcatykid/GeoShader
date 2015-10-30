using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 41 Problem 15
    //
    public class Page41Problem15 : TransversalsProblem
    {
        public Page41Problem15(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 41 Problem 15";


            Point a = new Point("A", 1, -1); points.Add(a);
            Point b = new Point("B", 3, 1); points.Add(b);
            Point c = new Point("C", 5, 1); points.Add(c);
            Point d = new Point("D", 7, -1); points.Add(d);

            Point w = new Point("W", 0, 0); points.Add(w);
            Point x = new Point("X", 2, 0); points.Add(x);
            Point y = new Point("Y", 6, 0); points.Add(y);
            Point z = new Point("Z", 11, 0); points.Add(z);

            List<Point> pts = new List<Point>();
            pts.Add(w);
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, x, y)), (Angle)parser.Get(new Angle(c, y, x))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, x, w)), (Angle)parser.Get(new Angle(d, y, z))));
        }
    }
}