using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 72 Problem 17
    //
    public class Page72Problem17 : CongruentTrianglesProblem
    {
        public Page72Problem17(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 72 Problem 17";


            Point a = new Point("A", 2, 5); points.Add(a);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point b = new Point("B", 12, 5); points.Add(b);
            Point d = new Point("D", 10, 0); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment db = new Segment(d, b); segments.Add(db);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(ab, cd));
            given.Add(new GeometricCongruentSegments(ab, cd));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, c, b)));
        }
    }
}