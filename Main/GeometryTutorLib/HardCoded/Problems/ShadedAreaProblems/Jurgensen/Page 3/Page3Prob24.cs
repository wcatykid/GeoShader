using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page3Prob24 : ActualShadedAreaProblem
    {
        public Page3Prob24(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 2, 0); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            Point o = new Point("O", 4, 4); points.Add(o);
            Point e = new Point("E", 0, 4); points.Add(e);
            Point f = new Point("F", 2, 4); points.Add(f);
            Point g = new Point("G", 6, 4); points.Add(g);
            Point h = new Point("H", 8, 4); points.Add(h);

            Point x = new Point("X", 4, 0); points.Add(x);

            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment bf = new Segment(b, f); segments.Add(bf);
            Segment cg = new Segment(c, g); segments.Add(cg);
            Segment dh = new Segment(d, h); segments.Add(dh);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(o);
            pts.Add(g);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(x);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2));
            circles.Add(new Circle(o, 4));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ae, dh, (Segment)parser.Get(new Segment(e, h)),
                                                                                     (Segment)parser.Get(new Segment(a, d))));
            given.Add(new Strengthened(quad, new Rectangle(quad)));

            known.AddSegmentLength(bf, 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, h)), 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, f)), 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 7));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);
            goalRegions.AddRange(parser.implied.GetAtomicRegionsByFigure(new Quadrilateral(new Segment(g, c), new Segment(h, d), new Segment(g, h), new Segment(c, d))));
            goalRegions.AddRange(parser.implied.GetAtomicRegionsByFigure(new Quadrilateral(new Segment(a, e), new Segment(f, b), new Segment(f, e), new Segment(b, a))));

            SetSolutionArea(4 * (4 + (3.0 / 2.0) * System.Math.PI));

            problemName = "Jurgensen Page 3 Problem 24";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}