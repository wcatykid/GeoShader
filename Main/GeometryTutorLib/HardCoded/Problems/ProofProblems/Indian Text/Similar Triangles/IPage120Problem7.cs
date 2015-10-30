using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class IPage120Problem7 : CongruentTrianglesProblem
    {
        public IPage120Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 120 Problem 7";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 10, 0); points.Add(b);
            Point p = new Point("P", 5, 0); points.Add(p);
            Point d = new Point("D", 7, 8); points.Add(d);
            Point e = new Point("E", 3, 8); points.Add(e);

//            System.Diagnostics.Debug.Write(new Segment(q, r).FindIntersection(new Segment(p, s)));

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment be = new Segment(b, e); segments.Add(be);
            Segment ep = new Segment(e, p); segments.Add(ep);
            Segment dp = new Segment(d, p); segments.Add(dp);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( p, (Segment)parser.Get(new Segment(a, b))))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, d)), (Angle)parser.Get(new Angle(a, b, e))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(e, p, a)), (Angle)parser.Get(new Angle(d, p, b))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(d, a, p), new Triangle(e, b, p)));
            goals.Add(new GeometricCongruentSegments(ad, be));
        }
    }
}