using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page6Prob26 : ActualShadedAreaProblem
    {
        public Page6Prob26(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -1 * System.Math.Sqrt(25/2), 0); points.Add(a);
            Point b = new Point("B", 0, System.Math.Sqrt(25/2)); points.Add(b);
            Point c = new Point("C", System.Math.Sqrt(25/2), 0); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bo = new Segment(b, o); segments.Add(bo);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, System.Math.Sqrt(25/2)));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(bc, 5);
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, o, a))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -2, 2));
            wanted.Add(new Point("", 2, 2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((25/2) * System.Math.PI - (25/2));
        }
    }
}