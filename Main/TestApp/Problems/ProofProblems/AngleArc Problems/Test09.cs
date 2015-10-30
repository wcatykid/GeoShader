using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Test09 : CirclesProblem
    {
        //Demonstrates: Minor Arcs Congruent if central angles congruent

        public Test09(bool onoff, bool complete)
            : base(onoff, complete)
        {

            //Circles
            Point o = new Point("O", 0, 0); points.Add(o);
            Point c = new Point("C", 12, 0); points.Add(c);
            Circle circleO = new Circle(o, 5.0);
            Circle circleC = new Circle(c, 5.0);
            circles.Add(circleO);
            circles.Add(circleC);

            //Vertices for angle TOV, a central angle of circle O, with one endpoint at 90 degrees and another at 30 degrees
            Point t = new Point("T", 0, 5); points.Add(t);
            double unitX = Math.Sqrt(3) / 2;
            double unitY = 0.5;
            Point unitPnt = new Point("", unitX, unitY);
            Point v, trash;
            circleO.FindIntersection(new Segment(o, unitPnt), out v, out trash);
            if (v.X < 0) v = trash;
            v = new Point("V", v.X, v.Y); points.Add(v);

            //Vertices for angle ACB, a central angle of circle C, with one endpoint at 90 degrees and another at 30 degrees
            Point a = new Point("A", 12, 5); points.Add(a);
            Point b = new Point("B", 12 + v.X, v.Y); points.Add(b);

            Segment ot = new Segment(o, t); segments.Add(ot);
            Segment ov = new Segment(o, v); segments.Add(ov);
            Segment tv = new Segment(t, v); segments.Add(tv);
            Segment ca = new Segment(c, a); segments.Add(ca);
            Segment cb = new Segment(c, b); segments.Add(cb);
            Segment ab = new Segment(a, b); segments.Add(ab);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentCircles(circleO, circleC));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(o, t, v)), (Angle)parser.Get(new Angle(c, a, b))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(o, v, t)), (Angle)parser.Get(new Angle(c, b, a))));

            goals.Add(new GeometricCongruentArcs((MinorArc)parser.Get(new MinorArc(circleO, t, v)), (MinorArc)parser.Get(new MinorArc(circleC, a, b))));

        }
    }
}
