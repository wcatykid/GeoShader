using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Row6Prob44 : ActualShadedAreaProblem
    {
        public Page8Row6Prob44(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", 2.5, 0); points.Add(p);
            Point q = new Point("Q", 5, 0); points.Add(q);
            Point r = new Point("R", 7.5, 0); points.Add(r);
            Point s = new Point("S", 10, 0); points.Add(s);
            Point t = new Point("T", 12.5, 0); points.Add(t);
            Point u = new Point("U", 15, 0); points.Add(u);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(p);
            pts.Add(q);
            pts.Add(r);
            pts.Add(s);
            pts.Add(t);
            pts.Add(u);
            collinear.Add(new Collinear(pts));

            Circle outer = new Circle(r, 7.5);
            Circle left = new Circle(p, 2.5);
            Circle middle = new Circle(r, 2.5);
            Circle right = new Circle(t, 2.5);

            circles.Add(outer);
            circles.Add(left);
            circles.Add(middle);
            circles.Add(right);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, u)), 15);

            //Problem is slightly ambiguous as written, appears to be reliant on assumption that the diameter of each inner circle is 1/3 the total diameter
            Segment oq = (Segment)parser.Get(new Segment(o, q));
            oq.multiplier = 3;
            given.Add(new GeometricSegmentEquation(oq, (Segment)parser.Get(new Segment(o, u))));
            given.Add(new GeometricCongruentCircles(left, middle));
            given.Add(new GeometricCongruentCircles(middle, right));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7, 7));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(18.75 * System.Math.PI);

            problemName = "Glencoe Page 8 Row 6 Problem 44";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
