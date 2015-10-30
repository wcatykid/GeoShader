using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Test02 : CirclesProblem
    {
        //Demonstrates: ChordTangentAngleHalfInterceptedArc

        public Test02(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point r = new Point("R", -3, -5); points.Add(r);
            Point s = new Point("S", 8, -5); points.Add(s);
            Point t = new Point("T", 0, -5); points.Add(t);
            Point u = new Point("U", -3, 4); points.Add(u);
            Point v = new Point("V", 3, 4); points.Add(v);

            Segment tu = new Segment(t, u); segments.Add(tu);
            Segment tv = new Segment(t, v); segments.Add(tv);

            List<Point> pnts = new List<Point>();
            pnts.Add(r);
            pnts.Add(t);
            pnts.Add(s);
            collinear.Add(new Collinear(pnts));

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            MinorArc m1 = (MinorArc)parser.Get(new MinorArc(c, t, u));
            MinorArc m2 = (MinorArc)parser.Get(new MinorArc(c, t, v));
            Segment rs = (Segment)parser.Get(new Segment(r, s));

            CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(t, c, rs));

            given.Add(new Strengthened(cInter, new Tangent(cInter)));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(r, t, u)), (Angle)parser.Get(new Angle(s, t, v))));

            goals.Add(new GeometricCongruentArcs(m1, m2));
        }
    }
}

