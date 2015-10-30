using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TwoCircleInteriorTangent : ActualShadedAreaProblem
    {
        public TwoCircleInteriorTangent(bool onoff, bool complete) : base(onoff, complete)
        {
            Point x = new Point("X", 5, 0);   points.Add(x);
            Point y = new Point("Y", 10, 0);   points.Add(y);
            Point a = new Point("A", 0, 0); points.Add(a);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(x, 5.0));
            circles.Add(new Circle(y, 10.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, y)), 10);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 10, 1));
            wanted.Add(new Point("", 10, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(75 * System.Math.PI);

            problemName = "ACT Practice Problem 1";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}