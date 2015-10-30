using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page1Col2Prob2 : ActualShadedAreaProblem
    {
        public Page1Col2Prob2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 3.5); points.Add(a);
            Point c = new Point("C", -3.5, 0); points.Add(c);
            Point d = new Point("D", 0, 2); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment co = new Segment(c, o); segments.Add(co);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(d);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 3.5));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(c, o, d))));

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, d)), 2);
            known.AddSegmentLength(co, 3.5);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0, -1));
            unwanted.Add(new Point("", -1, 1));
            goalRegions.AddRange(parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted));

            SetSolutionArea(6.121127502);

            problemName = "Page 1 Col 2 Problem 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}