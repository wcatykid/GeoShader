using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page6Prob4 : ActualShadedAreaProblem
    {
        public Page6Prob4(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 0, 11); points.Add(a);
            Point b = new Point("B", 33, 0); points.Add(b);

            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment ob = new Segment(o, b); segments.Add(ob);

            circles.Add(new Circle(o, 11));
            circles.Add(new Circle(o, 33));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ao, 11);
            known.AddSegmentLength(ob, 33);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 20, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(968 * System.Math.PI);
        }
    }
}