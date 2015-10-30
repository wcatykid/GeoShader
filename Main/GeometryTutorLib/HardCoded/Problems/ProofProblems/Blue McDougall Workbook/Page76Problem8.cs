using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 76 Problem 8
    //
    public class Page76Problem8 : SimilarTrianglesProblem
    {
        public Page76Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 76 Problem 8";


            Point s = new Point("S", 2, 8);  points.Add(s);
            Point r = new Point("R", 1, 4);  points.Add(r);
            Point t = new Point("T", 5, 4);  points.Add(t);
            Point q = new Point("Q", 4, 0);  points.Add(q);
            Point p = new Point("P", 0, 0);  points.Add(p);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(p, d)));

            Segment st = new Segment(s, t); segments.Add(st);
            Segment rt = new Segment(r, t); segments.Add(rt);
            Segment rq = new Segment(r, q); segments.Add(rq);
            Segment pq = new Segment(p, q); segments.Add(pq);

            List<Point> pts = new List<Point>();
            pts.Add(s);
            pts.Add(r);
            pts.Add(p);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel(st, rq));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(s, r)), (Segment)parser.Get(new Segment(r, p))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(s, r, t)), (Angle)parser.Get(new Angle(r, p, q))));

            goals.Add(new GeometricCongruentSegments(st, rq));
        }
    }
}