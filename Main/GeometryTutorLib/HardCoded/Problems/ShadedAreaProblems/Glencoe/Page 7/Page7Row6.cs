using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Row6 : ActualShadedAreaProblem
    {
        public Page7Row6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 16); points.Add(b);
            Point c = new Point("C", 21, 16); points.Add(c);
            Point d = new Point("D", 21, 0); points.Add(d);
            Point e = new Point("E", 21, 7); points.Add(e);
            Point f = new Point("F", 6, 7); points.Add(f);
            Point g = new Point("G", 6, 0); points.Add(g);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);

            List<Point> pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(e);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(g);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 16);
            known.AddSegmentLength(bc, 21);
            known.AddSegmentLength(new Segment(c, e), 9);
            known.AddSegmentLength(new Segment(f, g), 7);
            known.AddSegmentLength(new Segment(g, a), 6);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, b, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, e, f))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(f, g, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(g, a, b))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", 1, 8));
            wanted.Add(new Point("", 7, 9));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(231);

            problemName = "Glencoe Page 7 Row 6";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}