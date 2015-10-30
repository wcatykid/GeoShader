using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
    //
    // Geometry; Page 24 Problem 7
    //
    public class Page24Problem7 : TransversalsProblem
    {
        public Page24Problem7(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 24 Problem 7";


            Point a = new Point("A", 1, 4); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 3.5, 3); points.Add(c);
            Point d = new Point("D", 6, 2); points.Add(d);
            Point e = new Point("E", 7, 6); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment de = new Segment(d, e); segments.Add(de);

            // System.Diagnostics.Debug.WriteLine("Intersection: " + new Segment(b, e).FindIntersection(new Segment(a, d)));

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(c);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( c, (Segment)parser.Get(new Segment(a, d))))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( c, (Segment)parser.Get(new Segment(b, e))))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, c), new Triangle(d, e, c)));
        }
    }
}