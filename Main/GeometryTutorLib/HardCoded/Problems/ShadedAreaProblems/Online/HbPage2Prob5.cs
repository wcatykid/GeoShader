using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class HbPage2Prob5 : ActualShadedAreaProblem
    {
        public HbPage2Prob5(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -4, -3); points.Add(a);
            Point b = new Point("B", -4, 3); points.Add(b);
            Point c = new Point("C", 4, 3); points.Add(c);
            Point d = new Point("D", 4, -3); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(o);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(o, 5.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle a1 = (Angle)parser.Get(new Angle(c, d, a));
            Angle a2 = (Angle)parser.Get(new Angle(d, a, b));
            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));

            known.AddSegmentLength(bc, 8);
            known.AddSegmentLength(cd, 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, d)), 10);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 4));
            wanted.Add(new Point("", 0, -4));
            wanted.Add(new Point("", -4.5, 0));
            wanted.Add(new Point("", 4.5, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(25 * System.Math.PI - 48);

            problemName = "Hatboro Page 2 Problem 5";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
