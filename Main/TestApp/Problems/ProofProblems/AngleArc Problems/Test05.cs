using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Test05 : CirclesProblem
    {
        //Demonstrates: ExteriorAngleHalfDifferenceInterceptedArcs : two tangents
        //Demonstrates: Tangents from point are congruent

        public Test05(bool onoff, bool complete)
            : base(onoff, complete)
        {
            //Circle
            Point o = new Point("O", 0, 0); points.Add(o);
            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);

            //Intersection point for two tangents
            Point c = new Point("C", 0, 6.25); points.Add(c);

            //Points for tangent line ac, intersection at b
            Point a = new Point("A", -8, 0.25); points.Add(a);
            Point b = new Point("B", -3, 4); points.Add(b);

            //Points for tangent line ec, intersection at d
            Point e = new Point("E", 8, 0.25); points.Add(e);
            Point d = new Point("D", 3, 4); points.Add(d);

            //Create point for another arc (Arc(DF)) of equal measure to (1/2)*(MajorArc(BD)-MinorArc(BD))
            MinorArc minor = new MinorArc(circleO, b, d);
            MajorArc major = new MajorArc(circleO, b, d);
            double measure = (major.GetMajorArcMeasureDegrees() - minor.GetMinorArcMeasureDegrees()) / 2;
            //Get theta for D and E
            Circle unit = new Circle(o, 1.0);
            Point inter1, trash;
            unit.FindIntersection(new Segment(o, d), out inter1, out trash);
            if (inter1.X < 0) inter1 = trash;
            double dThetaDegrees = (System.Math.Acos(inter1.X)) * (180 / System.Math.PI);
            double fThetaRadians = (dThetaDegrees - measure) * (System.Math.PI / 180);
            //Get coordinates for E
            Point unitPnt = new Point("", System.Math.Cos(fThetaRadians), System.Math.Sin(fThetaRadians));
            Point f;
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

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ac = (Segment)parser.Get(new Segment(a, c));
            Segment ce = (Segment)parser.Get(new Segment(c, e));
            CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(b, circleO, ac));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(d, circleO, ce));
            given.Add(new Strengthened(cInter, new Tangent(cInter)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));

            MinorArc a1 = (MinorArc)parser.Get(new MinorArc(circleO, b, d));
            MajorArc a2 = (MajorArc)parser.Get(new MajorArc(circleO, b, d));
            MinorArc centralAngleArc = (MinorArc)parser.Get(new MinorArc(circleO, d, f));
            given.Add(new GeometricArcEquation(new Multiplication(new NumericValue(2), centralAngleArc), new Subtraction(a2, a1)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, c, e)), (Angle)parser.Get(new Angle(d, o, f))));
            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(c, d))));

        }
    }
}
