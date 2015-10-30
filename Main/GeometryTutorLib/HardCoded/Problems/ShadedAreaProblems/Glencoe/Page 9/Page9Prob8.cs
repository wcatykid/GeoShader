using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob8 : ActualShadedAreaProblem
    {
        public Page9Prob8(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 3, 2.5); points.Add(b);
            Point c = new Point("C", 0, 5); points.Add(c);
            Point d = new Point("D", 10, 5); points.Add(d);
            Point e = new Point("E", 13, 2.5); points.Add(e);
            Point f = new Point("F", 10, 0); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fa = new Segment(f, a); segments.Add(fa);

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 5);
            known.AddSegmentLength(fa, 10);
            known.AddSegmentLength(cd, 10);

            given.Add(new GeometricCongruentSegments(ab, bc));
            given.Add(new GeometricCongruentSegments(ab, de));
            given.Add(new GeometricCongruentSegments(ab, ef));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 6, 2.5));;
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(50);
        }
    }
}