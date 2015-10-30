using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage1Prob8 : ActualShadedAreaProblem
    {
        public TvPage1Prob8(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 6); points.Add(a);
            Point b = new Point("B", 8, 6); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            Point e = new Point("E", 3.5, 2); points.Add(e);
            Point f = new Point("F", 4.5, 2); points.Add(f);
            Point g = new Point("G", 3.5, 0); points.Add(g);
            Point h = new Point("H", 4.5, 0); points.Add(h);

            Point o = new Point("O", 4, 6); points.Add(o);
            Point q = new Point("Q", 4, 2); points.Add(q);

            Segment ac = new Segment(c, a); segments.Add(ac);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment ge = new Segment(g, e); segments.Add(ge);
            Segment hf = new Segment(h, f); segments.Add(hf);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(e);
            pnts.Add(q);
            pnts.Add(f);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(g);
            pnts.Add(h);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle big = new Circle(o, 4);
            Circle small = new Circle(q, 0.5);
            circles.Add(big);
            circles.Add(small);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad1 = (Quadrilateral)parser.Get(new Quadrilateral(ac, bd, (Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));
            Quadrilateral quad2 = (Quadrilateral)parser.Get(new Quadrilateral(ge, hf, (Segment)parser.Get(new Segment(e, f)), (Segment)parser.Get(new Segment(g, h))));

            given.Add(new Strengthened(quad1, new Rectangle(quad1)));
            given.Add(new Strengthened(quad2, new Rectangle(quad2)));

            known.AddSegmentLength(ac, 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 8);
            known.AddSegmentLength((Segment)parser.Get(new Segment(g, h)), 1);
            known.AddSegmentLength(hf, 2);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 4, 0.6));
            unwanted.Add(new Point("", 4, 0.1));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(46 + 7.875 * System.Math.PI);

            problemName = "Tutor Vista Page 1 Problem 8";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

