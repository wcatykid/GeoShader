using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page89Prob8 : ActualShadedAreaProblem
    {
        public Page89Prob8(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 10); points.Add(b);
            Point c = new Point("C", 30, 10); points.Add(c);
            Point d = new Point("D", 30, 0); points.Add(d);
            Point e = new Point("E", 5, 3); points.Add(e);
            Point f = new Point("F", 25, 3); points.Add(f);
            Point g = new Point("G", 5, 10); points.Add(g);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment be = new Segment(b, e); segments.Add(be);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment eg = new Segment(e, g); segments.Add(eg);
            Segment cf = new Segment(c, f); segments.Add(cf);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(g);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment bc = (Segment)parser.Get(new Segment(b, c));
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, da));
            Quadrilateral quad2 = (Quadrilateral)parser.Get(new Quadrilateral(be, cf, bc, ef));
            given.Add(new Strengthened(quad, new Rectangle(quad)));
            given.Add(new Strengthened(quad2, new Trapezoid(quad2)));

            known.AddSegmentLength(da, 30);
            known.AddSegmentLength(ef, 20);
            known.AddSegmentLength(cd, 10);
            known.AddSegmentLength(eg, 7);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 4.5, 4));
            unwanted.Add(new Point("", 10, 4));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(125);

            problemName = "Collected Learning Page 89 Prob 8";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}



