using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page6Prob30 : ActualShadedAreaProblem
    {
        public Page6Prob30(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 2, 0); points.Add(p);
            Point q = new Point("Q", 4, 0); points.Add(q);
            Point r = new Point("R", 6, 0); points.Add(r);
            Point s = new Point("S", 8, 0); points.Add(s);

            Segment rs = new Segment(r, s); segments.Add(rs);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(p);
            pts.Add(q);
            pts.Add(r);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 8));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(rs, 2);
            given.Add(new GeometricCongruentSegments(rs, (Segment)parser.Get(new Segment(o, p))));
            given.Add(new GeometricCongruentSegments(rs, (Segment)parser.Get(new Segment(p, q))));
            given.Add(new GeometricCongruentSegments(rs, (Segment)parser.Get(new Segment(q, r))));


            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, 0));
            wanted.Add(new Point("", 7, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(40 * System.Math.PI);
        }
    }
}