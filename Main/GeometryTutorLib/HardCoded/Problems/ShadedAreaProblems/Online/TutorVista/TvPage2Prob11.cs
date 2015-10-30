using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage2Prob11 : ActualShadedAreaProblem
    {
        public TvPage2Prob11(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = System.Math.Cos(4 * System.Math.PI / 9.0);
            double y = System.Math.Sin(4 * System.Math.PI / 9.0);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", -120 * x, 120 * y); points.Add(b);
            Point c = new Point("C", -180 * x, 180 * y); points.Add(c);
            Point d = new Point("D", 120 * x, 120 * y); points.Add(d);
            Point e = new Point("E", 180 * x, 180 * y); points.Add(e);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(b);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(d);
            pnts.Add(e);
            collinear.Add(new Collinear(pnts));

            Circle small = new Circle(a, 120);
            Circle big = new Circle(a, 180);
            circles.Add(small);
            circles.Add(big);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, d)), 120);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, e)), 180);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(c, a, e)), 20);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 9, 130));
            wanted.Add(new Point("", 0, 180 * y + 0.1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(1000 * System.Math.PI);

            problemName = "Tutor Vista Page 2 Problem 11";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

