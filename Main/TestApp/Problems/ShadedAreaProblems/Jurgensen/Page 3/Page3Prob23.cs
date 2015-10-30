using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page3Prob23 : ActualShadedAreaProblem
    {
        public Page3Prob23(bool onoff, bool complete) : base(onoff, complete)
        {
            Point q = new Point("Q", -2, 0); points.Add(q);
            Point p = new Point("P", 2, 0); points.Add(p);
            Point o = new Point("O", 0, 0); points.Add(o);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(o);
            pts.Add(p);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 4));
            circles.Add(new Circle(p, 2));
            circles.Add(new Circle(q, 2));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, q)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(p, o)), 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, -3));
            wanted.Add(new Point("", -2, -1));
            wanted.Add(new Point("", -2, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(8 * System.Math.PI);
        }
    }
}