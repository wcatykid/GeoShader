using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page170ClassroomExercise02 : QuadrilateralsProblem
    {
        // Geometry; Page 172 Problem 19
        // Demonstrates: Definition of rhombus: If two consecutive sides of a parallelogram are congruent, then the parallelogram is a rhombus.

        public Page170ClassroomExercise02(bool onoff, bool complete) : base(onoff, complete)
        {
            Point n = new Point("N", 0, 0); points.Add(n);
            Point g = new Point("G", 5, 0); points.Add(g);
            Point c = new Point("C", 3, 4); points.Add(c);
            Point t = new Point("T", 8, 4); points.Add(t);

            Segment nc = new Segment(n, c); segments.Add(nc);
            Segment ng = new Segment(n, g); segments.Add(ng);
            Segment gt = new Segment(g, t); segments.Add(gt);
            Segment ct = new Segment(c, t); segments.Add(ct);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(nc, gt, ct, ng));
            given.Add(new Strengthened(quad, new Parallelogram(quad)));
            given.Add(new GeometricCongruentSegments(nc, ct));

            goals.Add(new Strengthened(quad, new Rhombus(quad)));
        }
    }
}
