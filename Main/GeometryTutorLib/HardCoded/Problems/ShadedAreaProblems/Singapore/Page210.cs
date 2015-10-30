using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page210 : ActualShadedAreaProblem
    {
        //
        // Polygons only.
        //
        public Page210(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0);   points.Add(a);
            Point b = new Point("B", 3, 0);   points.Add(b);
            Point c = new Point("C", 10, 0);  points.Add(c);
            Point d = new Point("D", 10, 7);  points.Add(d);
            Point e = new Point("E", 10, 10); points.Add(e);
            Point f = new Point("F", 7, 10);  points.Add(f);
            Point g = new Point("G", 0, 10);  points.Add(g);
            Point h = new Point("H", 0, 3);   points.Add(h);

            Segment fh = new Segment(f, h); segments.Add(fh);
            Segment fd = new Segment(f, d); segments.Add(fd);
            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment bh = new Segment(b, h); segments.Add(bh);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(h);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral aceg = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, g)),
                                                                             (Segment)parser.Get(new Segment(c, e)),
                                                                             (Segment)parser.Get(new Segment(e, g)),
                                                                             (Segment)parser.Get(new Segment(a, c))));
            given.Add(new Strengthened(aceg, new Square(aceg)));

            Quadrilateral bdfh = (Quadrilateral)parser.Get(new Quadrilateral(bh, fd, fh, bd));
            given.Add(new Strengthened(bdfh, new Rectangle(bdfh)));

            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(f, g)), (Segment)parser.Get(new Segment(c, d))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(e, f)), (Segment)parser.Get(new Segment(e, d))));

            known.AddSegmentLength((Segment)parser.Get(new Segment(f, g)), 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, f)), 3);

            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 7);
            known.AddSegmentLength((Segment)parser.Get(new Segment(d, e)), 3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 5, 5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(42.0);

            problemName = "Singapore Problem Page 210";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}