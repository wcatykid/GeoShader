using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class HbPage3Prob7 : ActualShadedAreaProblem
    {
        public HbPage3Prob7(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 7); points.Add(a);
            Point b = new Point("B", 7, 7); points.Add(b);
            Point c = new Point("C", 7, 0); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            circles.Add(new Circle(d, 7.0));
            circles.Add(new Circle(b, 7.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(da, bc, ab, cd));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(ab, 7);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3.5, 3.3));
            wanted.Add(new Point("", 3.5, 3.7));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(24.5 * System.Math.PI - 49);

            problemName = "Hatboro Page 3 Problem 7";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

