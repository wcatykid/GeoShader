using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 301 problem 52
    //
    public class Page301Problem52 : ParallelLinesProblem
    {
        public Page301Problem52(bool onoff, bool complete) : base(onoff, complete)
        {

            problemName = "Page 301 Problem 52";


            Point p = new Point("P", 0, 5); points.Add(p);
            Point q = new Point("Q", 10, 5); points.Add(q);
            Point r = new Point("R", 10, 0); points.Add(r);
            Point s = new Point("S", 0, 0); points.Add(s);

            Segment pq = new Segment(p, q); segments.Add(pq);
            Segment pr = new Segment(p, r); segments.Add(pr);
            Segment ps = new Segment(p, s); segments.Add(ps);
            Segment qr = new Segment(q, r); segments.Add(qr);
            Segment rs = new Segment(r, s); segments.Add(rs);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(q, p, r)), (Angle)parser.Get(new Angle(p, r, s))));
            given.Add(new RightAngle(p, s, r));
            given.Add(new RightAngle(p, q, r));

            goals.Add(new GeometricCongruentTriangles(new Triangle(p, s, r), new Triangle(r, q, p)));
        }
    }
}