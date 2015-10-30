using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 285 problem 21
    //
    public class Page285Problem21 : CongruentTrianglesProblem
    {
        public Page285Problem21(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 285 Problem 21";


            Point a = new Point("A", 3, 4); points.Add(a);
            Point b = new Point("B", 9, 4); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);
            Point e = new Point("E", 12, 0); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment be = new Segment(b, e); segments.Add(be);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(d, e))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, c, d)), (Angle)parser.Get(new Angle(b, e, d))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, a, d)), (Angle)parser.Get(new Angle(d, b, e))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, d)), (Angle)parser.Get(new Angle(d, b, a))));
        }
    }
}