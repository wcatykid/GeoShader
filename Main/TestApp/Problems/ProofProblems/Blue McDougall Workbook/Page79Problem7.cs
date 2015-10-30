using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 78 Problem 13
    //
    public class Page78Problem13 : CongruentTrianglesProblem
    {
        public Page78Problem13(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 78 Problem 13";


            Point a = new Point("A", 2, 2); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 10, 0); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);
            Point e = new Point("E", 8, -2); points.Add(e);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ce = new Segment(c, e); segments.Add(ce);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, d, b)), (Angle)parser.Get(new Angle(b, c, e))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(b, e))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(d, b)), (Segment)parser.Get(new Segment(c, b))));
        }
    }
}