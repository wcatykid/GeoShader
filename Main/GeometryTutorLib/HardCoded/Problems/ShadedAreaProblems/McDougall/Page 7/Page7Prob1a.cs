using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob1a : ActualShadedAreaProblem
    {
        public Page7Prob1a(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 12); points.Add(b);
            Point c = new Point("C", 18, 12); points.Add(c);
            Point d = new Point("D", 18, 0); points.Add(d);
            Point p = new Point("P", 0, -18); points.Add(p);


            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(a);
            pnts.Add(p);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(a, 18));
            circles.Add(new Circle(b, 6));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a,p)), 18);
            known.AddSegmentLength(cd, 12);

            Angle a1 = (Angle)parser.Get(new Angle(a, b, c));
            Angle a2 = (Angle)parser.Get(new Angle(b, c, d));
            Angle a3 = (Angle)parser.Get(new Angle(c, d, a));
            Angle a4 = (Angle)parser.Get(new Angle(d, a, b));
            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -1, 10));
            wanted.Add(new Point("", -6, 0));
            wanted.Add(new Point("", 1, -1));
            wanted.Add(new Point("", 10, -0.5));
            wanted.Add(new Point("", 10, -8.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(243 * System.Math.PI);

            problemName = "McDougall Page 7 Problem 1a";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}