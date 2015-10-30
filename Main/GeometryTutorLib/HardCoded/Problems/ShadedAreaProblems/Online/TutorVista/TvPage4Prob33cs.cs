using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage4Prob33 : ActualShadedAreaProblem
    {
        public TvPage4Prob33(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point p = new Point("P", -12, 0); points.Add(p);
            Point a = new Point("A", -6, 0); points.Add(a);
            Point q = new Point("Q", 0, 0); points.Add(q);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 11, 0); points.Add(c);
            Point r = new Point("R", 22, 0); points.Add(r);

            List<Point> pnts = new List<Point>();
            pnts.Add(p);
            pnts.Add(a);
            pnts.Add(q);
            pnts.Add(b);
            pnts.Add(c);
            pnts.Add(r);
            collinear.Add(new Collinear(pnts));

            Circle outer = new Circle(b, 17);
            Circle left = new Circle(a, 6);
            Circle right = new Circle(c, 11);
            circles.Add(outer);
            circles.Add(left);
            circles.Add(right);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(p, a)), 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, r)), 11);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 8));
            wanted.Add(new Point("", 0, -8));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(132 * System.Math.PI);

            problemName = "Tutor Vista Page 4 Problem 33";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


