using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Test08 : CirclesProblem
    {
        //Demonstrates: If two inscribed angles intercept the same arc, the angles are congruent

        public Test08(bool onoff, bool complete)
            : base(onoff, complete)
        {

            //Points for chord BAC
            Point a = new Point("A", -1, -System.Math.Sqrt(24)); points.Add(a);
            Point b = new Point("B", -3, 4); points.Add(b);
            Point c = new Point("C", 2, System.Math.Sqrt(21)); points.Add(c);
            
            //Points for angle BDC
            Point d = new Point("D", 3, -4); points.Add(d);

            //Lable the intersection between BD and AC
            Segment ac = new Segment(a, c);
            Segment db = new Segment(d, b);
            Point i = ac.FindIntersection(db);
            i = new Point("I", i.X, i.Y); points.Add(i);

            //Segments for both angles
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment dc = new Segment(d, c); segments.Add(dc);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(i);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(d);
            pnts.Add(i);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            //Circle
            Point o = new Point("O", 0, 0); points.Add(o);
            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);


            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, c)), (Angle)parser.Get(new Angle(b, d, c))));


        }
    }
}
