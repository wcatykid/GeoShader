using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class JPage140Problem7 : SimilarTrianglesProblem
    {
        public JPage140Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 140 Problem 7";


            Point a = new Point("A", 0, 0);    points.Add(a);
            Point b = new Point("B", 18, 0);   points.Add(b);
            Point c = new Point("C", 2, 8);   points.Add(c);
            Point d = new Point("D", 3.6, 7.2);  points.Add(d);
            Point e = new Point("E", 2, 0);    points.Add(e);
            Point p = new Point("P", 2, 4); points.Add(p);

            Segment ac = new Segment(a, c); segments.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(p);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(p);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Altitude((Triangle)parser.Get(new Triangle(a, b, c)), (Segment)parser.Get(new Segment(a, d))));
            given.Add(new Altitude((Triangle)parser.Get(new Triangle(a, b, c)), (Segment)parser.Get(new Segment(c, e))));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, e, p), new Triangle(c, d, p)));
            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, d), new Triangle(c, b, e)));
            goals.Add(new GeometricSimilarTriangles(new Triangle(a, e, p), new Triangle(a, d, b)));
            goals.Add(new GeometricSimilarTriangles(new Triangle(p, d, c), new Triangle(b, e, c)));
        }
    }
}