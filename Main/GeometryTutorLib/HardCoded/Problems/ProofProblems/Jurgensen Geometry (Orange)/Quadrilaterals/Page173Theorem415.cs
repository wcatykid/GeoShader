using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page173Theorem415 : QuadrilateralsProblem
    {
        // Demonstrates: 
        // Definition of trapezoid & isosceles trapezoid
        // Base angles of isosceles trapezoid are congruent

        public Page173Theorem415(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 173 Theorem 4-15";

            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 10, 0); points.Add(b);
            Point c = new Point("C", 8, 5); points.Add(c);
            Point d = new Point("D", 2, 5); points.Add(d);

            //trapezoid sides
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(da, bc, cd, ab));
            given.Add(new Strengthened(quad, new Trapezoid(quad)));
            given.Add(new GeometricCongruentSegments(bc, da));

            goals.Add(new Strengthened(quad, new IsoscelesTrapezoid(quad)));
        }
    }
}
