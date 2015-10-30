using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class FailingCircCircRegionTester : ActualShadedAreaProblem
    {
        public FailingCircCircRegionTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -2, 0); points.Add(a);
            Point c = new Point("C", 2, 0); points.Add(c);

            circles.Add(new Circle(a, 3.0));
            circles.Add(new Circle(c, 2.0));

            //List<Point> pts = new List<Point>();
            //pts.Add(a);
            //pts.Add(d);
            //pts.Add(b);
            //pts.Add(c);
            //collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            //known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 3.0);
            //known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 3.0);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(42.06195997);

            problemName = "Circle-Circle Problem";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}