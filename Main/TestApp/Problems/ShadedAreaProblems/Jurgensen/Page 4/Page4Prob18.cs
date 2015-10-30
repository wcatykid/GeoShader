using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page4prob18 : ActualShadedAreaProblem
    {
        public Page4prob18(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -6.5, 0); points.Add(a);
            Point b = new Point("B", System.Math.Sqrt(27), 3); points.Add(b);
            Point c = new Point("C", 6.5, 0); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 6.5));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 13);
            known.AddSegmentLength(ab, 5);
            known.AddSegmentLength(bc, 12);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -6.4, 1.1));
            wanted.Add(new Point("", 0, 6));
            wanted.Add(new Point("", 0, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(42.25 * System.Math.PI - 30);
        }
    }
}