using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 145 Problem 4
    //
    public class Page145Problem04 : CongruentTrianglesProblem
    {
        public Page145Problem04(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 145 Problem 4";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point d = new Point("D", 1, 4); points.Add(d);
            Point c = new Point("C", 6, 4); points.Add(c);
            Point l = new Point("L", 2, 3); points.Add(l);
            Point m = new Point("M", 4, 1); points.Add(m);
            
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment al = new Segment(a, l); segments.Add(al);
            Segment cm = new Segment(c, m); segments.Add(cm);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(l);
            pts.Add(m);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ab, cd));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, d, l)), (Angle)parser.Get(new Angle(m, b, a))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, a, l)), (Angle)parser.Get(new Angle(m, c, b))));

            goals.Add(new GeometricCongruentSegments(al, cm));
        }
    }
}