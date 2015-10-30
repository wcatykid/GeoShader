using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MgGeoPracticeProb5 : ActualShadedAreaProblem
    {
        public MgGeoPracticeProb5(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point f = new Point("F", 0, 0); points.Add(f);
            Point e = new Point("E", (144 / 13.0), (60 / 13.0)); points.Add(e);
            Point d = new Point("D", 13, 0); points.Add(d);
            Point q = new Point("Q", 6.5, 0); points.Add(q);
            Point g = new Point("G", -39, 0); points.Add(g);
            double x = -432/13.0;
            Point h = new Point("H", x, -2.4 * x - 93.6); points.Add(h);

            Segment ed = new Segment(e, d); segments.Add(ed);
            Segment gh = new Segment(g, h); segments.Add(gh);

            List<Point> pnts = new List<Point>();
            pnts.Add(g);
            pnts.Add(f);
            pnts.Add(q);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(e);
            pnts.Add(f);
            pnts.Add(h);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(q, 6.5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(ed, gh));

            known.AddSegmentLength((Segment)parser.Get(new Segment(f, d)), 13);
            known.AddSegmentLength(gh, 15);
            known.AddSegmentLength(ed, 5);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -20, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(270);

            problemName = "Magoosh Geometry Practice Problem 5";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}



