using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TwoIsoscelesTriangles : ActualShadedAreaProblem
    {
        public TwoIsoscelesTriangles(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);
            Point d = new Point("D", 1, 0); points.Add(d);
            Point e = new Point("E", 1, 3); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment de = new Segment(d, e); segments.Add(de);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(e);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(ab, (Segment)parser.Get(new Segment(a, c))));
            given.Add(new RightAngle(b, a, c));
            given.Add(new RightAngle(e, d, c));

            known.AddSegmentLength(ab, 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0.5, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(3.5);

            problemName = "ACT Practice Problem 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}