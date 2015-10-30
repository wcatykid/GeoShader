using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page4prob17 : ActualShadedAreaProblem
    {
        public Page4prob17(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -System.Math.Sqrt(27), 3); points.Add(a);
            Point b = new Point("B", System.Math.Sqrt(27), 3); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 0, -6); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment op = new Segment(o, p); segments.Add(op);

            circles.Add(new Circle(o, 6.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(op, 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((System.Math.PI * 36) - 18 * (((2 * System.Math.PI) / 3) - System.Math.Sin(120)));
        }
    }
}