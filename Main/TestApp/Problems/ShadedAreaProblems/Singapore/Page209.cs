using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page209 : ActualShadedAreaProblem
    {
        //
        // Polygons only.
        //
        public Page209(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 6, 6);  points.Add(a);
            Point b = new Point("B", 0, 6);  points.Add(b);
            Point c = new Point("C", 0, 0);  points.Add(c);
            Point d = new Point("D", 6, 0);  points.Add(d);
            Point e = new Point("E", 10, 0);  points.Add(e);
            Point f = new Point("F", 10, 4);  points.Add(f);
            Point g = new Point("G", 6, 4);  points.Add(g);
            Point x = new Point("X", 6, 2.4); points.Add(x);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment fg = new Segment(f, g); segments.Add(fg);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(g);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(x);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral abcd = (Quadrilateral)parser.Get(new Quadrilateral(bc, (Segment)parser.Get(new Segment(a, d)), ab, (Segment)parser.Get(new Segment(c, d))));
            given.Add(new Strengthened(abcd, new Square(abcd)));

            Quadrilateral defg = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(g, d)), ef, fg, (Segment)parser.Get(new Segment(d, e))));
            given.Add(new Strengthened(defg, new Square(defg)));

            known.AddSegmentLength(ab, 6);
            known.AddSegmentLength(ef, 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 6.1, 3.9));
            wanted.Add(new Point("", 3, 2.9));
            wanted.Add(new Point("", 5.9, 5.8));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(14.0);

            problemName = "Singapore Problem Page 209";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}