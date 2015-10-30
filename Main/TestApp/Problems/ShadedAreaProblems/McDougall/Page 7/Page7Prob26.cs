using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page7Prob26 : ActualShadedAreaProblem
    {
        public Page7Prob26(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 4, 4); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point o = new Point("O", 2, 2); points.Add(o);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment da = new Segment(d, a); segments.Add(da);

            circles.Add(new Circle(o, 2));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(da, 4);
            known.AddSegmentLength(cd, 4);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, b, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, d, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, a, b))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 1));
            wanted.Add(new Point("", 0.5, 0.5));
            wanted.Add(new Point("", 3.5, 0.5));
            wanted.Add(new Point("", 2, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(8 + 2 * System.Math.PI);
        }
    }
}