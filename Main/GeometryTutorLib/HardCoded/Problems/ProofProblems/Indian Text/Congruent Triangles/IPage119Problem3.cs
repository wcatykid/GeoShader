using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Book i Page 119 Problem 3
    //
    public class IPage119Problem3 : ActualProofProblem
    {
        public IPage119Problem3(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "I Page 119 Problem 3";

            Point a = new Point("A", 4, 0); points.Add(a);
            Point b = new Point("B", 4, 10); points.Add(b);
            Point c = new Point("C", 8, 10); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);
            Point o = new Point("O", 4, 5); points.Add(o);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ad, bc));
            given.Add(new Perpendicular(parser.GetIntersection(ad, new Segment(a, b))));
            given.Add(new Perpendicular(parser.GetIntersection(bc, new Segment(a, b))));

            goals.Add(new SegmentBisector(parser.GetIntersection((Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(a, b))), (Segment)parser.Get(new Segment(c, d))));
        }
    }
}