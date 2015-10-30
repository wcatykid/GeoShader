using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row4Prob24 : ActualShadedAreaProblem
    {
        public Page5Row4Prob24(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 13, 0); points.Add(b);
            Point c = new Point("C", 13, 6); points.Add(c);
            Point d = new Point("D", 0, 6); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            Point x = new Point("X", 0, 3); points.Add(x);
            Point y = new Point("Y", 13, 3); points.Add(y);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(x, 3.0));
            circles.Add(new Circle(y, 3.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)),
                                                                             (Segment)parser.Get(new Segment(b, c)),
                                                                             cd, ab));
            given.Add(new Strengthened(quad, new Rectangle(quad)));
            
            known.AddSegmentLength(ab, 13);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 6, 3));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(78 - System.Math.PI * 3 * 3);

            problemName = "Jurgensen Page 5 Problem 24";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}