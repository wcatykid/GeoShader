using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 226 problem 41
    //
    public class Page226Problem41 : ParallelLinesProblem
    {
        public Page226Problem41(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 226 Problem 41";

            Point a = new Point("A", -2, 0); points.Add(a);
            Point b = new Point("B", 8, 0); points.Add(b);
            Point c = new Point("C", -6, -4); points.Add(c);
            Point d = new Point("D", 9, -2); points.Add(d);

            Point q = new Point("Q", 3, 2); points.Add(q);
            Point r = new Point("R", 0, 0); points.Add(r);
            Point s = new Point("S", 6, 0); points.Add(s);
            
            Segment qr = new Segment(q, r); segments.Add(qr);
            Segment qs = new Segment(q, s); segments.Add(qs);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(r);
            pts.Add(s);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(r);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(s);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, r, q)), (Angle)parser.Get(new Angle(b, s, q))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(q, r)), (Segment)parser.Get(new Segment(q, s))));
        }
    }
}