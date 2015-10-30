using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page307Theorem7_5 : CirclesProblem
    {
        //Demonstrates: A diameter perpendicular to a chord bisects the chord

        public Page307Theorem7_5(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);

            Point a = new Point("A", -3, -4); points.Add(a);
            Point b = new Point("B", 3, -4); points.Add(b);

            Point x = new Point("X", 0, 5); points.Add(x);
            Point y = new Point("Y", 0, -4); points.Add(y);
            Point z = new Point("Z", 0, -5); points.Add(z);


            List<Point> pnts = new List<Point>();
            pnts.Add(x);
            pnts.Add(o);
            pnts.Add(y);
            pnts.Add(z);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(y);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 5.0);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Intersection inter = (Intersection)parser.Get(new Intersection(y, (Segment)parser.Get(new Segment(x, z)), (Segment)parser.Get(new Segment(a, b))));
            given.Add(new Strengthened(inter, new Perpendicular(inter)));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, y)), (Segment)parser.Get(new Segment(b, y))));
        }
    }
}