using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 144 Problem 2
    //
    public class Page144Problem02 : CongruentTrianglesProblem
    {
        public Page144Problem02(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Problem 2";


            Point a = new Point("A", 0, 20); points.Add(a);
            Point b = new Point("B", 5, 32); points.Add(b);
            Point c = new Point("C", 45, 20); points.Add(c);
            Point d = new Point("D", 40, 8); points.Add(d);
            Point e = new Point("E", 5, 20); points.Add(e);
            Point f = new Point("F", 40, 20); points.Add(f);
            
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment be = new Segment(b, e); segments.Add(be);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment df = new Segment(d, f); segments.Add(df);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(ab), (Segment)parser.Get(cd)));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(ad), (Segment)parser.Get(bc)));
            given.Add(new RightAngle(a, e, b));
            given.Add(new RightAngle(c, f, d));

            goals.Add(new GeometricCongruentSegments(be, df));
        }
    }
}