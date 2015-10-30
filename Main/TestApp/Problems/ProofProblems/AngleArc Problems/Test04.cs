using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Test04 : CirclesProblem
    {
        //Demonstrates: ExteriorAngleHalfDifferenceInterceptedArcs : two secants

        public Test04(bool onoff, bool complete)
            : base(onoff, complete)
        {
            //Circle
            Point o = new Point("O", 0, 0); points.Add(o);
            Circle circleO = new Circle(o, 5.0);
            circles.Add(circleO);
            
            //Intersection point for two secants
            Point x = new Point("X", 0, 6); points.Add(x);

            //Secant intersection points for circle O
            Point a = new Point("A", -3, -4); points.Add(a);
            Point b = new Point("B", 3, -4); points.Add(b);
            Point c, d, trash;
            circleO.FindIntersection(new Segment(b, x), out c, out trash);
            if (b.StructurallyEquals(c)) c = trash;
            c = new Point("C", c.X, c.Y);
            points.Add(c);
            circleO.FindIntersection(new Segment(a, x), out d, out trash);
            if (a.StructurallyEquals(d)) d = trash;
            d = new Point("D", d.X, d.Y);
            points.Add(d);

            //Create point for another arc (Arc(CE)) of equal measure to (1/2)*(Arc(AB)-Arc(CD))
            Point e = new Point("E", 3, 4); points.Add(e);

            //Should now be able to form segments for a central angle of equal measure to (1/2)*(Arc(AB)-Arc(CD))
            Segment oc = new Segment(o, c); segments.Add(oc);
            Segment oe = new Segment(o, e); segments.Add(oe);

            //Label the intersection betweeen OE and BX
            Point i = oe.FindIntersection(new Segment(b, x));
            i = new Point("I", i.X, i.Y); points.Add(i);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(d);
            pnts.Add(x);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(i);
            pnts.Add(c);
            pnts.Add(x);
            collinear.Add(new Collinear(pnts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            MinorArc farMinor1 = (MinorArc)parser.Get(new MinorArc(circleO, a, b));
            MinorArc closeMinor1 = (MinorArc)parser.Get(new MinorArc(circleO, c, d));
            MinorArc centralAngleArc = (MinorArc)parser.Get(new MinorArc(circleO, c, e));
            given.Add(new GeometricArcEquation(new Multiplication(new NumericValue(2), centralAngleArc), new Subtraction(farMinor1, closeMinor1)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, x, b)), (Angle)parser.Get(new Angle(c, o, e))));


        }
    }
}
