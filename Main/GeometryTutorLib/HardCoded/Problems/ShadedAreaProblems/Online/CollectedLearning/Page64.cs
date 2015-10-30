using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page64 : ActualShadedAreaProblem
    {
        public Page64(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 6, 4); points.Add(b);
            Point c = new Point("C", 14, 4); points.Add(c);
            Point d = new Point("D", 20, 0); points.Add(d);
            Point o = new Point("O", 10, 4); points.Add(o);
            Point p = new Point("P", 10, 0); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment op = new Segment(o, p); segments.Add(op);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(o);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(p);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 4);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ad = (Segment)parser.Get(new Segment(a, d));
            Angle a1 = (Angle)parser.Get(new Angle(b, o, p));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(p, circle, ad));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));

            known.AddSegmentLength(ad, 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 5, 1));
            wanted.Add(new Point("", 15, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(56 - 8 * System.Math.PI);

            problemName = "Collected Learning Page 64";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

