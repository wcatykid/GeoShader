using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class RightRight : ActualShadedAreaProblem
    {
        public RightRight(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 24); points.Add(b);
            Point c = new Point("C", 0, 48); points.Add(c);
            Point d = new Point("D", 48, 0); points.Add(d);
            Point e = new Point("E", 24, 0); points.Add(e);

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment be = new Segment(b, e); segments.Add(be);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle(c, a, d));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(b, (Segment)parser.Get(new Segment(a, c))))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle(e, (Segment)parser.Get(new Segment(a, d))))));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 48);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, d)), 48);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 24, 23));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(864);

            problemName = "Right Triangle - Right Triangle  Synthesis";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}