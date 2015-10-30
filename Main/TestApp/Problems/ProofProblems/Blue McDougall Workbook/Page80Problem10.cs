using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 80 Problem 10
    //
    public class Page80Problem10 : CongruentTrianglesProblem
    {
        public Page80Problem10(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 80 Problem 10";


            Point a = new Point("A", 2, 7); points.Add(a);
            Point b = new Point("B", 8, 7); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 10, 0); points.Add(d);
            Point e = new Point("E", 5, 4.375); points.Add(e);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(e);
            pts.Add(b);
            collinear.Add(new Collinear(pts));
            
                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, a, d)), (Angle)parser.Get(new Angle(c, b, d))));
            given.Add(new GeometricCongruentSegments(ac, bd));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, a, b)), (Angle)parser.Get(new Angle(c, b, a))));
        }
    }
}