using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob18 : ActualShadedAreaProblem
    {
        public Page10Prob18(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 7); points.Add(b);
            Point c = new Point("C", 7, 7); points.Add(c);
            Point d = new Point("D", 7, 0); points.Add(d);

            Point o = new Point("O", 0, 3.5); points.Add(o);
            Point p = new Point("P", 3.5, 7); points.Add(p);

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(p);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 3.5));
            circles.Add(new Circle(p, 3.5));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = new Quadrilateral((Segment)parser.Get(new Segment(a, b)), cd,
                                                   (Segment)parser.Get(new Segment(b, c)), da);
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(cd, 7);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(49 + (12.25 * System.Math.PI));

            problemName = "Glencoe Page 10 Problem 18";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}