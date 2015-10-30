using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page1Col1Prob1 : ActualShadedAreaProblem
    {
        public Page1Col1Prob1(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 10.54, 6.72); points.Add(a);
            Point b = new Point("B", 12.5, 0); points.Add(b);
            Point c = new Point("C", -12.5, 0); points.Add(c);
            Point d = new Point("D", 0, -12.5); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);
        
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment od = new Segment(o, d); segments.Add(od);

            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 12.5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, o, d))));

            known.AddSegmentLength(ac, 24);
            known.AddSegmentLength(ab, 7);
            //known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(b, o, d)), 90);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0, 1));
            unwanted.Add(new Point("", 1, 0.1));
            unwanted.Add(new Point("", 10, 0.1));
            unwanted.Add(new Point("", -1, -1));
            unwanted.Add(new Point("", -2, -12));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(284.1553891);

            problemName = "Class X Page 1 Col 1 Problem 1";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}