using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row2Prob29 : ActualShadedAreaProblem
    {
        public Page5Row2Prob29(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 7 / System.Math.Sqrt(3);

            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", x, 7); points.Add(b);
            Point c = new Point("C", 12 + x, 7); points.Add(c);
            Point d = new Point("D", 12, 0); points.Add(d);
            Point e = new Point("E", x/2, 3.5); points.Add(e);
            Point f = new Point("F", 12 + (x/2), 3.5); points.Add(f);
            Point g = new Point("G", x, 0); points.Add(g);
            Point h = new Point("H", x, 3.5); points.Add(h);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment bf = new Segment(b, f); segments.Add(bf);
            Segment gf = new Segment(g, f); segments.Add(gf);
            Segment eg = new Segment(e, g); segments.Add(eg);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(f);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(h);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(h);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(g);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment bg = (Segment)parser.Get(new Segment(b, g));
            Segment ef = (Segment)parser.Get(new Segment(e, f));
            given.Add(new GeometricParallel(bc, ef));
            given.Add(new GeometricParallel(bc, (Segment)parser.Get(new Segment(a, d))));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));
            given.Add(new PerpendicularBisector((Intersection)parser.Get(new Intersection(h, bg, ef)), ef));
 

            known.AddSegmentLength(bg, 7);
            known.AddSegmentLength(bc, 12);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(36);

            problemName = "McDougall Page 5 Row 2 Problem 29";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
