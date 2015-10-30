using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row2Prob29 : ActualShadedAreaProblem
    {
        public Page6Row2Prob29(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 17); points.Add(a);
            Point b = new Point("B", 0, 34); points.Add(b);
            Point c = new Point("C", 0, -34); points.Add(c);
            //Point d = new Point("D", 0, -17); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            Circle outer = new Circle(o, 34);
            Circle inner = new Circle(o, 17);
            circles.Add(outer);
            circles.Add(inner);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 17);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(b, o, c)), 180);
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(a, o))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 19, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(433.5 * System.Math.PI);

            problemName = "McDougall Page 6 Row 2 Problem 29";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}