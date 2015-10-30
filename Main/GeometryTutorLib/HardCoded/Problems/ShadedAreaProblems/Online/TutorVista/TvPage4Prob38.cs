using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage4Prob38 : ActualShadedAreaProblem
    {
        public TvPage4Prob38(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", -2.3, 0); points.Add(p);
            Point q = new Point("Q", 0, 1); points.Add(q);
            Point r = new Point("R", 0, 2.3); points.Add(r);

            Segment po = new Segment(p, o); segments.Add(po);
            Segment pq = new Segment(p, q); segments.Add(pq);

            List<Point> pnts = new List<Point>();
            pnts.Add(r);
            pnts.Add(q);
            pnts.Add(o);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 2.3);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, q)), 1);
            known.AddSegmentLength(po, 2.3);

            Angle a1 = (Angle)parser.Get(new Angle(p, o, r));

            given.Add(new Strengthened(a1, new RightAngle(a1)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -2, 0.4));
            wanted.Add(new Point("", -0.5, 1.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((5.29 / 4.0) * System.Math.PI - 1.15);

            problemName = "Tutor Vista Page 4 Problem 38";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


