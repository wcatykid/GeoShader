using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page208 : ActualShadedAreaProblem
    {
        //
        // Polygons only.
        //
        public Page208(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0);  points.Add(a);
            Point b = new Point("B", 5, 0);  points.Add(b);
            Point c = new Point("C", 8, 0);  points.Add(c);
            Point d = new Point("D", 8, 3);  points.Add(d);
            Point e = new Point("E", 5, 3);  points.Add(e);
            Point f = new Point("F", 8, 5);  points.Add(f);
            Point g = new Point("G", 5, 5);  points.Add(g);
            Point h = new Point("H", 0, 5);  points.Add(h);
            Point i = new Point("I", 5, 1.875); points.Add(i);

            Segment ha = new Segment(h, a); segments.Add(ha);
            Segment hb = new Segment(h, b); segments.Add(hb);
            Segment ed = new Segment(e, d); segments.Add(ed);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(h);
            pts.Add(g);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(i);
            pts.Add(e);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(f);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(h);
            pts.Add(i);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral abgh = (Quadrilateral)parser.Get(new Quadrilateral(ha, (Segment)parser.Get(new Segment(b, g)),
                                                                            (Segment)parser.Get(new Segment(g, h)), (Segment)parser.Get(new Segment(a, b))));
            given.Add(new Strengthened(abgh, new Square(abgh)));

            Quadrilateral bcde = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(b, e)), (Segment)parser.Get(new Segment(c, d)),
                                                                             ed, (Segment)parser.Get(new Segment(b, c))));
            given.Add(new Strengthened(bcde, new Square(bcde)));

            Quadrilateral acfh = (Quadrilateral)parser.Get(new Quadrilateral(ha, (Segment)parser.Get(new Segment(c, f)),
                                                                            (Segment)parser.Get(new Segment(f, h)), (Segment)parser.Get(new Segment(a, c))));
            given.Add(new Strengthened(acfh, new Rectangle(acfh)));

            known.AddSegmentLength(ha, 5);
            known.AddSegmentLength(ed, 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 3.05));
            wanted.Add(new Point("", 4.5, 1));
            wanted.Add(new Point("", 5.1, 0.1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(7.5);

            problemName = "Singapore Problem Page 208";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}