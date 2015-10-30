using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 37 Problem 2
    //
    public class Page37Problem2 : TransversalsProblem
    {
        public Page37Problem2(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 37 Problem 2";

            Point a = new Point("A", -4, 0); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", -4, 7); points.Add(c);

            Point x = new Point("X", 1, 3); points.Add(x);
            Point y = new Point("Y", 1, 0); points.Add(y);
            Point z = new Point("Z", 7, 0); points.Add(z);
            Point p = new Point("P", 5, 7); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);
            Segment yp = new Segment(y, p); segments.Add(yp);


                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            given.Add(new Complementary((Angle)parser.Get(new Angle(a, b, c)), (Angle)parser.Get(new Angle(x, y, p))));
            Addition sum = new Addition((Angle)parser.Get(new Angle(p, y, z)), (Angle)parser.Get(new Angle(x, y, p)));
            given.Add(new GeometricAngleEquation(sum, new NumericValue(90)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, c)), (Angle)parser.Get(new Angle(p, y, z))));
        }
    }
}