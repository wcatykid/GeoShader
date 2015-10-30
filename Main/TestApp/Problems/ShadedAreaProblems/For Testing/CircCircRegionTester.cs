using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class CircCircRegionTester : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public CircCircRegionTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -2, 0); points.Add(a);
            Point b = new Point("B", 1, 0); points.Add(b);
            Point c = new Point("C", 2, 0); points.Add(c);
            Point d = new Point("D", -1, 0); points.Add(d);

            circles.Add(new Circle(a, 3.0));
            circles.Add(new Circle(c, 3.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            // The goal is the entire area of the figure.
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>(parser.implied.atomicRegions);
        }
    }
}