using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage1Prob7 : ActualShadedAreaProblem
    {
        public TvPage1Prob7(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 2); points.Add(b);
            Point c = new Point("C", 4, 2); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point e = new Point("E", 4, 8); points.Add(e);
            Point f = new Point("F", 16, 8); points.Add(f);
            Point g = new Point("G", 16, 0); points.Add(g);
            Point h = new Point("H", 8, 0); points.Add(h);
            Point i = new Point("I", 2, 0); points.Add(i);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);

            List<Point> pnts = new List<Point>();
            pnts.Add(e);
            pnts.Add(c);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(i);
            pnts.Add(d);
            pnts.Add(h);
            pnts.Add(g);
            collinear.Add(new Collinear(pnts));

            Circle small = new Circle(d, 2);
            Circle big = new Circle(g, 8);
            circles.Add(small);
            circles.Add(big);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad1 = (Quadrilateral)parser.Get(new Quadrilateral(ab, (Segment)parser.Get(new Segment(c, d)),
                bc, (Segment)parser.Get(new Segment(a, d))));
            Quadrilateral quad2 = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(e, d)), fg,
                ef, (Segment)parser.Get(new Segment(d, g))));

            given.Add(new Strengthened(quad1, new Rectangle(quad1)));
            given.Add(new Strengthened(quad2, new Rectangle(quad2)));

            known.AddSegmentLength(ab, 2);
            known.AddSegmentLength(bc, 4);
            known.AddSegmentLength(ef, 12);
            known.AddSegmentLength(fg, 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", 5, 1.1));
            wanted.Add(new Point("", 5, 0.3));
            wanted.Add(new Point("", 7, 1));
            wanted.Add(new Point("", 7, 3));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(104 - 17 * System.Math.PI);

            problemName = "Tutor Vista Page 1 Problem 7";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

