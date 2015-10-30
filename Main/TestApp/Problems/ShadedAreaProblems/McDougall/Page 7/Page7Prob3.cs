using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page7Prob3 : ActualShadedAreaProblem
    {
        public Page7Prob3(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -10, 0); points.Add(a);
            Point b = new Point("B", -2, 0); points.Add(b);
            Point c = new Point("C", 10, 0); points.Add(c);
            Point p = new Point("P", -3.5, 0); points.Add(p);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(b);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 5));
            circles.Add(new Circle(p, 1.5));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 10);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -3.5, 1));
            wanted.Add(new Point("", -3.5, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(2.25 * System.Math.PI);
        }
    }
}