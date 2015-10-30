using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page170ClassroomExercise05 : QuadrilateralsProblem
    {
        public Page170ClassroomExercise05(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 170 Classroom Exercise 05";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 10, 0); points.Add(b);
            Point c = new Point("C", 10, 10); points.Add(c);
            Point d = new Point("D", 0, 10); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);


            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, cd, ab));
            Angle abc = (Angle)parser.Get(new Angle(a, b, c));

            given.Add(new Strengthened(quad, new Parallelogram(quad)));
            given.Add(new Strengthened(abc, new RightAngle(abc)));
            given.Add(new GeometricCongruentSegments(ab, bc));

            goals.Add(new Strengthened(quad, new Square(quad)));
        }
    }
}
