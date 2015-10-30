using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage4Prob34 : ActualShadedAreaProblem
    {
        public TvPage4Prob34(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 0.5;
            double y = System.Math.Sqrt(3) / 2.0;
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", -2, 0); points.Add(p);
            Point q = new Point("Q", -x, y); points.Add(q);
            Point r = new Point("R", -x, -y); points.Add(r);

            Segment pq = new Segment(p, q); segments.Add(pq);
            Segment pr = new Segment(p, r); segments.Add(pr);
            Segment qo = new Segment(q, o); segments.Add(qo);
            Segment or = new Segment(o, r); segments.Add(or);
            Segment po = new Segment(p, o); segments.Add(po);

            Circle circle = new Circle(o, 1);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(qo, 1);
            known.AddSegmentLength(pq, System.Math.Sqrt(3));

            Angle a1 = (Angle)parser.Get(new Angle(p, q, o));
            Angle a2 = (Angle)parser.Get(new Angle(p, r, o));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -1.5, 0.1));
            wanted.Add(new Point("", -1.5, -0.1));
            wanted.Add(new Point("", -1, 0.1));
            wanted.Add(new Point("", -1, -0.1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(System.Math.Sqrt(3) - (System.Math.PI/6.0));

            problemName = "Tutor Vista Page 4 Problem 34";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

