using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Col2Prob20 : ActualShadedAreaProblem
    {
        public Page2Col2Prob20(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -1 * System.Math.Sqrt(3), 1); points.Add(a);
            Point b = new Point("B", System.Math.Sqrt(3), 1); points.Add(b);
            Point c = new Point("C", -1 * System.Math.Sqrt(3), -1); points.Add(c);
            Point d = new Point("D", System.Math.Sqrt(3), -1); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ac = new Segment(a, c); segments.Add(ac);

            circles.Add(new Circle(o, 2));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ac, 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", -1.8, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((4.0 / 3.0) * System.Math.PI + 2.0 * System.Math.Sqrt(3.0));
        }
    }
}