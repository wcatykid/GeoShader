using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row3Prob4 : ActualShadedAreaProblem
    {
        public Page5Row3Prob4(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 5, 4); points.Add(b);
            Point c = new Point("C", 13, 0); points.Add(c);
            Point d = new Point("D", 5, -4); points.Add(d);
            Point o = new Point("O", 5, 0); points.Add(o);

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

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, a)), 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, d)), 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, c)), 8);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(52);

            problemName = "McDougall Page 5 Row 3 Problem 4";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}