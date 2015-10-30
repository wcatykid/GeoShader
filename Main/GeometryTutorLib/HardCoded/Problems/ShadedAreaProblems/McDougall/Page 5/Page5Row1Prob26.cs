using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row1Prob26 : ActualShadedAreaProblem
    {
        public Page5Row1Prob26(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 21, 0); points.Add(b);
            Point c = new Point("C", 42, -20); points.Add(c);
            Point d = new Point("D", 0, -20); points.Add(d);
            Point e = new Point("E", 21, -20); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment be = new Segment(b, e); segments.Add(be);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(e);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, a, b))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, d, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, e, c))));

            known.AddSegmentLength(ab, 21);
            known.AddSegmentLength(bc, 29);
            known.AddSegmentLength(ad, 20);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(630);

            problemName = "McDougall Page 5 Row 1 Problem 26";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}