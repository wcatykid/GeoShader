using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob27 : ActualShadedAreaProblem
    {
        public Page7Prob27(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 7, 5); points.Add(c);
            Point d = new Point("D", 7, 10); points.Add(d);
            Point e = new Point("E", 0, 10); points.Add(e);
            Point f = new Point("F", 0, 15); points.Add(f);
            Point g = new Point("G", 10, 15); points.Add(g);
            Point h = new Point("H", 10, 10); points.Add(h);
            Point i = new Point("I", 17, 10); points.Add(i);
            Point j = new Point("J", 17, 15); points.Add(j);
            Point k = new Point("K", 17, 5); points.Add(k);
            Point l = new Point("L", 10, 5); points.Add(l);
            Point m = new Point("M", 10, 0); points.Add(m);
            Point n = new Point("N", 17, 0); points.Add(n);


            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);

            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment hi = new Segment(h, i); segments.Add(hi);

            Segment kl = new Segment(k, l); segments.Add(kl);
            Segment lm = new Segment(l, m); segments.Add(lm);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(b);
            pnts.Add(e);
            pnts.Add(f);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(f);
            pnts.Add(g);
            pnts.Add(j);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(j);
            pnts.Add(i);
            pnts.Add(k);
            pnts.Add(n);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(n);
            pnts.Add(m);
            pnts.Add(a);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 5);
            known.AddSegmentLength(bc, 7);
            known.AddSegmentLength(cd, 5);
            known.AddSegmentLength(de, 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, f)), 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(f, g)), 10);
            known.AddSegmentLength(gh, 5);
            known.AddSegmentLength(hi, 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(i, k)), 5);
            known.AddSegmentLength(kl, 7);
            known.AddSegmentLength(lm, 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(m, a)), 10);

            Angle a1 = (Angle)parser.Get(new Angle(a, b, c));
            Angle a2 = (Angle)parser.Get(new Angle(b, c, d));
            Angle a3 = (Angle)parser.Get(new Angle(c, d, e));
            Angle a4 = (Angle)parser.Get(new Angle(d, e, f));
            Angle a5 = (Angle)parser.Get(new Angle(e, f, g));
            Angle a6 = (Angle)parser.Get(new Angle(f, g, h));
            Angle a7 = (Angle)parser.Get(new Angle(g, h, i));
            Angle a8 = (Angle)parser.Get(new Angle(h, i, n));
            Angle a9 = (Angle)parser.Get(new Angle(i, k, l));
            Angle a10 = (Angle)parser.Get(new Angle(k, l, m));
            Angle a11 = (Angle)parser.Get(new Angle(l, m, a));
            Angle a12 = (Angle)parser.Get(new Angle(m, a, b));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new Strengthened(a5, new RightAngle(a5)));
            given.Add(new Strengthened(a6, new RightAngle(a6)));
            given.Add(new Strengthened(a7, new RightAngle(a7)));
            given.Add(new Strengthened(a8, new RightAngle(a8)));
            given.Add(new Strengthened(a9, new RightAngle(a9)));
            given.Add(new Strengthened(a10, new RightAngle(a10)));
            given.Add(new Strengthened(a11, new RightAngle(a11)));
            given.Add(new Strengthened(a12, new RightAngle(a12)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 3, 6));
            unwanted.Add(new Point("", 12, 13));
            unwanted.Add(new Point("", 12, 2));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(150);

            problemName = "Glencoe Page 7 Problem 27";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}