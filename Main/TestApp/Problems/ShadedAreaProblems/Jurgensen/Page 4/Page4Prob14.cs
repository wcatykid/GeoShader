using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page4Prob14 : ActualShadedAreaProblem
    {
        public Page4Prob14(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 8); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 14, 0); points.Add(d);
            Point e = new Point("E", 10, 8); points.Add(e);

            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment ae = new Segment(a, e); segments.Add(ae);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(c, d))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(d, e))));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, c)), 6);
            known.AddSegmentLength(bd, 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(10.0 * System.Math.Sqrt(5));

            problemName = "Jurgensen Page 4 Problem 14";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}