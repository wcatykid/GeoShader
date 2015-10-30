using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page296Theorem7_1_Test3 : CirclesProblem
    {
        //Demonstrates: A segment perpendicular to a radius is a tangent, tangents from point are congruent

        public Page296Theorem7_1_Test3(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);

            //Intersection point for two tangents
            Point c = new Point("C", 0, 6.25); points.Add(c);

            //Points for two tangents
            Point a = new Point("A", 3, 4); points.Add(a);
            Point b = new Point("B", -3, 4); points.Add(b);

            //Tangent and Radii segments
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ob = new Segment(o, b); segments.Add(ob);

            Circle circle = new Circle(o, 5.0);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Intersection inter1 = parser.GetIntersection(ac, oa);
            Intersection inter2 = parser.GetIntersection(bc, ob);
            given.Add(new Strengthened(inter1, new Perpendicular(inter1)));
            given.Add(new Strengthened(inter2, new Perpendicular(inter2)));

            //CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(a, circle, ap));
            //given.Add(new Strengthened(cInter, new Tangent(cInter)));

            goals.Add(new GeometricCongruentSegments(ac, bc));
        }
    }
}
