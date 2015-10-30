using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    class Page306Theorem7_4_1 : CirclesProblem
    {
        //Demonstrates: congruent chords have congruent arcs

        public Page306Theorem7_4_1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point r = new Point("R", -3, 4); points.Add(r);
            Point s = new Point("S", 2, Math.Sqrt(21)); points.Add(s);
            Point t = new Point("T", 2, -Math.Sqrt(21)); points.Add(t);
            Point u = new Point("U", -3, -4); points.Add(u);

            Segment rt = new Segment(r, t);
            Segment su = new Segment(s, u);
            Point v = rt.FindIntersection(su); points.Add(v);

            Segment rs = new Segment(r, s); segments.Add(rs);
            Segment ut = new Segment(u, t); segments.Add(ut);

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(rs, ut));

            MinorArc a1 = (MinorArc)parser.Get(new MinorArc(c, r, s));
            MinorArc a2 = (MinorArc)parser.Get(new MinorArc(c, t, u));
            MajorArc ma1 = (MajorArc)parser.Get(new MajorArc(c, r, s));
            MajorArc ma2 = (MajorArc)parser.Get(new MajorArc(c, t, u));

            goals.Add(new GeometricCongruentArcs(a1, a2));
            goals.Add(new GeometricCongruentArcs(ma1, ma2));
        }
    }
}
