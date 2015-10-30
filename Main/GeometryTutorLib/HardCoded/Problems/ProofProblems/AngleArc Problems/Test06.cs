using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Test06: CirclesProblem
    {
        //Demonstrates: ExteriorAngleHalfDifferenceInterceptedArcs : one tangent, one secant

        public Test06(bool onoff, bool complete)
            : base(onoff, complete)
        {
            //Circle
            Point o = new Point("O", 0, 0); points.Add(o);
            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);

            //Intersection point for tangent & secant
            Point c = new Point("C", 0, 6.25); points.Add(c);

            //Points for tangent line ac, intersection at b
            Point a = new Point("A", -8, 0.25); points.Add(a);
            Point b = new Point("B", -3, 4); points.Add(b);

            //Points for secant line ce, intersections at D & E
            Point d = new Point("D", 0, 5); points.Add(d);
            Point e = new Point("E", 0, -5); points.Add(e);

            //Create point for another arc (Arc(DF)) of equal measure to (1/2)*(MinorArc(BE)-MinorArc(BD))
            MinorArc farMinor = new MinorArc(circleO, b, e);
            MinorArc closeMinor = new MinorArc(circleO, b, d);
            double measure = (farMinor.GetMinorArcMeasureDegrees() - closeMinor.GetMinorArcMeasureDegrees()) / 2;
            //Get theta for F
            double dThetaDegrees = 90;
            double fThetaRadians = (dThetaDegrees - measure) * (System.Math.PI / 180);
            //Get coordinates for F
            Point unitPnt = new Point("", System.Math.Cos(fThetaRadians), System.Math.Sin(fThetaRadians));
            Point f, trash;
            circleO.FindIntersection(new Segment(o, unitPnt), out f, out trash);
            if (f.X < 0) f = trash;
            f = new Point("F", f.X, f.Y); points.Add(f);

            //Should now be able to form segments for a central angle of equal measure to (1/2)*(Arc(AB)-Arc(CD))
            Segment od = new Segment(o, d); segments.Add(od);
            Segment of = new Segment(o, f); segments.Add(of);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(b);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(d);
            pnts.Add(e);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ac = (Segment)parser.Get(new Segment(a, c));
            CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(b, circleO, ac));
            given.Add(new Strengthened(cInter, new Tangent(cInter)));

            MinorArc far = (MinorArc)parser.Get(new MinorArc(circleO, b, e));
            MinorArc close = (MinorArc)parser.Get(new MinorArc(circleO, b, d));
            MinorArc centralAngleArc = (MinorArc)parser.Get(new MinorArc(circleO, d, f));
            given.Add(new GeometricArcEquation(new Multiplication(new NumericValue(2), centralAngleArc), new Subtraction(far, close)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, c, e)), (Angle)parser.Get(new Angle(d, o, f))));


        }
    }
}
