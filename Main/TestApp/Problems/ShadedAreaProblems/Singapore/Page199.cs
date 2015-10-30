using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page199 : ActualShadedAreaProblem
    {
        //
        // Polygons only.
        //
        public Page199(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0);   points.Add(a);
            Point b = new Point("B", 5, 0);   points.Add(b);
            Point c = new Point("C", 13, 0);  points.Add(c);
            Point d = new Point("D", 13, 8);  points.Add(d);
            Point e = new Point("E", 5, 8);   points.Add(e);
            Point f = new Point("F", 5, 5);   points.Add(f);
            Point g = new Point("G", 0, 5);   points.Add(g);
            // Point h = new Point("H", 0, 3);   points.Add(h);

            Segment ag = new Segment(a, g); segments.Add(ag);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment af = new Segment(a, f); segments.Add(af);
            Segment fd = new Segment(f, d); segments.Add(fd);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(f);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral abfg = (Quadrilateral)parser.Get(new Quadrilateral(ag,
                                                                             (Segment)parser.Get(new Segment(b, f)),
                                                                             fg,
                                                                             (Segment)parser.Get(new Segment(a, b))));
            given.Add(new Strengthened(abfg, new Square(abfg)));

            Quadrilateral bcde = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(b, e)),
                                                                             cd,
                                                                             de,
                                                                             (Segment)parser.Get(new Segment(b, c))));
            given.Add(new Strengthened(bcde, new Square(bcde)));

            known.AddSegmentLength(de, 8);
            known.AddSegmentLength(ag, 5);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4.9, 4.9));
            wanted.Add(new Point("", 5.1, 4.9));
            wanted.Add(new Point("", 5.5, 5.1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(12.5);

            problemName = "Singapore Problem Page 199";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}