using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page7Prob4 : ActualShadedAreaProblem
    {
        public Page7Prob4(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 8, 5); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            Point e = new Point("E", 3, 0); points.Add(e);
            Point f = new Point("F", 3, 5); points.Add(f);
            Point g = new Point("G", 5, 3); points.Add(g);
            Point h = new Point("H", 5, 0); points.Add(h);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment he = new Segment(h, e); segments.Add(he);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts.Add(a);
            pts.Add(e);
            pts.Add(h);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(he, 2);
            known.AddSegmentLength(gh, 3);
            known.AddSegmentLength(cd, 5);
            known.AddSegmentLength(bc, 8);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, b, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, a, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, d, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, e, f))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(g, h, d))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 2));
            wanted.Add(new Point("", 7, 2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(32);
        }
    }
}