using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class IPage120Problem6 : CongruentTrianglesProblem
    {
        public IPage120Problem6(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 120 Problem 6";

            Point a = new Point("A", 2, 6);  points.Add(a);
            Point b = new Point("B", 0, 0);  points.Add(b);
            Point c = new Point("C", 10, 0); points.Add(c);
            Point d = new Point("D", 4, 0);  points.Add(d);
            Point e = new Point("E", 12, 6); points.Add(e);


            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment ec = new Segment(e, c); segments.Add(ec);
            Segment de = new Segment(d, e); segments.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ac, ae));
            given.Add(new GeometricCongruentSegments(ab, ad));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, d)), (Angle)parser.Get(new Angle(e, a, c))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, c)), de));
        }
    }
}