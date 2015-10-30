using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Prob15 : ActualShadedAreaProblem
    {
        public Page8Prob15(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 10, 5); points.Add(c);
            Point d = new Point("D", 10, 0); points.Add(d);
            Point o = new Point("O", 2.5, 2.5); points.Add(o);
            Point p = new Point("P", 2.5, 5); points.Add(p);
            Point q = new Point("Q", 2.5, 0); points.Add(q);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(p);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(q);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(p);
            pnts.Add(o);
            pnts.Add(q);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 2.5);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle rA = (Angle)parser.Get(new Angle(a, d, c));
            Segment bc = (Segment)parser.Get(new Segment(b, c));
            Segment ad = (Segment)parser.Get(new Segment(a, d));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(p, circle, bc));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(q, circle, ad));

            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 5);
            known.AddSegmentLength(ad, 10);
            known.AddAngleMeasureDegree(rA, 90);

            given.Add(new Strengthened(rA, new RightAngle(rA)));
            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));

            //Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, ad));
            //given.Add(new Strengthened(quad, new Rectangle(quad)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0.5, 4.5));
            wanted.Add(new Point("", 0.1, 0.1));
            wanted.Add(new Point("", 7, 3));
            wanted.Add(new Point("", 7, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(50 - 6.25 * System.Math.PI);

            problemName = "Glencoe Page 8 Problem 15";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}