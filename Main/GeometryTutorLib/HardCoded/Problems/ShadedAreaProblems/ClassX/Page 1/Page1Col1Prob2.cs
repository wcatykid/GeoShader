﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;
using System;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page1Col1Prob2 : ActualShadedAreaProblem
    {
        public Page1Col1Prob2(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 14); points.Add(a);
            Point b = new Point("B", 14, 14); points.Add(b);
            Point c = new Point("C", 14, 0); points.Add(c);
            Point d = new Point("D", 0, 0); points.Add(d);
            Point p = new Point("P", 7, 7); points.Add(p);

            Point x = new Point("X", 0, 7); points.Add(x);
            Point y = new Point("Y", 14, 7); points.Add(y);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            circles.Add(new Circle(x, 7));
            circles.Add(new Circle(y, 7));

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(y);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)), (Segment)parser.Get(new Segment(b, c)), ab, cd));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength(ab, 14);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 7, 10));
            wanted.Add(new Point("", 7, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(42.06195997);

            problemName = "Class X Page 1 Col 1 Problem 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}