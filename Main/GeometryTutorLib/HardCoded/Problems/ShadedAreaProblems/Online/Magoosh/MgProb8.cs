using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MgProb8 : ActualShadedAreaProblem
    {
        public MgProb8(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double y = 4 * System.Math.Sqrt(3);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 4, 4); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);
            Point m = new Point("M", 2, 2); points.Add(m);

            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(m);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            Circle circle1 = new Circle(m, System.Math.Sqrt(32)/2.0);
            Circle circle2 = new Circle(c, 4);
            circles.Add(circle1);
            circles.Add(circle2);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle a1 = (Angle)parser.Get(new Angle(a, c, b));

            given.Add(new Strengthened(a1, new RightAngle(a1)));

            known.AddSegmentLength(ac, 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(8);

            problemName = "Magoosh Problem 8";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


