using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
namespace GeometryTutorLib.GeometryTestbed
{
    class Page169Theorem413 : QuadrilateralsProblem
    {
        // Geometry; Page 169 Theorem 4-13
        // Demonstrates usage of:
        // Definition of a rectangle

        public Page169Theorem413(bool onoff, bool complete)
            : base(onoff, complete)
        {
            problemName = "Page 169 Theorem 4-13";

            Point g = new Point("G", 0, 3); points.Add(g);
            Point r = new Point("R", 5, 3); points.Add(r);
            Point a = new Point("A", 5, 0); points.Add(a);
            Point m = new Point("M", 0, 0); points.Add(m);

            Segment gr = new Segment(g, r); segments.Add(gr);
            Segment ra = new Segment(r, a); segments.Add(ra);
            Segment am = new Segment(a, m); segments.Add(am);
            Segment mg = new Segment(m, g); segments.Add(mg);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad= (Quadrilateral)parser.Get(new Quadrilateral(mg, ra, gr, am));
            Angle angle = (Angle)parser.Get(new Angle(g, m, a));
            given.Add(new Strengthened(quad, new Parallelogram(quad)));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            goals.Add(new Strengthened(quad, new Rectangle(quad)));
        }
    }
}
