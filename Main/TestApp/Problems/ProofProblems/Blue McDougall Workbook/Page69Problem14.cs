using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
    //
    // Geometry; Page 69 Problem 14
    //
    public class Page69Problem14 : CongruentTrianglesProblem
    {
        public Page69Problem14(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 69 Problem 14";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 7, -6); points.Add(b);
            Point c = new Point("C", 14, 0); points.Add(c);
            Point d = new Point("D", 7, 0); points.Add(d);

            Segment ba = new Segment(b, a); segments.Add(ba);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment bd = new Segment(b, d); segments.Add(bd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(ba, bc));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(d, (Segment)parser.Get(new Segment(a, c))))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(c, b, d)));
        }
    }
}