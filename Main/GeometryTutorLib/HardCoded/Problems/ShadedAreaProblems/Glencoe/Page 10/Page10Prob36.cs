using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob36 : ActualShadedAreaProblem
    {
        public Page10Prob36(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 9); points.Add(b);
            Point c = new Point("C", 8, 9); points.Add(c);
            Point d = new Point("D", 12, 9); points.Add(d);
            Point e = new Point("E", 12, 0); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ea = new Segment(e, a); segments.Add(ea);

            Point x = new Point("X", 4, 9.0); points.Add(x);
            circles.Add(new Circle(x, 4.0));

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(c);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            CircleSegmentIntersection csi = new CircleSegmentIntersection(b, circles[0], ab);
            given.Add(new Tangent((CircleSegmentIntersection)parser.Get(csi)));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, a, e))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, d)), ea));

            known.AddSegmentLength(ab, 9);
            known.AddSegmentLength(cd, 4);
            known.AddSegmentLength(ea, 12);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 10, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(108 - 16 * System.Math.PI / 2.0);

            problemName = "Glencoe Page 10 Problem 36";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}