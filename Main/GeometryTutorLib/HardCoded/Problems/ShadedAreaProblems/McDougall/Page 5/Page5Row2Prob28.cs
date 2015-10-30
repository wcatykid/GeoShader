using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row2Prob28 : ActualShadedAreaProblem
    {
        public Page5Row2Prob28(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 4, 3); points.Add(b);
            Point c = new Point("C", 8, 3); points.Add(c);
            Point d = new Point("D", 12, 0); points.Add(d);
            Point e = new Point("E", 8, 0); points.Add(e);
            Point f = new Point("F", 4, -3); points.Add(f);
            Point p = new Point("P", 4, 0); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fa = new Segment(f, a); segments.Add(fa);
            Segment be = new Segment(b, e); segments.Add(be);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, p, e))));

            given.Add(new GeometricCongruentSegments(ab, be));
            given.Add(new GeometricCongruentSegments(ab, ef));
            given.Add(new GeometricCongruentSegments(ab, fa));
            given.Add(new GeometricCongruentSegments(ab, cd));
            given.Add(new GeometricCongruentSegments(bc, (Segment)parser.Get(new Segment(a, p))));
            given.Add(new GeometricCongruentSegments(bc, (Segment)parser.Get(new Segment(p, e))));
            given.Add(new GeometricCongruentSegments(bc, (Segment)parser.Get(new Segment(d, e))));

            known.AddSegmentLength(cd, 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(d, e)), 4);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(36);

            problemName = "McDougall Page 5 Row 2 Problem 28";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}