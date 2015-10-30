using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page312Theorem7_7_Semicircle: CirclesProblem
    {
        //Demonstrates: An angle inscribed in a semi circle is a right angle

        public Page312Theorem7_7_Semicircle(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);

            //diameter points plus extra point to define semicircle
            Point a = new Point("A", -5, 0); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 1, -Math.Sqrt(24)); points.Add(c);

            //vertex point
            Point x = new Point("X", -2, -Math.Sqrt(21)); points.Add(x);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ax = new Segment(a, x); segments.Add(ax);
            Segment bx = new Segment(b, x); segments.Add(bx);

            Circle c1 = new Circle(o, 5.0);
            circles.Add(c1);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle angle = (Angle)parser.Get(new Angle(a, x, b));
            goals.Add(new Strengthened(angle, new RightAngle(angle)));
        }
    }
}
