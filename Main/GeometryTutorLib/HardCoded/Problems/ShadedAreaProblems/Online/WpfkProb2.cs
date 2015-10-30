using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class WpfkProb2 : ActualShadedAreaProblem
    {
        public WpfkProb2(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double r = System.Math.Sqrt(13);

            Point a = new Point("A", 0, r); points.Add(a);
            Point b = new Point("B", 0, 2); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 3, 0); points.Add(d);
            Point e = new Point("E", r, 0); points.Add(e);
            Point f = new Point("F", 3, 2); points.Add(f);

            Segment bf = new Segment(b, f); segments.Add(bf);
            Segment fd = new Segment(f, d); segments.Add(fd);
            Segment cf = new Segment(c, f); segments.Add(cf);

            List<Point> pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(b);
            pnts.Add(a);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(d);
            pnts.Add(e);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(c, r));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(
                (Segment)parser.Get(new Segment(b, c)), fd, 
                bf, (Segment)parser.Get(new Segment(c, d))));
            given.Add(new Strengthened(quad, new Rectangle(quad)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 2.5));
            wanted.Add(new Point("", 3.2, 0.2));
            wanted.Add(new Point("", 2, ((2-r)/3.0)*2 + r + 0.1));
            wanted.Add(new Point("", 3.3, 1.43));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(3.25 * System.Math.PI - 6);

            problemName = "Word Problems For Kids - Grade 11 Prob 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
