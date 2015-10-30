using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 48 Problem 23-31
    //
    public class Page48Problem23To31 : ParallelLinesProblem
    {
        public Page48Problem23To31(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 48 Problem 23-31";


            Point a = new Point("A", 0, 5); points.Add(a);
            Point b = new Point("B", 2, 4); points.Add(b);
            Point c = new Point("C", 4, 3); points.Add(c);
            Point m = new Point("M", 3, 6); points.Add(m);

            Point x = new Point("X", -2, 1); points.Add(x);
            Point y = new Point("Y", 0, 0); points.Add(y);
            Point z = new Point("Z", 2, -1); points.Add(z);
            Point q = new Point("Q", -1, -2); points.Add(q);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(y);
            pts.Add(b);
            pts.Add(m);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            given.Add(new Perpendicular(parser.GetIntersection(new Segment(m, q), new Segment(a, c))));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(x, z))));

            goals.Add(new Perpendicular(parser.GetIntersection(new Segment(m, q), new Segment(x, z))));
        }
    }
}