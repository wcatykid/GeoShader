using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page5Row3Prob3 : ActualShadedAreaProblem
    {
        public Page5Row3Prob3(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 6.5, 0); points.Add(b);
            Point c = new Point("C", 5, -4); points.Add(c);
            Point d = new Point("D", 2, -4); points.Add(d);
            Point p = new Point("P", 5, 0); points.Add(p);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment pc = new Segment(p, c); segments.Add(pc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(cd, (Segment)parser.Get(new Segment(a, b))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, p, c))));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 6.5);
            known.AddSegmentLength(cd, 3);
            known.AddSegmentLength(pc, 4);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(19);

            problemName = "McDougall Page 5 Row 3 Problem 3";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}