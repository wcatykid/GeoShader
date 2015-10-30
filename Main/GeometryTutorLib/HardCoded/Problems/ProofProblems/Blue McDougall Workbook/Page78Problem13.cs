using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 79 Problem 7
    //
    public class Page79Problem7 : CongruentTrianglesProblem
    {
        public Page79Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 79 Problem 7";


            Point a = new Point("A", 0, 8); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 5, 0); points.Add(c);
            Point d = new Point("D", 5, 8); points.Add(d);

       //     System.Diagnostics.Debug.WriteLine(new Segment(b, c).FindIntersection(new Segment(a, d)));

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment ac = new Segment(a, c); segments.Add(ac);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, c)), (Angle)parser.Get(new Angle(c, d, b))));
            given.Add(new RightAngle(new Angle(a, b, c)));
            given.Add(new RightAngle(new Angle(b, c, d)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, c, b)));
        }
    }
}