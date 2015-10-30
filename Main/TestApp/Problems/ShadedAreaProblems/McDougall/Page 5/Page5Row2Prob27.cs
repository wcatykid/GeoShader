using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page5Row2Prob27: ActualShadedAreaProblem
    {
        public Page5Row2Prob27(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 1.5, 2); points.Add(b);
            Point c = new Point("C", 8.5, 2); points.Add(c);
            Point d = new Point("D", 10, 0); points.Add(d);
            Point e = new Point("E", 10, -5); points.Add(e);
            Point f = new Point("F", 0, -5); points.Add(f);
            Point x = new Point("X", 1.5, 0); points.Add(x);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fa = new Segment(f, a); segments.Add(fa);
            Segment bx = new Segment(b, x); segments.Add(bx);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(bc, (Segment)parser.Get(new Segment(a, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, d, e))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, e, f))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(e, f, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(f, a, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, x, b))));

            known.AddSegmentLength(bc, 7);
            known.AddSegmentLength(bx, 2);
            known.AddSegmentLength(ef, 10);
            known.AddSegmentLength(fa, 5);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(67);

            problemName = "McDougall Page 5 Row 2 Problem 27";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}