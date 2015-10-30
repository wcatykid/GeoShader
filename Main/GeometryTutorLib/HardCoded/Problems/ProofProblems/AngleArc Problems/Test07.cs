using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Test07 : CirclesProblem
    {
        //Demonstrates: Measure of an angle formed by two chords that intersect inside a circle is equal to half the sum of the measures
        //of the intercepted arcs
        //To see use of theorem, need to turn off VERTICAL_ANGLES and RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT in JustificationSwitch

        public Test07(bool onoff, bool complete)
            : base(onoff, complete)
        {
            //Circle
            Point o = new Point("O", 0, 0); points.Add(o);
            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);

            //Points for chord ab
            Point a = new Point("A", -3, 4); points.Add(a);
            Point b = new Point("B", 3, -4); points.Add(b);

            //Points for chord cd
            Point c = new Point("C", -3, -4); points.Add(c);
            Point d = new Point("D", 1, System.Math.Sqrt(24)); points.Add(d);

            //Find intersection point of ab and cd
            Segment ab = new Segment(a, b);
            Segment cd = new Segment(c, d);
            Point inter = ab.FindIntersection(cd);
            Point z = new Point("Z", inter.X, inter.Y); points.Add(z);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(z);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(z);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, z, d)), (Angle)parser.Get(new Angle(c, z, b))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, z, c)), (Angle)parser.Get(new Angle(b, z, d))));


        }
    }
}