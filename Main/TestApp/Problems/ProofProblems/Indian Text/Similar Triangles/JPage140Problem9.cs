using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Book J: Page 140 Problem 9
    //
    public class JPage140Problem9 : SimilarTrianglesProblem
    {
        public JPage140Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 140 Problem 9";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 6, 0); points.Add(b);
            Point c = new Point("C", 6, 6); points.Add(c);
            Point m = new Point("M", 5, 5); points.Add(m);
            Point p = new Point("P", 10, 0); points.Add(p);

            Point x = new Point("X", 6, 4); points.Add(x);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(x);
            pts.Add(p);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(p);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, b, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, m, p))));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, c), new Triangle(a, m, p)));
// given.Add(new GeometricProportionalSegments(ab, ac));
        }
    }
}