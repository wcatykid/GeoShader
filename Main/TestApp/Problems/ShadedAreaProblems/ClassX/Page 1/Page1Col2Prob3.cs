using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTestbed
{
    public class Page1Col2Prob3 : ActualShadedAreaProblem
    {
        public Page1Col2Prob3(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -3.5, 3.5); points.Add(a);
            Point b = new Point("B", 0, 3.5); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", -5.5, 0); points.Add(d);
            Point e = new Point("E", -3.5, 0); points.Add(e);
            Point f = new Point("F", -3.5 * Math.Sqrt(2) / 2.0, 3.5 * Math.Sqrt(2) / 2.0); points.Add(f);
        
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(e);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(c, 3.5));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricParallel(ab, (Segment)parser.Get(new Segment(c, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(b, c, d))));

            known.AddSegmentLength(ab, 3.5);
            known.AddSegmentLength(bc, 3.5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(d, e)), 2.0);

            goalRegions.Add(parser.implied.GetAtomicRegionByPoint(new Point("", -4, 1)));

            SetSolutionArea(6.128872498);

            problemName = "Page 1 Col 2 Problem 3";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}