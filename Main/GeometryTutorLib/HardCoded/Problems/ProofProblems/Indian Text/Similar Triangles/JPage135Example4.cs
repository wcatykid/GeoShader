using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class JPage135Example4 : SimilarTrianglesProblem
    {
        public JPage135Example4(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 135 Example 4";


            Point p = new Point("P", 1, 7);       points.Add(p);
            Point q = new Point("Q", 0, 0);       points.Add(q);
            Point r = new Point("R", 11.5, 11.5); points.Add(r);
            Point s = new Point("S", 10, 1);      points.Add(s);
            Point o = new Point("O", 4.6, 4.6);   points.Add(o);

            Segment pq = new Segment(p, q); segments.Add(pq);
            Segment rs = new Segment(r, s); segments.Add(rs);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(o);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(o);
            pts.Add(r);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel(pq, rs));

            goals.Add(new GeometricSimilarTriangles(new Triangle(p, o, q), new Triangle(s, o, r)));
        }
    }
}