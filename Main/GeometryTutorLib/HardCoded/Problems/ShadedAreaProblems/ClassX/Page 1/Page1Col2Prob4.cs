using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page1Col2Prob4 : ActualShadedAreaProblem
    {
        public Page1Col2Prob4(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 14, 0); points.Add(b);

            Point x = new Point("X", 1.75, 0);   points.Add(x);
            Point y = new Point("Y", 7, 0);   points.Add(y);
            Point z = new Point("Z", 12.25, 0); points.Add(z);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(y);
            pts.Add(z);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(x, 1.75));
            circles.Add(new Circle(y, 3.5));
            circles.Add(new Circle(z, 1.75));
            circles.Add(new Circle(y, 7));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 14);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, x)), 1.75);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, z)), 1.75);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7, 1));
            wanted.Add(new Point("", 7, -1));
            wanted.Add(new Point("", 7, 6));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(86.59014751);

            problemName = "Class X Page 1 Column 2 Problem 4";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}