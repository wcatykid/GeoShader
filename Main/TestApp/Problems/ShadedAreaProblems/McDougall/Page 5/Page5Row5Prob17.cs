using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page5Row5Prob17 : ActualShadedAreaProblem
    {
        public Page5Row5Prob17(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", 6, 6); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment da = new Segment(d, a); segments.Add(da);

            Point x = new Point("X", 0, 3.0); points.Add(x);
            Point y = new Point("Y", 6, 3.0); points.Add(y);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(x, 3.0));
            circles.Add(new Circle(y, 3.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(da, (Segment)parser.Get(new Segment(a, b))));
            given.Add(new GeometricCongruentSegments(da, (Segment)parser.Get(new Segment(c, d))));
            
            known.AddSegmentLength(da, 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, .5));
            wanted.Add(new Point("", 3, 5.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(36 - System.Math.PI * 3 * 3);
        }
    }
}