using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    class Page309Problem09 : CirclesProblem
    {
        //Demonstrates: Congruent chords have congruent arcs (and converse); arc addition axiom

        public Page309Problem09(bool onoff, bool complete) : base(onoff, complete)
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

            List<Point> pnts = new List<Point>();
            pnts.Add(r);
            pnts.Add(v);
            pnts.Add(t);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(s);
            pnts.Add(v);
            pnts.Add(u);
            collinear.Add(new Collinear(pnts));

            Circle c = new Circle(o, 5.0);
            circles.Add(c);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, s)), (Segment)parser.Get(new Segment(u, t))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(r, t)), (Segment)parser.Get(new Segment(u, s))));
        }
    }
}

