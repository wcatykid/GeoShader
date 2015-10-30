using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page6Prob32c : ActualShadedAreaProblem
    {
        public Page6Prob32c(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point r = new Point("R", -8 * System.Math.Sin(0.3 * System.Math.PI), 8 * System.Math.Cos(0.3 * System.Math.PI)); points.Add(r);
            Point p = new Point("P", 0, 0); points.Add(p);
            Point s = new Point("S", 8 * System.Math.Sin(0.3 * System.Math.PI), 8 * System.Math.Cos(0.3 * System.Math.PI)); points.Add(s);
            Point q = new Point("Q", 0, -4); points.Add(q);

            Segment rp = new Segment(r, p); segments.Add(rp);
            Segment ps = new Segment(p, s); segments.Add(ps);
            Segment pq = new Segment(p, q); segments.Add(pq);

            circles.Add(new Circle(p, 8));
            circles.Add(new Circle(q, 4));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(pq, 4);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(r, p, s)), 108);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7, 0));
            wanted.Add(new Point("", -7, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(28.8 * System.Math.PI);
        }
    }
}