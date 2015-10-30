using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 197 Problem 35
    //
    public class Page197Problem35 : TransversalsProblem
    {
        public Page197Problem35(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 197 Problem 35";


            Point a = new Point("A", 0, 5); points.Add(a);
            Point b = new Point("B", 3, 8); points.Add(b);
            Point c = new Point("C", 5, 0); points.Add(c);
            Point d = new Point("D", 9000, 5); points.Add(d);
            Point e = new Point("E", 0, 0); points.Add(e);
            Point f = new Point("F", 0, 30); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ad = new Segment(a, d); segments.Add(ad);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(a);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(f, a, b)),
                                                   (Angle)parser.Get(new Angle(d, a, b))));
            
            goals.Add(new Strengthened(parser.GetIntersection(new Segment(a, b), new Segment(a, c)),
                                       new Perpendicular(parser.GetIntersection(new Segment(a, b), new Segment(a, c)))));
        }
    }
}