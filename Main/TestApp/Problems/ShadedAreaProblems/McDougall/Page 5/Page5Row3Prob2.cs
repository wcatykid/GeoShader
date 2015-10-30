using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page5Row3Prob2 : ActualShadedAreaProblem
    {
        public Page5Row3Prob2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 12, 0); points.Add(c);
            Point d = new Point("D", 12, -6); points.Add(d);
            Point e = new Point("E", 0, -6); points.Add(e);
        
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ac = new Segment(a, c); segments.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(a);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, d, e))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, e, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(e, a, c))));

            known.AddSegmentLength(bc, 13);
            known.AddSegmentLength(cd, 6);
            known.AddSegmentLength(de, 12);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(102);

            problemName = "McDougall Page 5 Row 3 Problem 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}