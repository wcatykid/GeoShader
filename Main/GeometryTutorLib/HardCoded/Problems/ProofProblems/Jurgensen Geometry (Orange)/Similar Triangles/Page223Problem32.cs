using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 223 Problem 32
    //
    public class Page223Problem32 : CongruentTrianglesProblem
    {
        public Page223Problem32(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 223 Problem 32";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 6, 12); points.Add(d);
            Point m = new Point("M", 2, 4); points.Add(m);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            collinear.Add(new Collinear(pts));
            
                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(ab), (Segment)parser.Get(cd)));
            given.Add(new RightAngle(b, a, c));
            given.Add(new RightAngle(a, c, d));
        }
    }
}