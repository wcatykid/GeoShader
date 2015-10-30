using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page4Prob13 : ActualShadedAreaProblem
    {
        public Page4Prob13(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -4 * System.Math.Sqrt(3), 4); points.Add(a);
            Point b = new Point("B", 0, 8); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 14, 0); points.Add(d);
            Point e = new Point("E", 10, 8); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);

            Segment be = new Segment(b, e); segments.Add(be);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment cd = new Segment(c, d); segments.Add(cd);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Triangle tri = (Triangle)parser.Get(new Triangle(a, b, c));
            given.Add(new Strengthened(tri, new EquilateralTriangle(tri)));

            Angle angle = (Angle)parser.Get(new Angle(b, c, d));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            angle = (Angle)parser.Get(new Angle(c, b, e));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            known.AddSegmentLength(ca, 8);
            known.AddSegmentLength(be, 10);
            known.AddSegmentLength(cd, 14);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(123.7128129);

            problemName = "Jurgensen Page 4 Problem 13";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}