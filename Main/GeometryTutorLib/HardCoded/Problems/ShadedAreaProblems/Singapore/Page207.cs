using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page207 : ActualShadedAreaProblem
    {
        //
        // Polygons only.
        //
        public Page207(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 8);  points.Add(a);
            Point b = new Point("B", 8, 8);  points.Add(b);
            Point c = new Point("C", 8, 0);  points.Add(c);
            Point d = new Point("D", 0, 0);  points.Add(d);
            Point e = new Point("E", 3, 0);  points.Add(e);
            Point f = new Point("F", 5, 0);  points.Add(f);
            Point g = new Point("G", 5, 8);  points.Add(g);
            Point h = new Point("H", 3, 8);  points.Add(h);

            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment he = new Segment(h, e); segments.Add(he);
            Segment fg = new Segment(f, g); segments.Add(fg);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(h);
            pts.Add(g);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(e);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral abcd = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, (Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));
            given.Add(new Strengthened(abcd, new Square(abcd)));

            Quadrilateral efgh = (Quadrilateral)parser.Get(new Quadrilateral(he, fg, (Segment)parser.Get(new Segment(g, h)), (Segment)parser.Get(new Segment(e, f))));
            given.Add(new Strengthened(efgh, new Rectangle(efgh)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, g)), 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, c)), 5);
            known.AddSegmentLength(ad, 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(24);

            problemName = "Singapore Problem Page 207";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}