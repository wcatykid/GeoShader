using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row4Prob25 : ActualShadedAreaProblem
    {
        public Page5Row4Prob25(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 3); points.Add(b);
            Point c = new Point("C", 9, 3); points.Add(c);
            Point d = new Point("D", 9, 0); points.Add(d);
            Point e = new Point("E", 3, 0); points.Add(e);
            Point f = new Point("F", 6, 3); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(a, 3.0));
            circles.Add(new Circle(c, 3.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, (Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(a, d))));
            given.Add(new Strengthened(quad, new Rectangle(quad)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, e)), 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(f, c)), 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, f)), 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, c)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(27 - 4.5*System.Math.PI);

            problemName = "Jurgensen Page 5 Problem 24";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
