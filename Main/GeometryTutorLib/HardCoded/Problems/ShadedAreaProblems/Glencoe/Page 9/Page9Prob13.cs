using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob13 : ActualShadedAreaProblem
    {
        public Page9Prob13(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 22, 0); points.Add(b);
            Point c = new Point("C", 22, 14); points.Add(c);
            Point d = new Point("D", 0, 14); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            Point x = new Point("X", 0, 7); points.Add(x);
            Point y = new Point("Y", 22, 7); points.Add(y);

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

            circles.Add(new Circle(x, 7.0));
            circles.Add(new Circle(y, 7.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)),
                                                                             (Segment)parser.Get(new Segment(b, c)),
                                                                             cd, ab));
            given.Add(new Strengthened(quad, new Rectangle(quad)));
            
            known.AddSegmentLength(ab, 22);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 14);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 11, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(308 - System.Math.PI * 7 * 7);

            problemName = "Glencoe Page 9 Problem 13";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}