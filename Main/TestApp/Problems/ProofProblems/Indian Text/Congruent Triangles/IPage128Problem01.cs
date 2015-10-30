using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Book I: Page 128 Problem 1
    //
    public class IPage128Problem01 : CongruentTrianglesProblem
    {
        public IPage128Problem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 128 Problem 1";

            Point a = new Point("A", 2, 7); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);
            Point d = new Point("D", 2, 3); points.Add(d);
            Point p = new Point("P", 2, 0); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(p);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new IsoscelesTriangle((Triangle)parser.Get(new Triangle(a, b, c))));
            given.Add(new IsoscelesTriangle((Triangle)parser.Get(new Triangle(d, b, c))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, d), new Triangle(a, c, d)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, b, p), new Triangle(a, c, p)));
            goals.Add(new AngleBisector((Angle)parser.Get(new Angle(b, a, c)), (Segment)parser.Get(new Segment(a, p))));
            goals.Add(new AngleBisector((Angle)parser.Get(new Angle(b, d, c)), (Segment)parser.Get(new Segment(a, p))));
            goals.Add(new PerpendicularBisector(parser.GetIntersection(new Segment(a, p), new Segment(b, c)),
                                                (Segment)parser.Get(new Segment(a, p))));
        }
    }
}