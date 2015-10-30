using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page296Theorem7_1_Test2 : CirclesProblem
    {
        //Demonstrates: Tangents are perpendicular to radii

        public Page296Theorem7_1_Test2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 3, 4); points.Add(a);
            Point p = new Point("P", 0, 6.25); points.Add(p);

            Segment ap = new Segment(a, p); segments.Add(ap);
            Segment ao = new Segment(a, o); segments.Add(ao);

            Circle circle = new Circle(o, 5.0);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(a, circle, ap));
            given.Add(new Strengthened(cInter, new Tangent(cInter)));

            Intersection inter = parser.GetIntersection(ao, ap);
            goals.Add(new Strengthened(inter, new Perpendicular(inter)));
        }
    }
}
