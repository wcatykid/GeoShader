using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page4Prob7 : ActualShadedAreaProblem
    {
        public Page4Prob7(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 8, 0); points.Add(b);
            Point c = new Point("C", 8, -8); points.Add(c);
            Point d = new Point("D", 0, -8); points.Add(d);
            Point o = new Point("O", 4, -4); points.Add(o);

            Point x = new Point("X", 4, 0); points.Add(x);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            Segment ox = new Segment(o, x); segments.Add(ox);

            circles.Add(new Circle(o, 4.0));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, ab, cd));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(ab, 8);
            known.AddSegmentLength(ox, 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7.9, -0.1));
            wanted.Add(new Point("", 0.1, -0.1));
            wanted.Add(new Point("", 7.9, -7.9));
            wanted.Add(new Point("", 0.1, -7.9));

            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(64 - 16 * System.Math.PI);

            problemName = "Jurgensen Page 4 Problem 7";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}