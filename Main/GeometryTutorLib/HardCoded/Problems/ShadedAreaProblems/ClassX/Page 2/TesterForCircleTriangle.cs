using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TesterForCircleTriangle : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public TesterForCircleTriangle(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 1, 0); points.Add(a);
            Point b = new Point("B", 6, 3); points.Add(b);
            Point c = new Point("C", 6, -3); points.Add(c);

            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment bc = new Segment(b, c); segments.Add(bc);

            circles.Add(new Circle(o, 5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
        }
    }
}