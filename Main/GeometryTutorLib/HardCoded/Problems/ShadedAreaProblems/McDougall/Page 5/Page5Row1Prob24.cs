using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row1Prob24 : ActualShadedAreaProblem
    {
        public Page5Row1Prob24(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 16, 12); points.Add(b);
            Point c = new Point("C", 46, 0); points.Add(c);
            Point d = new Point("D", 16, -12); points.Add(d);
            Point o = new Point("O", 16, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(ab, ad));
            given.Add(new GeometricCongruentSegments(bc, cd));

            known.AddSegmentLength(ab, 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, o)), 16);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, c)), 30);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(552);

            problemName = "McDougall Page 5 Row 1 Problem 24";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}