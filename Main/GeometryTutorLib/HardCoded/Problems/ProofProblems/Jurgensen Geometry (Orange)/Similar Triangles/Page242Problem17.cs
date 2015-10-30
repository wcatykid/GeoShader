using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 242 Problem 17
    //
    public class Page242Problem17 : SimilarTrianglesProblem
    {
        public Page242Problem17(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 242 Problem 17";


            Point a = new Point("A", 1, 5); points.Add(a);
            Point b = new Point("B", 5, 5); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);
            Point n = new Point("N", 3, 3); points.Add(n);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(n);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(n);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, b, a)), (Angle)parser.Get(new Angle(a, d, c))));

            goals.Add(new GeometricSimilarTriangles(new Triangle(n, c, d), new Triangle(n, a, b)));
        }
    }
}