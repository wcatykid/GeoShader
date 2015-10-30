using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Prob19 : ActualShadedAreaProblem
    {
        public Page2Prob19(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -6, 0); points.Add(a);
            Point b = new Point("B", 6, 0); points.Add(b);
            Point p = new Point("P", -3, 6.0 * System.Math.Sqrt(3) / 2.0); points.Add(p);

            Segment pa = new Segment(p, a); segments.Add(pa);
            Segment pb = new Segment(p, b); segments.Add(pb);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 6));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricAngleEquation((Angle)parser.Get(new Angle(o, a, p)), new NumericValue(60.0)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, b)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, 4));
            wanted.Add(new Point("", -5.19, 2.9));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(25.37175323);

            problemName = "Jurgensen Page 2 Problem 19";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}