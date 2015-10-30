using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 113 Problem 7
    //
    public class Page113Problem7 : CongruentTrianglesProblem
    {
        public Page113Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 113 Problem 7";


            Point a = new Point("A", -4, 3);          points.Add(a);
            Point b = new Point("B", -4.0 / 3.0, -1); points.Add(b);
            Point c = new Point("C", 0, 0);           points.Add(c);
            Point p = new Point("P", 4, 3);           points.Add(p);
            Point q = new Point("Q", 4.0 / 3.0, -1);  points.Add(q);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment pq = new Segment(p, q); segments.Add(pq);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p);
            pts.Add(c);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(c, q))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(c, p))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(p, q, c)));
        }
    }
}