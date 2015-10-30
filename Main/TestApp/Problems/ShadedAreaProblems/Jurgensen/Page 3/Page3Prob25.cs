using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page3Prob25 : ActualShadedAreaProblem
    {
        public Page3Prob25(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 4, 4); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point p = new Point("P", 0, 2); points.Add(p);
            Point q = new Point("Q", 2, 4); points.Add(q);
            Point r = new Point("R", 4, 2); points.Add(r);
            Point s = new Point("S", 2, 0); points.Add(s);
            Point o = new Point("O", 2, 2); points.Add(o);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(b);

            pts.Add(b);
            pts.Add(q);
            pts.Add(c);

            pts.Add(c);
            pts.Add(r);
            pts.Add(d);

            pts.Add(d);
            pts.Add(s);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(p, 2.0));
            circles.Add(new Circle(q, 2.0));
            circles.Add(new Circle(r, 2.0));
            circles.Add(new Circle(s, 2.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(d, a))));
            given.Add(new Strengthened(quad, new Square(quad)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, r)), 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, 3));
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", 1, 3));
            wanted.Add(new Point("", 3, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(8 * System.Math.PI - 16);
        }
    }
}