using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Row5Prob41 : ActualShadedAreaProblem
    {
        public Page8Row5Prob41(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 10); points.Add(o);
            Point p = new Point("P", 0, 5); points.Add(p);
            Point q = new Point("Q", 0, 0); points.Add(q);
            Point r = new Point("R", 0, -5); points.Add(r);
            Point s = new Point("S", 0, -10); points.Add(s);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(p);
            pts.Add(q);
            pts.Add(r);
            pts.Add(s);
            collinear.Add(new Collinear(pts));

            Circle outer = new Circle(q, 10);
            Circle top = new Circle(p, 5);
            Circle bottom = new Circle(r, 5);

            circles.Add(outer);
            circles.Add(top);
            circles.Add(bottom);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, s)), 20);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 7.4));
            wanted.Add(new Point("", 0, -7.4));
            wanted.Add(new Point("", 7.4, 0));
            wanted.Add(new Point("", -7.4, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(50 * System.Math.PI);

            problemName = "Glencoe Page 8 Row 5 Problem 41";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
