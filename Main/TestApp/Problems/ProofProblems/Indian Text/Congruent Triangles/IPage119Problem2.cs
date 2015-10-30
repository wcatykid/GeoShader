using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Book i Page 119 Problem 2
    //
    public class IPage119Problem2 : ActualProofProblem
    {
        public IPage119Problem2(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "I Page 119 Problem 2";

            Point a = new Point("A", 5, 4); points.Add(a);
            Point x = new Point("X", 3, 2.4); points.Add(x);
            Point b = new Point("B", 1, 4); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);

            // System.Diagnostics.Debug.WriteLine(new Segment(a, c).FindIntersection(new Segment(b, d)));

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            // given.Add(new Midpoint(m, (Segment)parser.Get(new Segment(a, c))));
            given.Add(new GeometricCongruentSegments(ad, bc));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, c)), (Angle)parser.Get(new Angle(b, a, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(b, a, c)));
            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, d)), (Segment)parser.Get(new Segment(a, c))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, d)), (Angle)parser.Get(new Angle(b, a, c))));
        }
    }
}