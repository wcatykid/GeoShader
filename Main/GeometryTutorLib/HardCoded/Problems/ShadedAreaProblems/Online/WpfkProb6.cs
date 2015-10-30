using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class WpfkProb6 : ActualShadedAreaProblem
    {
        public WpfkProb6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 100, 0); points.Add(b);
            Point c = new Point("C", 50, 0); points.Add(c);
            Point d = new Point("D", 0, 40); points.Add(d);
            Point e = new Point("E", 100, 40); points.Add(e);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment eb = new Segment(e, b); segments.Add(eb);
            Segment dc = new Segment(d, c); segments.Add(dc);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(c);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ad, eb, de, (Segment)parser.Get(new Segment(a, b))));
            given.Add(new Strengthened(quad, new Rectangle(quad)));
            Intersection inter = (Intersection)parser.Get(new Intersection(c, (Segment)parser.Get(new Segment(a, b)), dc));
            given.Add(new SegmentBisector(inter, dc));

            known.AddSegmentLength(eb, 40);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 100);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 10));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(1000);

            problemName = "Word Problems For Kids - Grade 11 Prob 6";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

