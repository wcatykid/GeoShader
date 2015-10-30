using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob28 : ActualShadedAreaProblem
    {
        public Page7Prob28(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 8); points.Add(b);
            Point c = new Point("C", 11, 8); points.Add(c);
            Point d = new Point("D", 11, 0); points.Add(d);
            Point e = new Point("E", 11, 5); points.Add(e);
            Point f = new Point("F", 8, 5); points.Add(f);
            Point g = new Point("G", 8, 7); points.Add(g);
            Point h = new Point("H", 6, 7); points.Add(h);
            Point i = new Point("I", 6, 8); points.Add(i);
            Point j = new Point("J", 11, 7); points.Add(j);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment hi = new Segment(h, i); segments.Add(hi);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(i);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(j);
            pnts.Add(e);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(h);
            pnts.Add(g);
            pnts.Add(j);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 8);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b,i)), 6);
            known.AddSegmentLength(hi, 1);
            known.AddSegmentLength((Segment)parser.Get(new Segment(g, h)), 2);
            known.AddSegmentLength(fg, 2);
            known.AddSegmentLength(ef, 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, d)), 5);
            known.AddSegmentLength(ad, 11);

            Angle a1 = (Angle)parser.Get(new Angle(a, b, i));
            Angle a2 = (Angle)parser.Get(new Angle(b, i, h));
            Angle a3 = (Angle)parser.Get(new Angle(h, g, f));
            Angle a4 = (Angle)parser.Get(new Angle(f, e, d));
            Angle a5 = (Angle)parser.Get(new Angle(e, d, a));
            Angle a6 = (Angle)parser.Get(new Angle(d, a, b));
            Angle a7 = (Angle)parser.Get(new Angle(e, f, g));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new Strengthened(a5, new RightAngle(a5)));
            given.Add(new Strengthened(a6, new RightAngle(a6)));

            // Right now, problem will not work unless given this extra right angle
            given.Add(new Strengthened(a7, new RightAngle(a7)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 7, 7.5));
            unwanted.Add(new Point("", 9, 7.5));
            unwanted.Add(new Point("", 9, 6));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(77);

            problemName = "Glencoe Page 7 Problem 28";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}