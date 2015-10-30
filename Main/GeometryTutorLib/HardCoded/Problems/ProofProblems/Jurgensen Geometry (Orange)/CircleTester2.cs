using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class CircleTester2 : ActualProofProblem
    {
        //
        //     A  _______M________ B
        //       |                |
        //       |                |
        //       | Right Triangle |
        //Z    N |        O       | P       W
        //       |                |
        //       |                |
        //    D  |________________| C
        //
        public CircleTester2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 4); points.Add(a);
            Point m = new Point("M", 2, 4);  points.Add(m);
            Point b = new Point("B", 4, 4);  points.Add(b);
            Point d = new Point("D", 0, 0); points.Add(d);
            Point o = new Point("O", 2, 2); points.Add(o);
            Point c = new Point("C", 4, 0); points.Add(c);
            Point n = new Point("N", 0, 2); points.Add(n);
            Point p = new Point("P", 4, 2); points.Add(p);
            Point z = new Point("Z", -4, 2); points.Add(z);
            Point w = new Point("W", 10, 2); points.Add(w);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(n);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(z);
            pts.Add(n);
            pts.Add(o);
            pts.Add(p);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment mp = new Segment(m, p); segments.Add(mp);

            // Add circles last;
            circles.Add(new Circle(o, 2.0));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(mn, mp));

            goals.Add(new GeometricCongruentSegments(cd, (Segment)parser.Get(new Segment(p, n))));
        }
    }
}