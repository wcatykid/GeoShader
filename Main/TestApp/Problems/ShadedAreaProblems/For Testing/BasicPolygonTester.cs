using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class BasicPolygonTester : ActualShadedAreaProblem
    {
        public BasicPolygonTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -2, 0); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", 2, 0); points.Add(c);
            Point d = new Point("D", 3, -1); points.Add(d);
            Point e = new Point("E", 1, 3); points.Add(e);
            Point f = new Point("F", 0, 0); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fd = new Segment(f, d); segments.Add(fd);

            Segment cd = new Segment(c, d); segments.Add(cd);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            // The goal is the entire area of the figure.
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>(parser.implied.atomicRegions);
        }
    }
}