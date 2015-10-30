using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row1Prob25 : ActualShadedAreaProblem
    {
        public Page5Row1Prob25(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 9, 0); points.Add(a);
            Point b = new Point("B", 20, 0); points.Add(b);
            Point c = new Point("C", 17, -12); points.Add(c);
            Point d = new Point("D", 9, -12); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment od = new Segment(o, d); segments.Add(od);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment dc = new Segment(d, c); segments.Add(dc);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(a);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(dc, (Segment)parser.Get(new Segment(o, b))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, a, b))));

            known.AddSegmentLength(od, 15);
            known.AddSegmentLength(dc, 8);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, a)), 9);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 11);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(168);

            problemName = "McDougall Page 5 Row 1 Problem 25";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}