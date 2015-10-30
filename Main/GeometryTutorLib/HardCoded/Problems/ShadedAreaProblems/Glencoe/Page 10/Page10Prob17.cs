using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page10Prob17 : ActualShadedAreaProblem
    {
        public Page10Prob17(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5); points.Add(b);
            Point c = new Point("C", 3, 5 + 3 * System.Math.Sqrt(3)); points.Add(c);
            Point d = new Point("D", 6, 5); points.Add(d);
            Point e = new Point("E", 9, 5 + 3 * System.Math.Sqrt(3)); points.Add(e);
            Point f = new Point("F", 12, 5); points.Add(f);
            Point g = new Point("G", 12, 0); points.Add(g);
            Point h = new Point("H", 6, 0); points.Add(h);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment dh = new Segment(d, h); segments.Add(dh);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(h);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(bc, cd));
            given.Add(new GeometricCongruentSegments(bc, de));
            given.Add(new GeometricCongruentSegments(bc, ef));
            given.Add(new GeometricCongruentSegments(bc, (Segment)parser.Get(new Segment(a, h))));
            given.Add(new GeometricCongruentSegments(bc, (Segment)parser.Get(new Segment(h, g))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(d, h, g))));
            given.Add(new GeometricCongruentSegments(ab, fg));
            given.Add(new GeometricCongruentSegments(dh, fg));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, g)), 12);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, h)), 6);
            known.AddSegmentLength(fg, 5);
            known.AddSegmentLength(ef, 6);

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(114);

            problemName = "Glencoe Page 10 Problem 17";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}