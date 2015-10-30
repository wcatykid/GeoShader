using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob4 : ActualShadedAreaProblem
    {
        public Page7Prob4(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 8, 5); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            Point e = new Point("E", 3, 0); points.Add(e);
            Point f = new Point("F", 3, 5); points.Add(f);
            Point g = new Point("G", 5, 5); points.Add(g);
            Point h = new Point("H", 5, 0); points.Add(h);
            Point i = new Point("I", 5, 3); points.Add(i);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment fe = new Segment(f, e); segments.Add(fe);
            Segment fi = new Segment(f, i); segments.Add(fi);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(f);
            pnts.Add(g);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(e);
            pnts.Add(h);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(g);
            pnts.Add(i);
            pnts.Add(h);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 8);
            known.AddSegmentLength(cd, 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(i, h)), 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, h)), 2);

            Angle a1 = (Angle)parser.Get(new Angle(d, a, b));
            Angle a2 = (Angle)parser.Get(new Angle(a, b, c));
            Angle a3 = (Angle)parser.Get(new Angle(b, c, d));
            Angle a4 = (Angle)parser.Get(new Angle(c, d, h));
            Angle a5 = (Angle)parser.Get(new Angle(d, h, i));
            Angle a6 = (Angle)parser.Get(new Angle(a, e, f));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new Strengthened(a5, new RightAngle(a5)));
            given.Add(new Strengthened(a6, new RightAngle(a6)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", 4.5, 4));
            wanted.Add(new Point("", 6, 1));
            wanted.Add(new Point("", 6, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(32);

            problemName = "Glencoe Page 7 Problem 4";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
