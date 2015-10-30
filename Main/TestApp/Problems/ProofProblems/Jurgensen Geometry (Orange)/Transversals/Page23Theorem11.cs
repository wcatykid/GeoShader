using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Midpoint Theorem
    //
    public class Page23Theorem11 : TransversalsProblem
    {
        public Page23Theorem11(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Midpoint Theorem";


            Point a = new Point("A", -3, 0); points.Add(a);
            Point m = new Point("M", 0, 0);  points.Add(m);
            Point b = new Point("B", 3, 0);  points.Add(b);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( m, (Segment)parser.Get(new Segment(a, b))))));

            Multiplication product1 = new Multiplication(new NumericValue(2), (Segment)parser.Get(new Segment(a, m)));
            goals.Add(new GeometricSegmentEquation(product1, (Segment)parser.Get(new Segment(a, b))));

            Multiplication product2 = new Multiplication(new NumericValue(2), (Segment)parser.Get(new Segment(m, b)));
            goals.Add(new GeometricSegmentEquation(product2, (Segment)parser.Get(new Segment(a, b))));
        }
    }
}