using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class RegionTester : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public RegionTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 4, 4); points.Add(b);
            Point c = new Point("C", 4, -4); points.Add(c);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ac = new Segment(a, c); segments.Add(ac);

            circles.Add(new Circle(a, 2.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.AtomicRegion>(parser.implied.atomicRegions);

            //given.Add(new GeometricCongruentSegments(mn, mp));

            //goals.Add(new GeometricCongruentSegments(cd, (Segment)parser.Get(new Segment(p, n))));
        }
    }
}