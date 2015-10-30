using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Test11 : CirclesProblem
    {
        //Demonstrates: If two inscribed angles intercept the same arc, the angles are congruent

        public Test11(bool onoff, bool complete)
            : base(onoff, complete)
        {
            //Circles
            Point o = new Point("O", 0, 0); points.Add(o);
            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);

            Point r = new Point("R", 0, 5); points.Add(r);
            Circle circleR = new Circle(r, 10.0);
            circles.Add(circleR);

            //Chords
            Point a = new Point("A", 0, -5); points.Add(a);
            Point d = new Point("D", -5, Math.Sqrt(75) + 5); points.Add(d);
            Point e = new Point("E", 5, Math.Sqrt(75) + 5); points.Add(e);
            Point f = new Point("F", 6, 13); points.Add(f);
            Point b, c, trash;
            circleO.FindIntersection(new Segment(a, d), out b, out trash);
            if (b.StructurallyEquals(a)) b = trash;
            circleO.FindIntersection(new Segment(a, e), out c, out trash);
            if (c.StructurallyEquals(a)) c = trash;
            b = new Point("B", b.X, b.Y); points.Add(b);
            c = new Point("C", c.X, c.Y); points.Add(c);
            Point g = (new Segment(a, e)).FindIntersection(new Segment(f, d));
            g = new Point("G", g.X, g.Y); points.Add(g);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(b);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(c);
            pnts.Add(g);
            pnts.Add(e);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(f);
            pnts.Add(g);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Segment fe = new Segment(f, e); segments.Add(fe);


            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, a, e)), (Angle)parser.Get(new Angle(d, f, e))));


        }
    }
}
