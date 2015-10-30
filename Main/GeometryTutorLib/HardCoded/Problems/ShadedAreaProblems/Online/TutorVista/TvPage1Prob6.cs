using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage1Prob6 : ActualShadedAreaProblem
    {
        public TvPage1Prob6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 8); points.Add(b);
            Point c = new Point("C", 10, 8); points.Add(c);
            Point d = new Point("D", 10, 0); points.Add(d);
            Point o = new Point("O", 0, 2); points.Add(o);
            Point p = new Point("P", 10, 4); points.Add(p);
            Point q = new Point("Q", 0, 4); points.Add(q);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(q);
            pnts.Add(o);
            pnts.Add(a);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(p);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle small = new Circle(o, 2);
            Circle big = new Circle(p, 4);
            circles.Add(small);
            circles.Add(big);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(c, big, bc));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(d, big, da));
            CircleSegmentIntersection cInter3 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(a, small, da));

            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));
            given.Add(new Strengthened(cInter3, new Tangent(cInter3)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(b, q)), 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 8);
            known.AddSegmentLength(bc, 10);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", -0.5, 2));
            unwanted.Add(new Point("", 0.5, 2));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(80 + 6 * System.Math.PI);

            problemName = "Tutor Vista Page 1 Problem 6";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

