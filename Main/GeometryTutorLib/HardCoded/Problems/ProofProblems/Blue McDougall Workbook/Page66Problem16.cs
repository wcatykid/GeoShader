using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 66 Problem 16
    //
    public class Page66Problem16 : CongruentTrianglesProblem
    {
        public Page66Problem16(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 66 Problem 16";


            Point a = new Point("A", 1, 5); points.Add(a);
            Point b = new Point("B", 8, 5); points.Add(b);
            Point c = new Point("C", 7, 0); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);

            Segment ba = new Segment(b, a); segments.Add(ba);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment cd = new Segment(c, d); segments.Add(cd);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ba, cd));
            given.Add(new GeometricCongruentSegments(bc, ad));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, d)), (Angle)parser.Get(new Angle(c, d, b))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, d, b)), (Angle)parser.Get(new Angle(c, b, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(c, d, b)));
        }
    }
}