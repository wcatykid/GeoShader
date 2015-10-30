using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 42 Problem 16
    //
    public class Page42Problem16 : TransversalsProblem
    {
        public Page42Problem16(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 42 Problem 16";


            Point a = new Point("A", -4, 0); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);

            Point x = new Point("X", -3, -2); points.Add(x);
            Point y = new Point("Y", 0, -5); points.Add(y);
            Point z = new Point("Z", 2, -3); points.Add(z);

            Segment bx = new Segment(b, x); segments.Add(bx);
            Segment by = new Segment(b, y); segments.Add(by);
            Segment bz = new Segment(b, z); segments.Add(bz);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Complementary((Angle)parser.Get(new Angle(a, b, x)), (Angle)parser.Get(new Angle(x, b, y))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, x)), (Angle)parser.Get(new Angle(y, b, z))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(x, b, y)), (Angle)parser.Get(new Angle(c, b, z))));

            goals.Add(new Complementary((Angle)parser.Get(new Angle(y, b, z)), (Angle)parser.Get(new Angle(z, b, c))));
        }
    }
}