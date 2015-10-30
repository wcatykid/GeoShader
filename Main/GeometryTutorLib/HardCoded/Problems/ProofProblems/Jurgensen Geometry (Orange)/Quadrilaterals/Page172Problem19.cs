using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class Page172Problem19 : QuadrilateralsProblem
    {
        // Geometry; Page 172 Problem 19
        // Demonstrates: "Each diagonal of a rhombus bisects two angles of the rhombus" (Jurgensen 4-11)
        // Will run successfully, but requires a very long run time. Hypergraph will have over 3000 nodes


        public Page172Problem19(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 2, 4); points.Add(c);
            Point d = new Point("D", -3, 4); points.Add(d);
            Point e = new Point("E", -1, 0); points.Add(e);
            Point f = new Point("F", -1, -2); points.Add(f);
            Point g = new Point("G", 3, -4); points.Add(g);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment bd = new Segment(b, d); segments.Add(bd);

            List<Point> pnts = new List<Point>();
            pnts.Add(f);
            pnts.Add(a);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            List<Point> pnts2 = new List<Point>();
            pnts2.Add(g);
            pnts2.Add(a);
            pnts2.Add(d);
            collinear.Add(new Collinear(pnts2));

            List<Point> pnts3 = new List<Point>();
            pnts3.Add(e);
            pnts3.Add(a);
            pnts3.Add(b);
            collinear.Add(new Collinear(pnts3));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)),
                bc, cd, (Segment)parser.Get(new Segment(a, b))));

            given.Add(new Strengthened(quad, new Rhombus(quad)));

            //To test creating a new Rhombus from individual segments rather than a quad. The diagonal properties must be specified
            //given.Add(new Strengthened(quad, new Rhombus((Segment)parser.Get(new Segment(a, d)),
            //    bc, cd, (Segment)parser.Get(new Segment(a, b)), quad.TopLeftDiagonalIsValid(), 
            //    quad.BottomRightDiagonalIsValid(), quad.diagonalIntersection)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(e, a, f)), (Angle)parser.Get(new Angle(g, a, f))));
        }
    }
}
