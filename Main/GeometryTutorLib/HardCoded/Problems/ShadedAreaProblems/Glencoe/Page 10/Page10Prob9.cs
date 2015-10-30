using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob9 : ActualShadedAreaProblem
    {
        public Page10Prob9(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 18); points.Add(b);
            Point c = new Point("C", 39, 18); points.Add(c);
            Point d = new Point("D", 56, 0); points.Add(d);
            Point e = new Point("E", 39, 0); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ce = new Segment(c, e); segments.Add(ce);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, a, e))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, e, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, b, c))));

            known.AddSegmentLength(ab, 18);
            known.AddSegmentLength(bc, 39);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, d)), 56);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(855);

            problemName = "Glencoe Page 10 Problem 39";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}