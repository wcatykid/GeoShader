using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Row5Prob40 : ActualShadedAreaProblem
    {
        public Page8Row5Prob40(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -4.5, 6);   points.Add(a);
            Point b = new Point("B", 4.5, 6);    points.Add(b);
            Point c = new Point("C", 4.5, -6);    points.Add(c);
            Point d = new Point("D", -4.5, -6);   points.Add(d);

            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 7.5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ad, 12);
            known.AddSegmentLength(cd, 9);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, 7.4));
            wanted.Add(new Point("", 0, -7.4));
            wanted.Add(new Point("", 7.4, 0));
            wanted.Add(new Point("", -7.4, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(7.5 * 7.5 * System.Math.PI - 108);

            problemName = "Glencoe Page 8 Row 5 Problem 40";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}