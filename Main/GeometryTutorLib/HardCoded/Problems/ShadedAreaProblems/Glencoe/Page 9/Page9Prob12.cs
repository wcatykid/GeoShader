using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob12 : ActualShadedAreaProblem
    {
        public Page9Prob12(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 7.5, 10); points.Add(b);
            Point c = new Point("C", 0, 20); points.Add(c);
            Point d = new Point("D", 7.5, 20); points.Add(d);
            Point e = new Point("E", 23, 20); points.Add(e);
            Point f = new Point("F", 15.5, 10); points.Add(f);
            Point g = new Point("G", 23, 0); points.Add(g);
            Point h = new Point("H", 7.5, 0); points.Add(h);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment bf = new Segment(b, f); segments.Add(bf);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(h);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(b);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ce = (Segment)parser.Get(new Segment(c, e));
            Segment ga = (Segment)parser.Get(new Segment(g, a));
            known.AddSegmentLength(ce, 23);
            known.AddSegmentLength(ga, 23);
            known.AddSegmentLength((Segment)parser.Get(new Segment(d, b)), 10);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, h)), 10);
            known.AddSegmentLength(bf, 8);

            Quadrilateral quad1 = (Quadrilateral)parser.Get(new Quadrilateral(bc, ef, ce, bf));
            Quadrilateral quad2 = (Quadrilateral)parser.Get(new Quadrilateral(ab, fg, bf, ga));

            given.Add(new Strengthened(quad1, new Trapezoid(quad1)));
            given.Add(new Strengthened(quad2, new Trapezoid(quad2)));

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(310);

            problemName = "Glencoe Page 9 Problem 12";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}