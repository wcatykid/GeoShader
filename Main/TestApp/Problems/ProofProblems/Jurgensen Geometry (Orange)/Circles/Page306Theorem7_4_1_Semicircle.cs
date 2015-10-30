using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    class Page306Theorem7_4_1_Semicircle : CirclesProblem
    {
        //Demonstrates: congruent chords have congruent arcs

        public Page306Theorem7_4_1_Semicircle(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point c = new Point("C", 15, 0); points.Add(c);

            Point s = new Point("S", -3, -4); points.Add(s);
            Point t = new Point("T", 3, 4); points.Add(t);
            Point u = new Point("U", 2, Math.Sqrt(21)); points.Add(u);

            Point a = new Point("A", 12, 4); points.Add(a);
            Point b = new Point("B", 18, -4); points.Add(b);
            Point d = new Point("D", 16, -Math.Sqrt(24)); points.Add(d);

            Segment st = new Segment(s, t); segments.Add(st);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ou = new Segment(o, u); segments.Add(ou);
            Segment cd = new Segment(c, d); segments.Add(cd);

            Circle c1 = new Circle(o, 5.0);
            Circle c2 = new Circle(c, 5.0);
            circles.Add(c1);
            circles.Add(c2);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(st, ab));

            Semicircle semi1 = (Semicircle)parser.Get(new Semicircle(c1, s, t, u, st));
            Semicircle semi2 = (Semicircle)parser.Get(new Semicircle(c2, a, b, d,ab));
            goals.Add(new GeometricCongruentArcs(semi1, semi2));
        }
    }
}
