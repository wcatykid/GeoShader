using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page1Col1Prob3 : ActualShadedAreaProblem
    {
        public Page1Col1Prob3(bool onoff, bool complete) : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 7, 0); points.Add(a);
            Point b = new Point("B", 7, 7); points.Add(b);
            Point c = new Point("C", 0, 7); points.Add(c);
            Point f = new Point("F", 7 * Math.Sqrt(2) / 2.0, 7 * Math.Sqrt(2) / 2.0); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ao = new Segment(a, o); segments.Add(ao);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment co = new Segment(c, o); segments.Add(co);

            circles.Add(new Circle(o, 7));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(co, ab, bc, ao));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(ao, 7);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 6, 6));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(10.515489999);

            problemName = "Class X Page 1 Col 1 Problem 3";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}