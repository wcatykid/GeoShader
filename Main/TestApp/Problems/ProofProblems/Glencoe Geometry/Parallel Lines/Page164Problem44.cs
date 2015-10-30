using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 164 problem 44
    //
    public class Page164Problem44 : ParallelLinesProblem
    {
        public Page164Problem44(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 164 Problem 44";

            Point l = new Point("L", 0, 9); points.Add(l);
            Point m = new Point("M", 12, 9); points.Add(m);
            Point n = new Point("N", 6, 0); points.Add(n);
            Point a = new Point("A", 4, 3); points.Add(a);
            Point b = new Point("B", 8, 3); points.Add(b);

            Segment lm = new Segment(l, m); segments.Add(lm);
            Segment ab = new Segment(a, b); segments.Add(ab);

            List<Point> pts = new List<Point>();
            pts.Add(n);
            pts.Add(a);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(b);
            pts.Add(m);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(n, l)),
                                                     (Segment)parser.Get(new Segment(n, m))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, l)),
                                                     (Segment)parser.Get(new Segment(b, m))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, n)),
                                                     (Segment)parser.Get(new Segment(b, n))));
        }
    }
}