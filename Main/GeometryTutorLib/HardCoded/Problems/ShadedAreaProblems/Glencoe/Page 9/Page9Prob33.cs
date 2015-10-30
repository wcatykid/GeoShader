using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob33 : ActualShadedAreaProblem
    {
        public Page9Prob33(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 2.5); points.Add(b);
            Point c = new Point("C", 0, 5); points.Add(c);
            Point d = new Point("D", 5, 5); points.Add(d);
            Point e = new Point("E", 5, 0); points.Add(e);

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ea = new Segment(e, a); segments.Add(ea);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(b, 2.5));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = new Quadrilateral((Segment)parser.Get(new Segment(a, c)), de, cd, ea);
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(cd, 5);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(25 - (2.5 * 2.5 * System.Math.PI / 2.0));

            problemName = "Glencoe Page 9 Problem 33";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}