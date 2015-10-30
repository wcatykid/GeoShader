using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 68 Problem 13
    //
    public class Page68Problem13 : CongruentTrianglesProblem
    {
        public Page68Problem13(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 68 Problem 13";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 1, 5); points.Add(b);
            Point c = new Point("C", 8, 5); points.Add(c);
            Point d = new Point("D", 7, 0); points.Add(d);

            Segment ba = new Segment(b, a); segments.Add(ba);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment cd = new Segment(c, d); segments.Add(cd);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(ba, cd));
            given.Add(new GeometricCongruentSegments(bc, ad));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(c, d, a)));
        }
    }
}