using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob16 : ActualShadedAreaProblem
    {
        public Page10Prob16(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 1, 2); points.Add(b);
            Point c = new Point("C", 3, 6); points.Add(c);
            Point d = new Point("D", 8, 6); points.Add(d);
            Point e = new Point("E", 6, 2); points.Add(e);
            Point f = new Point("F", 6, 0); points.Add(f);
            Point g = new Point("G", 3, 0); points.Add(g);
            Point h = new Point("H", 3, 2); points.Add(h);

            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment be = new Segment(b, e); segments.Add(be);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts.Add(a);
            pts.Add(g);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            pts.Add(b);
            pts.Add(h);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts.Add(c);
            pts.Add(h);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(cd, 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, f)), 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(g, h)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(h, c)), 4);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(h, g, f))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 1));
            wanted.Add(new Point("", 4, 1));
            wanted.Add(new Point("", 2, 3));
            wanted.Add(new Point("", 4, 3));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(32);
        }
    }
}