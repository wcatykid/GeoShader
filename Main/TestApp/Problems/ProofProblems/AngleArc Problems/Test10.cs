using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Test10 : CirclesProblem
    {
        //Demonstrates: Minor Arcs Congruent if central angles congruent

        public Test10(bool onoff, bool complete)
            : base(onoff, complete)
        {

            //Circle center
            Point o = new Point("O", 0, 0); points.Add(o);

            //Vertices for angle TOV, a central angle of circle O, with one endpoint at 90 degrees and another at 30 degrees
            Point t = new Point("T", -3, 4); points.Add(t);
            Point v = new Point("V", 0, 5); points.Add(v);

            //Vertices for angle VOA, a central angle of circle C, with one endpoint at 90 degrees and another at 30 degrees
            Point a = new Point("A", 3, 4); points.Add(a);

            Segment ot = new Segment(o, t); segments.Add(ot);
            Segment ov = new Segment(o, v); segments.Add(ov);
            Segment oa = new Segment(o, a); segments.Add(oa);

            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentArcs((MinorArc)parser.Get(new MinorArc(circleO, t, v)), (MinorArc)parser.Get(new MinorArc(circleO, v, a))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(t, o, v)), (Angle)parser.Get(new Angle(v, o, a))));

        }
    }
}