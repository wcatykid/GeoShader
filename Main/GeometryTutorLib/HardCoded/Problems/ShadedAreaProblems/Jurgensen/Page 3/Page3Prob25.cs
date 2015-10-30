using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page3Prob25 : ActualShadedAreaProblem
    {
        public Page3Prob25(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double sL = System.Math.Sqrt(8); //side length
            double rL = System.Math.Sqrt(2); //half side length

            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, sL); points.Add(b);
            Point c = new Point("C", sL, sL); points.Add(c);
            Point d = new Point("D", sL, 0); points.Add(d);
            Point p = new Point("P", 0, rL); points.Add(p);
            Point q = new Point("Q", rL, sL); points.Add(q);
            Point r = new Point("R", sL, rL); points.Add(r);
            Point s = new Point("S", rL, 0); points.Add(s);
            Point o = new Point("O", rL, rL); points.Add(o);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(q);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(r);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(s);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            Circle top = new Circle(q, 2.0);
            Circle bottom = new Circle(s, 2.0);
            Circle left = new Circle(p, 2.0);
            Circle right = new Circle(r, 2.0);

            circles.Add(top);
            circles.Add(bottom);
            circles.Add(left);
            circles.Add(right);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(d, a))));
            given.Add(new Strengthened(quad, new Square(quad)));
            given.Add(new GeometricCongruentCircles(top, bottom));
            given.Add(new GeometricCongruentCircles(top, left));
            given.Add(new GeometricCongruentCircles(top, right));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 4);

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