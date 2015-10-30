using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class SimpleCircleRegion : ActualShadedAreaProblem
    {
        public SimpleCircleRegion(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", -4, 0); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ob = new Segment(o, b); segments.Add(ob);
            Segment oc = new Segment(o, c); segments.Add(oc);

            circles.Add(new Circle(o, 4));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle(b, o, c));

            known.AddSegmentLength(ob, 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -2.8, 2.8));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(4.566370614);

            problemName = "ACT Practice Problem 3";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}