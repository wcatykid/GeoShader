using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Book i Page 120 Problem 8
    //
    public class IPage120Problem8 : ActualProofProblem
    {
        public IPage120Problem8(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Overlapping Right Triangles";


            Point a = new Point("A", 0, 3); points.Add(a);
            Point m = new Point("M", 2, 1.5); points.Add(m);
            Point b = new Point("B", 4, 3); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            //   given.Add(new Midpoint(m, (Segment)parser.Get(new Segment(a, c))));
            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, m)), (Segment)parser.Get(new Segment(m, c))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( m, (Segment)parser.Get(new Segment(b, d))))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, c, d))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(b, m, c), new Triangle(d, m, a)));
            goals.Add(new Strengthened((Angle)parser.Get(new Angle(a, d, c)), new RightAngle((Angle)parser.Get(new Angle(a, d, c)))));
            goals.Add(new GeometricCongruentTriangles(new Triangle(a, d, c), new Triangle(b, c, d)));

            Multiplication product = new Multiplication(new NumericValue(2), (Segment)parser.Get(new Segment(c, m)));
            goals.Add(new GeometricSegmentEquation(product, (Segment)parser.Get(new Segment(b, d))));
        }
    }
}