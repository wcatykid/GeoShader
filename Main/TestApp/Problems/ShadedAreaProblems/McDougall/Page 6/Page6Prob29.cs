using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page6Prob29 : ActualShadedAreaProblem
    {
        public Page6Prob29(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 17); points.Add(a);
            Point b = new Point("B", 0, 34); points.Add(b);
            Point c = new Point("C", 0, -34); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 34));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 17);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(b, o, c)), 180);
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(a, o))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 19, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(433.5 * System.Math.PI);
        }
    }
}