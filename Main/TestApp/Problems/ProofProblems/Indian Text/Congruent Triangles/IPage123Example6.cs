using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Book I: Page 123 Example 6
    //
    public class IPage123Example6 : CongruentTrianglesProblem
    {
        public IPage123Example6(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 123 Example 6";


            Point a = new Point("A", 3.5, 4); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 7, 0); points.Add(c);
            Point d = new Point("D", 2, 0); points.Add(d);
            Point e = new Point("E", 5, 0); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ae = new Segment(a, e); segments.Add(ae);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(e);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new IsoscelesTriangle((Triangle)parser.Get(new Triangle(a, b, c))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, e)), (Segment)parser.Get(new Segment(c, d))));

            goals.Add(new GeometricCongruentSegments(ad, ae));
        }
    }
}