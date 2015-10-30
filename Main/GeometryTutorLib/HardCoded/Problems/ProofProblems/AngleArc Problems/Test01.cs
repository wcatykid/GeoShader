using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Test01 : CirclesProblem
    {
        //Demonstrates: Use of AngleArc equation, Central angle equal to intercepted arc

        public Test01(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -5, 0); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 0, 5); points.Add(c);


            Segment oc = new Segment(o, c); segments.Add(oc);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 5.0);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            MinorArc m1 = (MinorArc)parser.Get(new MinorArc(circle, a, c));
            MinorArc m2 = (MinorArc)parser.Get(new MinorArc(circle, b, c));

            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, o, c)), (Angle)parser.Get(new Angle(b, o, c))));
            goals.Add(new GeometricCongruentArcs(m1, m2));
        }
    }
}
