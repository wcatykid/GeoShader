using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob35 : ActualShadedAreaProblem
    {
        public Page10Prob35(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 16, 35); points.Add(b);
            Point c = new Point("C", 44, 20); points.Add(c);
            Point d = new Point("D", 44, 0); points.Add(d);
            Point e = new Point("E", 16, 0); points.Add(e);
            Point f = new Point("F", 16, 20); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment fc = new Segment(f, c); segments.Add(fc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(f);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(b, f)), 15);
            known.AddSegmentLength((Segment)parser.Get(new Segment(f, e)), 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, e)), 16);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, d)), 28);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, f, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(f, e, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(f, c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, d, e))));

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(1050);

            problemName = "Glencoe Page 10 Problem 35";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}