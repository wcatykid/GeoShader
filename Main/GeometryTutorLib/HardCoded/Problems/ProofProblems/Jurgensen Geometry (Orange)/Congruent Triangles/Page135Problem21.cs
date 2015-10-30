using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 135 #21
    //
    public class Page135Problem21 : CongruentTrianglesProblem
    {
        public Page135Problem21(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 135 Problem 21";


            Point f = new Point("F", -3, 4);   points.Add(f);
            Point l = new Point("L", -1, 4);  points.Add(l);
            Point a = new Point("A", 1, 4);   points.Add(a);
            Point k = new Point("K", 3, 4); points.Add(k);
            Point m = new Point("M", -1.5, 2);   points.Add(m);
            Point n = new Point("N", 1.5, 2);  points.Add(n);
            Point s = new Point("S", 0, 0);   points.Add(s);

            Point x = new Point("X", 0, 3.2); points.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(m);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(f);
            pts.Add(l);
            pts.Add(a);
            pts.Add(k);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(x);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(m);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(n);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(f, l)),
                                                     (Segment)parser.Get(new Segment(a, k))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(s, f)),
                                                     (Segment)parser.Get(new Segment(s, k))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( m, (Segment)parser.Get(new Segment(s, f))))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( n, (Segment)parser.Get(new Segment(s, k))))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, m)),
                                                     (Segment)parser.Get(new Segment(l, n))));
        }
    }
}