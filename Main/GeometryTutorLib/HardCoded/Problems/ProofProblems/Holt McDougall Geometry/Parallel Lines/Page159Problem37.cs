using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 159 Problem 37
    //
    public class Page159Problem37 : ParallelLinesProblem
    {
        public Page159Problem37(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 159 Problem 37";


            Point a = new Point("A", -3, -1); points.Add(a);
            Point b = new Point("B", 0, 0);   points.Add(b);
            Point c = new Point("C", 6, 2);   points.Add(c);
            Point d = new Point("D", 1, -4);  points.Add(d);
            Point x = new Point("X", -5, 7);  points.Add(x);
            Point y = new Point("Y", -2, 8);  points.Add(y);
            Point z = new Point("Z", 1, 9);   points.Add(z);
            Point q = new Point("Q", -3, 12); points.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(x, z))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(x, y, q)), (Angle)parser.Get(new Angle(d, b, c))));
        }
    }
}