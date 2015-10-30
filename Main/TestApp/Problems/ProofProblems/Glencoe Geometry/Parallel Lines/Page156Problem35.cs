using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 156 problem 35
    //
    public class Page156Problem35 : ParallelLinesProblem
    {
        public Page156Problem35(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 156 Problem 35";

            Point a = new Point("A", 9, 0); points.Add(a);
            Point b = new Point("B", 0, 3); points.Add(b);
            Point c = new Point("C", 0, 9); points.Add(c);
            Point d = new Point("D", 9, 9); points.Add(d);
            
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment cd = new Segment(c, d); segments.Add(cd);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, d, b)), (Angle)parser.Get(new Angle(d, b, c))));
            given.Add(new Perpendicular(parser.GetIntersection(new Segment(a, d), new Segment(c, d))));

            goals.Add(new Strengthened(parser.GetIntersection(new Segment(b, c), new Segment(c, d)),
                      new Perpendicular(parser.GetIntersection(new Segment(b, c), new Segment(c, d)))));
        }
    }
}