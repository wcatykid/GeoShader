using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class ExtraSquareProblem : QuadrilateralsProblem
    {
        public ExtraSquareProblem(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 10, 0); points.Add(b);
            Point c = new Point("C", 10, 10); points.Add(c);
            Point d = new Point("D", 0, 10); points.Add(d);
            Point m = new Point("M", 5, 5); points.Add(m);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(m);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(m);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));


            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);


            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, cd, ab));
            Segment bd = (Segment)parser.Get(new Segment(b, d));
            Segment ac = (Segment)parser.Get(new Segment(a,c));

            given.Add(new Strengthened(quad, new Parallelogram(quad)));
            given.Add(new Perpendicular(parser.GetIntersection(ac, bd)));
            given.Add(new GeometricCongruentSegments(ac, bd));

            goals.Add(new Strengthened(quad, new Square(quad)));
        }
    }
}
