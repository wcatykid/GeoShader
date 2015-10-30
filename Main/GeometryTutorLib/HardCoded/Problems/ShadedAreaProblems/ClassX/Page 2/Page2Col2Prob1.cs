using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Col2Prob1 : ActualShadedAreaProblem
    {
        //
        // Triangle intersecting a circle.
        //
        public Page2Col2Prob1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 14, 0); points.Add(b);
            Point c = new Point("C", 14, 14); points.Add(c);
            Point d = new Point("D", 0, 14); points.Add(d);
            Point p = new Point("P", 7, 7); points.Add(p);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            Point x = new Point("X", 0, 7.0); points.Add(x);
            Point y = new Point("Y", 14, 7.0); points.Add(y);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(y);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(x, 7.0));
            circles.Add(new Circle(y, 7.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)),
                                                                             (Segment)parser.Get(new Segment(b, c)), cd, ab));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(ab, 14);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7, 1));
            wanted.Add(new Point("", 7, 13));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(42.06195997);

            problemName = "Class X Page 2 Col 2 Problem 1";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}