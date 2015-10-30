using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 156 problem 37
    //
    public class Page156Problem37 : ParallelLinesProblem
    {
        public Page156Problem37(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 156 Problem 37";

            Point p = new Point("P", 9, 0); points.Add(p);
            Point q = new Point("Q", 3, 0); points.Add(q);
            Point r = new Point("R", 0, 5); points.Add(r);
            Point s = new Point("S", 6, 5); points.Add(s);
            
            Segment pq = new Segment(p, q); segments.Add(pq);
            Segment ps = new Segment(p, s); segments.Add(ps);
            Segment qr = new Segment(q, r); segments.Add(qr);
            Segment qs = new Segment(q, s); segments.Add(qs);
            Segment rs = new Segment(r, s); segments.Add(rs);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(r, s, p)), (Angle)parser.Get(new Angle(p, q, r))));
            given.Add(new Supplementary((Angle)parser.Get(new Angle(q, r, s)), (Angle)parser.Get(new Angle(p, q, r))));

            goals.Add(new GeometricParallel(ps, qr));
        }
    }
}