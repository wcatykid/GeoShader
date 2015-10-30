using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Prob5 : ActualShadedAreaProblem
    {
        public Page8Prob5(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x = 2.4 * System.Math.Sqrt(3) / 2;
            double y = 1.2;

            Point a = new Point("A", -x, -y); points.Add(a);
            Point b = new Point("B", 0, 2.4); points.Add(b);
            Point c = new Point("C", x, -y); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ca = new Segment(c, a); segments.Add(ca);
            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ob = new Segment(o, b); segments.Add(ob); // Until triangles from atomizer are available, 
            Segment oc = new Segment(o, c); segments.Add(oc); // these two segments must be defined for the problem to work

            circles.Add(new Circle(o, 2.4));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(oa, 2.4);

            Triangle abc = (Triangle)parser.Get(new Triangle(a, b, c));
            given.Add(new Strengthened(abc, new EquilateralTriangle(abc)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -x, 0));
            wanted.Add(new Point("", x, 0));
            wanted.Add(new Point("", 0, -y - 0.1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((5.76 * System.Math.PI) - (108 * System.Math.Sqrt(3)) / 25);

            problemName = "Glencoe Page 8 Problem 5";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}