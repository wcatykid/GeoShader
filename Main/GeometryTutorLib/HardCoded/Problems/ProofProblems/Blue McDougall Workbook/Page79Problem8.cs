using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 79 Problem 8
    //
    public class Page79Problem8 : SimilarTrianglesProblem
    {
        public Page79Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 79 Problem 8";


            Point k = new Point("K", 3, 12);  points.Add(k);
            Point l = new Point("L", 1, 4);  points.Add(l);
            Point m = new Point("M", 9, 4);  points.Add(m);
            Point p = new Point("P", 4, 0);  points.Add(p);
            Point a = new Point("A", 0, 0);  points.Add(a);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment km = new Segment(k, m); segments.Add(km);
            Segment lm = new Segment(l, m); segments.Add(lm);
            Segment lp = new Segment(l, p); segments.Add(lp);
            Segment ap = new Segment(a, p); segments.Add(ap);

            List<Point> pts = new List<Point>();
            pts.Add(k);
            pts.Add(l);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel(km, lp));
            given.Add(new GeometricParallel(lm, ap));

            goals.Add(new GeometricSimilarTriangles(new Triangle(k, m, l), new Triangle(l, p, a)));
        }
    }
}