using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob17 : ActualShadedAreaProblem
    {
        public Page7Prob17(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 9.2); points.Add(b);
            Point c = new Point("C", 9.2, 9.2); points.Add(c);
            Point d = new Point("D", 20, 9.2); points.Add(d);
            Point e = new Point("E", 20, 0); points.Add(e);
            Point f = new Point("F", 20, 3.1); points.Add(f);
            Point g = new Point("G", 9.2, 3.1); points.Add(g);

            Point w = new Point("W", 3, 3); points.Add(w);
            Point x = new Point("X", 3, 6.1); points.Add(x);
            Point y = new Point("Y", 6.1, 6.1); points.Add(y);
            Point z = new Point("Z", 6.1, 3); points.Add(z);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cg = new Segment(c, g); segments.Add(cg);
            Segment gf = new Segment(g, f); segments.Add(gf);
            Segment ea = new Segment(e, a); segments.Add(ea);

            Segment wx = new Segment(w, x); segments.Add(wx);
            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);
            Segment zw = new Segment(z, w); segments.Add(zw);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(c);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(d);
            pnts.Add(f);
            pnts.Add(e);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 9.2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 9.2);
            known.AddSegmentLength(gf, 10.8);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, f)), 3.1);
            known.AddSegmentLength(zw, 3.1);

            given.Add(new GeometricCongruentSegments(wx, xy));
            given.Add(new GeometricCongruentSegments(wx, yz));
            given.Add(new GeometricCongruentSegments(wx, zw));

            Angle a1 = (Angle)parser.Get(new Angle(x, w, z));
            Angle a2 = (Angle)parser.Get(new Angle(e, f, g));
            Angle a3 = (Angle)parser.Get(new Angle(g, c, b));
            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));

            Quadrilateral outer = (Quadrilateral)parser.Get(new Quadrilateral(ab, (Segment)parser.Get(new Segment(d, e)), (Segment)parser.Get(new Segment(b, d)), ea));
            given.Add(new Strengthened(outer, new Rectangle(outer)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 4, 5));
            unwanted.Add(new Point("", 10, 6));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(108.51);

            problemName = "Glencoe Page 7 Problem 17";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}