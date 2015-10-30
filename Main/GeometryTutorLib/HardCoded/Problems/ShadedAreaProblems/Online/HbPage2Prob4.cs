using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class HbPage2Prob4 : ActualShadedAreaProblem
    {
        public HbPage2Prob4(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 10); points.Add(b);
            Point c = new Point("C", 10, 10); points.Add(c);
            Point d = new Point("D", 10, 0); points.Add(d);
            Point o = new Point("O", 5, 5); points.Add(o);
            Point e = new Point("E", 0, 5); points.Add(e);
            Point f = new Point("F", 10, 5); points.Add(f);

            //Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            //Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(o);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(e);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(f);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 5);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ab = (Segment)parser.Get(new Segment(a, b));
            Segment cd = (Segment)parser.Get(new Segment(c, d));
            Angle a1 = (Angle)parser.Get(new Angle(b, c, d));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(e, circle, ab));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(f, circle, cd));

            given.Add(new GeometricCongruentSegments(ab, bc));
            given.Add(new GeometricCongruentSegments(ab, cd));
            given.Add(new GeometricCongruentSegments(ab, da));
            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));

            known.AddSegmentLength(ab, 10);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 9));
            wanted.Add(new Point("", 8, 9));
            wanted.Add(new Point("", 2, 1));
            wanted.Add(new Point("", 8, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(50 - 12.5 * System.Math.PI);

            problemName = "Hatboro Page 2 Problem 4";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
