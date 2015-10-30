using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Col1Prob2 : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public Page2Col1Prob2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 6 * System.Math.Sqrt(3), -6); points.Add(a);
            Point b = new Point("B", 6 * System.Math.Sqrt(3), 6); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);

            // Added for proper statement of knowns.
            Point m = new Point("M", 7 * System.Math.Sqrt(3) / 2.0, 3.5); points.Add(m);
            Point n = new Point("N", 7 * System.Math.Sqrt(3) / 2.0, -3.5); points.Add(n);

            Segment ab = new Segment(a, b); segments.Add(ab);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(m);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(o);
            pts.Add(n);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 7.0));


            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Triangle tri = (Triangle)parser.Get(new Triangle(a, b, o));
            given.Add(new EquilateralTriangle(tri));

            known.AddSegmentLength(ab, 12);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, m)), 7);

            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>(parser.implied.GetAllAtomicRegions());

            SetSolutionArea(190.6355291);

            problemName = "Class X Page 2 Col 1 Problem 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}