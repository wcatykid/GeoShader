using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 76 Problem 7
    //
    public class Page76Problem7 : CongruentTrianglesProblem
    {
        public Page76Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 76 Problem 7";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 7, 3); points.Add(b);
            Point c = new Point("C", 14, 3); points.Add(c);
            Point d = new Point("D", 7, 0); points.Add(d);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment cd = new Segment(c, d); segments.Add(cd);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ab, cd));
            given.Add(new RightAngle(new Angle(a, d, b)));
            given.Add(new RightAngle(new Angle(d, b, c)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, a, b)), (Angle)parser.Get(new Angle(b, c, d))));
        }
    }
}