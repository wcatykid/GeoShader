using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob9 : ActualShadedAreaProblem
    {
        public Page9Prob9(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 4, 12); points.Add(o);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 12); points.Add(b);
            Point c = new Point("C", 8, 12); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 4));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, (Segment)parser.Get(new Segment(b, c)), da));

            known.AddSegmentLength(da, 8);
            known.AddSegmentLength(cd, 12);

            given.Add(new Strengthened(quad, new Rectangle(quad)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(96 - (8 * System.Math.PI));

            problemName = "Glencoe Page 9 Problem 9";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}