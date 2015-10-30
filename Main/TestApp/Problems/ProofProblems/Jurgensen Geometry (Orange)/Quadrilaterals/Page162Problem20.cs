using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page162Problem20 : CongruentTrianglesProblem
    {
        // Geometry; Page 162 Problem 20
        // Demonstrates usage of:
        // Definition of a parallelogram
        // Opposite sides of parallelogram are congruent
        // Opposite angles of parallelogram are congruent

        public Page162Problem20(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 162 Problem 20";

            Point a = new Point("A", -3, -3); points.Add(a);
            Point b = new Point("B", -7, -1); points.Add(b);
            Point c = new Point("C", -4, 2); points.Add(c);
            Point d = new Point("D", 4, -2); points.Add(d);
            Point e = new Point("E", 7, 1); points.Add(e);
            Point f = new Point("F", 3, 3); points.Add(f);
            Point x = new Point("X", 0, 0); points.Add(x);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral givenQuad1 = (Quadrilateral)parser.Get(new Quadrilateral(bc, (Segment)parser.Get(new Segment(a, x)), ab, (Segment)parser.Get(new Segment(c, x))));
            Quadrilateral givenQuad2 = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(f, x)), de, ef, (Segment)parser.Get(new Segment(d, x))));
            given.Add(new Strengthened(givenQuad1, new Parallelogram(givenQuad1)));
            given.Add(new Strengthened(givenQuad2, new Parallelogram(givenQuad2)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, c)), (Angle)parser.Get(new Angle(d, e, f))));
        }
    }
}