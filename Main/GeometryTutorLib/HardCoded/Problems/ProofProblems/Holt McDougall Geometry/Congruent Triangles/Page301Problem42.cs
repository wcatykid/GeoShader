using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 301 problem 42
    //
    public class Page301Problem42 : CongruentTrianglesProblem
    {
        public Page301Problem42(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 301 Problem 42";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", 4, 3); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ad = new Segment(a, d); segments.Add(ad);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( c, (Segment)parser.Get(new Segment(b, d))))));
            given.Add(new RightAngle(b, a, d));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, c)), ac));
            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(d, c)), ac));
        }
    }
}