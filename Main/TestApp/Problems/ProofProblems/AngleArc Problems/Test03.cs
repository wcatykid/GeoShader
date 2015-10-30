using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Test03 : CirclesProblem
    {
        //Demonstrates: Inscribed angle half measure of intercepted arc and transitive substitution

        public Test03(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point r = new Point("R", -3, 4); points.Add(r);
            Point t = new Point("T", 3, 4); points.Add(t);
            Point v = new Point("V", -3, -4); points.Add(v);
            Point x = new Point("X", 0, -5); points.Add(x);

            Segment vr = new Segment(v, r); segments.Add(vr);
            Segment tx = new Segment(t, x); segments.Add(tx);

            Segment vt = new Segment(v, t);
            Segment rx = new Segment(r, x);
            Point i = vt.FindIntersection(rx);
            i = new Point("I", i.X, i.Y); points.Add(i);

            List<Point> pnts = new List<Point>();
            pnts.Add(v);
            pnts.Add(i);
            pnts.Add(o);
            pnts.Add(t);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(x);
            pnts.Add(i);
            pnts.Add(r);
            collinear.Add(new Collinear(pnts));

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(i, v, r)), (Angle)parser.Get(new Angle(i, x, t))));
        }
    }
}
