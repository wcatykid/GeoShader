using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob16 : ActualShadedAreaProblem
    {
        public Page7Prob16(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 15); points.Add(b);
            Point c = new Point("C", 18, 15); points.Add(c);
            Point d = new Point("D", 18, 0); points.Add(d);
            Point e = new Point("E", 11, 0); points.Add(e);
            Point f = new Point("F", 11, 8); points.Add(f);
            Point g = new Point("G", 7, 8); points.Add(g);
            Point h = new Point("H", 7, 0); points.Add(h);
            Point w = new Point("W", 15, 10); points.Add(w);
            Point x = new Point("X", 15, 13); points.Add(x);
            Point y = new Point("Y", 3, 13); points.Add(y);
            Point z = new Point("Z", 3, 10); points.Add(z);


            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            //Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment gh = new Segment(g, h); segments.Add(gh);
            //Segment ha = new Segment(h, a); segments.Add(ha);
            Segment wx = new Segment(w, x); segments.Add(wx);
            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);
            Segment zw = new Segment(z, w); segments.Add(zw);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(h);
            pnts.Add(e);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(h, a)), 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(d, e)), 7);
            known.AddSegmentLength(gh, 8);
            known.AddSegmentLength(fg, 4);
            known.AddSegmentLength(cd, 15);
            known.AddSegmentLength(wx, 3);
            known.AddSegmentLength(xy, 12);

            Angle a1 = (Angle)parser.Get(new Angle(b, a, h));
            Angle a2 = (Angle)parser.Get(new Angle(g, f, e));
            Angle a3 = (Angle)parser.Get(new Angle(y, z, w));
            Angle a4 = (Angle)parser.Get(new Angle(c, d, e));
            Angle a5 = (Angle)parser.Get(new Angle(f, g, h));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new Strengthened(a5, new RightAngle(a5)));

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(yz, wx, xy, zw));
            given.Add(new Strengthened(quad, new Parallelogram(quad)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 8, 12));
            unwanted.Add(new Point("", 8, 3));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(202);

            problemName = "Glencoe Page 7 Problem 16";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}