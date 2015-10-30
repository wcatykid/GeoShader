using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page3Prob22 : ActualShadedAreaProblem
    {
        public Page3Prob22(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 8, 4); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            Point q = new Point("Q", 8, 2); points.Add(q);
            Point p = new Point("P", 4, 2); points.Add(p);
            Point o = new Point("O", 0, 2); points.Add(o);

            Point m = new Point("M", 4, 4); points.Add(m);
            Point n = new Point("N", 4, 0); points.Add(n);

            Point x = new Point("X", 2, 2); points.Add(x);
            Point y = new Point("Y", 6, 2); points.Add(y);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(q);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(m);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(n);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(o);
            pts.Add(x);
            pts.Add(p);
            pts.Add(y);
            pts.Add(q);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 2));
            circles.Add(new Circle(p, 2));
            circles.Add(new Circle(q, 2));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, b)),
                                                                             (Segment)parser.Get(new Segment(c, d)),
                                                                             (Segment)parser.Get(new Segment(b, c)),
                                                                             (Segment)parser.Get(new Segment(d, a))));
            given.Add(new Strengthened(quad, new Rectangle(quad)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, x)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(x, p)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(q, y)), 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 0.5));
            wanted.Add(new Point("", 2, 3.5));
            wanted.Add(new Point("", 6, 0.5));
            wanted.Add(new Point("", 6, 3.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(4 * (8 - 2 * System.Math.PI));
        }
    }
}