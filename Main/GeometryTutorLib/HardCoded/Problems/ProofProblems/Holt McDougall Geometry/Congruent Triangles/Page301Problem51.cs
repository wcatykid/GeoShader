using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 301 problem 51
    //
    public class Page301Problem51 : CongruentTrianglesProblem
    {
        public Page301Problem51(bool onoff, bool complete) : base(onoff, complete)
        {
        
            problemName = "Page 301 Problem 51";


            Point a = new Point("A", 0, 5); points.Add(a);
            Point b = new Point("B", 3, 10); points.Add(b);
            Point c = new Point("C", 10, 5); points.Add(c);
            Point d = new Point("D", 7, 0); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, a, b)), (Angle)parser.Get(new Angle(a, c, d))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, c, b)), (Angle)parser.Get(new Angle(c, a, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(c, d, a)));
        }
    }
}