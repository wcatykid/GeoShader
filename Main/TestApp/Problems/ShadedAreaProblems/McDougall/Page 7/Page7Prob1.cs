using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page7Prob1 : ActualShadedAreaProblem
    {
        public Page7Prob1(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 12); points.Add(b);
            Point c = new Point("C", 18, 12); points.Add(c);
            Point d = new Point("D", 18, 0); points.Add(d);
            Point p = new Point("P", 0, -18); points.Add(p);


            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment ap = new Segment(a, p); segments.Add(ap);

            circles.Add(new Circle(a, 18));
            circles.Add(new Circle(b, 6));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ap, 18);
            known.AddSegmentLength(cd, 12);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, b, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, d, a))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, a, b))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, -1));
            wanted.Add(new Point("", -1, 0));
            wanted.Add(new Point("", 1, 13));
            wanted.Add(new Point("", -1, 13));
            wanted.Add(new Point("", -1, 11));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(252 * System.Math.PI);
        }
    }
}