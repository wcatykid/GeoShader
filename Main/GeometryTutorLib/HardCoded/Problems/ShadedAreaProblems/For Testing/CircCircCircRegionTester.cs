using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class CircCircCircRegionTester : ActualShadedAreaProblem
    {
        public CircCircCircRegionTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 4, 0); points.Add(b);
            Point c = new Point("C", 2, 2 * System.Math.Sqrt(3)); points.Add(c);

            circles.Add(new Circle(a, 3.0));
            circles.Add(new Circle(b, 3.0));
            circles.Add(new Circle(c, 3.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            // The goal is the entire area of the figure.
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>(parser.implied.atomicRegions);
        }
    }
}