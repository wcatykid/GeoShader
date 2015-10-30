using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob10 : ActualShadedAreaProblem
    {
        public Page10Prob10(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", -3, 0); points.Add(b);
            Point c = new Point("C", 0, 22); points.Add(c);
            Point d = new Point("D", 32, 22); points.Add(d);
            Point e = new Point("E", 39, 0); points.Add(e);
            Point f = new Point("F", 32, 0); points.Add(f);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment df = new Segment(d, f); segments.Add(df);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(a);
            pts.Add(f);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, a, f))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, f, d))));
            given.Add(new GeometricCongruentSegments(ac, df));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, f)), 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 22);
            known.AddSegmentLength(cd, 32);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(814);

            problemName = "Glencoe Page 10 Problem 10";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}