using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page312Corollary2 : CirclesProblem
    {
        //Demonstrates: If a quad is inscribed in a circle, then its opposite angles are supplementary

        public Page312Corollary2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);

            //Points and segments for an inscribed rectangle
            Point r = new Point("R", -3, 4); points.Add(r);
            Point s = new Point("S", 3, 4); points.Add(s);
            Point t = new Point("T", 3, -4); points.Add(t);
            Point u = new Point("U", -3, -4); points.Add(u);

            Segment rs = new Segment(r, s); segments.Add(rs);
            Segment st = new Segment(s, t); segments.Add(st);
            Segment tu = new Segment(t, u); segments.Add(tu);
            Segment ur = new Segment(u, r); segments.Add(ur);

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            
            Angle angle1 = (Angle)parser.Get(new Angle(r, s, t));
            Angle angle2 = (Angle)parser.Get(new Angle(u, r, s));
            Angle angle3 = (Angle)parser.Get(new Angle(s, t, u));

            given.Add(new Strengthened(angle1, new RightAngle(angle1)));
            given.Add(new GeometricCongruentAngles(angle2, angle3));

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ur, st, rs, tu));
            goals.Add(new Strengthened(quad, new Rectangle(quad)));
        }
    }
}
