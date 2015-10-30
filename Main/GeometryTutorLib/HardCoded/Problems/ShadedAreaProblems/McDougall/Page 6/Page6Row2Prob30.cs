using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page6Row2Prob30 : ActualShadedAreaProblem
    {
        public Page6Row2Prob30(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 2, 0); points.Add(p);
            Point q = new Point("Q", 4, 0); points.Add(q);
            Point r = new Point("R", 6, 0); points.Add(r);
            Point s = new Point("S", 8, 0); points.Add(s);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(p);
            pts.Add(q);
            pts.Add(r);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2));
            circles.Add(new Circle(o, 4));
            circles.Add(new Circle(o, 6));
            circles.Add(new Circle(o, 8));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment rs = (Segment)parser.Get(new Segment(r, s));
            known.AddSegmentLength(rs, 2);
            given.Add(new GeometricCongruentSegments(rs, (Segment)parser.Get(new Segment(o, p))));
            given.Add(new GeometricCongruentSegments(rs, (Segment)parser.Get(new Segment(p, q))));
            given.Add(new GeometricCongruentSegments(rs, (Segment)parser.Get(new Segment(q, r))));


            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 7));
            wanted.Add(new Point("", 0, 3));
            wanted.Add(new Point("", 0, -3));
            wanted.Add(new Point("", 0, -7));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(40 * System.Math.PI);

            problemName = "McDougall Page 6 Row 2 Problem 30";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}