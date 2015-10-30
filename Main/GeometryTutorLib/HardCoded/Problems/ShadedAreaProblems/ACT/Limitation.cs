using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Limitation : ActualShadedAreaProblem
    {
        public Limitation(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 11, 0); points.Add(b);
            Point c = new Point("C", 0, 5); points.Add(c);
            Point d = new Point("D", 11, 5); points.Add(d);

            Point e = new Point("E", 5, 5); points.Add(e);
            Point f = new Point("F", 4, 0); points.Add(f);
            Point g = new Point("G", 8, 5); points.Add(g);

            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment bg = new Segment(b, g); segments.Add(bg);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(e);
            pts.Add(g);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = new Quadrilateral(ac, bd, (Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(a, b)));
            given.Add(new Strengthened(quad, new Rectangle(quad)));

            known.AddSegmentLength(ac, 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 11);

            List<Figure> tris = new List<Figure>();
            tris.Add(new Triangle(a, e, f));
            tris.Add(new Triangle(f, g, b));
            goalRegions = parser.implied.GetAtomicRegionsNotByFigures(tris);

            SetSolutionArea(27.5);

            problemName = "Limiting Problem We Cannot Calculate";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}