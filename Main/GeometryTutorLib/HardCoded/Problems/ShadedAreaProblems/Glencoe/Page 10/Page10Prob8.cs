using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob8 : ActualShadedAreaProblem
    {
        public Page10Prob8(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 8.5, 15); points.Add(b);
            Point c = new Point("C", 17, 0); points.Add(c);
            Point d = new Point("D", 8.5, -15); points.Add(d);
            Point e = new Point("E", 4.25, 7.5); points.Add(e);
            Point f = new Point("F", 8.5, 0); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment fb = new Segment(f, b); segments.Add(fb);
            Segment ec = new Segment(e, c); segments.Add(ec);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ec, 15);
            known.AddSegmentLength(fb, 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 17);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, d)), 28);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, f, b))));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4.25, -3));
            wanted.Add(new Point("", 9, -8));
            wanted.Add(new Point("", 8, 2));
            wanted.Add(new Point("", 9, 2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(261);
        }
    }
}