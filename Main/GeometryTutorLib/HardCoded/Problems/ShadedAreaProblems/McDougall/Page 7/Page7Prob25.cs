using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob25 : ActualShadedAreaProblem
    {
        public Page7Prob25(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 0, -3); points.Add(a);
            Point b = new Point("B", 0, 3); points.Add(b);
            Point c = new Point("C", 15, 3); points.Add(c);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 3));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 6);
            known.AddSegmentLength(bc, 15);

            Angle angle = (Angle)parser.Get(new Angle(a, b, c));
            given.Add(new Strengthened(angle, new RightAngle(angle)));


            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", -2, 0));
            unwanted.Add(new Point("", 2.068, -2.173));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(45);


            problemName = "McDougall Page 7 Problem 25";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
