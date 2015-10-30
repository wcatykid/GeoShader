using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class PathologicalTester : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public PathologicalTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 4, 0); points.Add(b);
            Point c = new Point("C", 0, -2); points.Add(c);
            Point d = new Point("D", 4, -2); points.Add(d);

            circles.Add(new Circle(a, 2.0));
            circles.Add(new Circle(b, 2.0));

            segments.Add(new Segment(c, d));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            //given.Add(new GeometricCongruentSegments(mn, mp));

            //goals.Add(new GeometricCongruentSegments(cd, (Segment)parser.Get(new Segment(p, n))));
        }
    }
}