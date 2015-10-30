using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page296Theorem7_1 : CirclesProblem
    {
        //Demonstrates: Tangents are perpendicular to radii

        public Page296Theorem7_1(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point r = new Point("R", -3, -5); points.Add(r);
            Point s = new Point("S", 8, -5); points.Add(s);
            Point t = new Point("T", 0, -5); points.Add(t);

            Segment ot = new Segment(o, t); segments.Add(ot);

            List<Point> pnts = new List<Point>();
            pnts.Add(r);
            pnts.Add(t);
            pnts.Add(s);
            collinear.Add(new Collinear(pnts));

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment rs = (Segment)parser.Get(new Segment(r, s));

            CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(t, c, rs));
            given.Add(new Strengthened(cInter, new Tangent(cInter)));

            Intersection inter = parser.GetIntersection(ot, rs);
            goals.Add(new Strengthened(inter, new Perpendicular(inter)));
        }
    }
}
