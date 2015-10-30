using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page3Prob26 : ActualShadedAreaProblem
    {
        public Page3Prob26(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -System.Math.Sqrt(27), -3); points.Add(a);
            Point b = new Point("B", -System.Math.Sqrt(27), 3); points.Add(b);
            Point c = new Point("C", 0, 6); points.Add(c);
            Point d = new Point("D", System.Math.Sqrt(27), 3); points.Add(d);
            Point e = new Point("E", System.Math.Sqrt(27), -3); points.Add(e);
            Point f = new Point("F", 0, -6); points.Add(f);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment of = new Segment(o, f); segments.Add(of);

            circles.Add(new Circle(o, 6.0));
            circles.Add(new Circle(a, 6.0));
            circles.Add(new Circle(b, 6.0));
            circles.Add(new Circle(c, 6.0));
            circles.Add(new Circle(d, 6.0));
            circles.Add(new Circle(e, 6.0));
            circles.Add(new Circle(f, 6.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, f)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 3));
            wanted.Add(new Point("", 0.5, -3));
            wanted.Add(new Point("", -0.5, -3));
            wanted.Add(new Point("", -2.5, 1.5));
            wanted.Add(new Point("", -2.5, -1.5));
            wanted.Add(new Point("", 2.5, 1.5));
            wanted.Add(new Point("", 2.5, -1.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(72 * System.Math.PI - 108 * System.Math.Sqrt(3));
        }
    }
}