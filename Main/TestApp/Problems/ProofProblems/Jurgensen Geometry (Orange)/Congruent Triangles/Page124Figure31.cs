using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    // Isosceles Triangle Figure
    //
    //      A
    //     /|\
    //    / | \
    //   /  |  \
    //  /___|___\
    //  B   M    C
    //
    public class Page124Figure31 : CongruentTrianglesProblem
    {
        public Page124Figure31(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 2, 6); points.Add(a);
            Point m = new Point("M", 2, 0); points.Add(m);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment am = new Segment(a, m); segments.Add(am);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new IsoscelesTriangle(ac, ab, (Segment)parser.Get(new Segment(b, c))));
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(b, a, c)), am));
        }
    }
}