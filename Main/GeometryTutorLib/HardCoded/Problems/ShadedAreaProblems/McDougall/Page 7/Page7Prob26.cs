using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob26 : ActualShadedAreaProblem
    {
        public Page7Prob26(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 4, 4); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point o = new Point("O", 2, 2); points.Add(o);
            Point p = new Point("P", 0, 2); points.Add(p);
            Point q = new Point("Q", 4, 2); points.Add(q);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pts = new List<Point>();
            pts.Add(a); pts.Add(p); pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p); pts.Add(o); pts.Add(q);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c); pts.Add(q); pts.Add(d);
            collinear.Add(new Collinear(pts));

            Circle circle = new Circle(o, 2);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment cd = (Segment)parser.Get(new Segment(c, d));
            Angle a1 = (Angle)parser.Get(new Angle(a, b, c));
            Angle a2 = (Angle)parser.Get(new Angle(b, c, d));
            Angle a3 = (Angle)parser.Get(new Angle(c, d, a));
            Angle a4 = (Angle)parser.Get(new Angle(d, a, b));
            CircleSegmentIntersection cInter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(q, circle, cd));

            known.AddSegmentLength(da, 4);
            known.AddSegmentLength(cd, 4);

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new GeometricSegmentEquation(da, new NumericValue(4)));
            given.Add(new GeometricSegmentEquation(cd, new NumericValue(4)));
            given.Add(new Strengthened(cInter, new Tangent(cInter)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0.1, 3.8));
            unwanted.Add(new Point("", 3.9, 3.8));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(8 + 2 * System.Math.PI);

            problemName = "McDougall Page 7 Problem 24";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}