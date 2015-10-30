using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Row6Prob43 : ActualShadedAreaProblem
    {
        public Page8Row6Prob43(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 15); points.Add(a);
            Point b = new Point("B", 0, 10); points.Add(b);
            Point c = new Point("C", 0, 5); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);
            Point e = new Point("E", 0, -5); points.Add(e);
            Point f = new Point("F", 0, -10); points.Add(f);
            Point g = new Point("G", 0, -15); points.Add(g);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            pts.Add(f);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            Circle outer = new Circle(c, 15);
            Circle top = new Circle(b, 5);
            Circle mid1 = new Circle(c, 10);
            Circle mid2 = new Circle(e, 10);
            Circle bottom = new Circle(f, 5);

            circles.Add(outer);
            circles.Add(top);
            circles.Add(mid1);
            circles.Add(mid2);
            circles.Add(bottom);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ag = (Segment)parser.Get(new Segment(a, g));
            known.AddSegmentLength(ag, 30);

            //Problem is slightly ambiguous as written, appears to be reliant on assumption:
            //The three 'stacked' segments of the diameter each make up 1/3 of the total length
            Segment ac = (Segment)parser.Get(new Segment(a, c));
            ac.multiplier = 3;
            Segment ae = (Segment)parser.Get(new Segment(a, e));
            ae.multiplier = 2;
            given.Add(new GeometricSegmentEquation(ac, ag));
            given.Add(new GeometricSegmentEquation(ae, ag));
            given.Add(new GeometricCongruentCircles(top, bottom));
            given.Add(new GeometricCongruentCircles(mid1, mid2));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 5));
            wanted.Add(new Point("", 7.5, 5));
            wanted.Add(new Point("", 12.5, 5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(150 * System.Math.PI);

            problemName = "Glencoe Page 8 Row 6 Problem 43";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

