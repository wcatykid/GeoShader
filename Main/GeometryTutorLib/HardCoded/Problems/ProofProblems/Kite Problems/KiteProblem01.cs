using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class KiteProblem01 : QuadrilateralsProblem
    {
        //Demonstrates usage of definition of kite

        public KiteProblem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Kite Problem 01";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 3, 3); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 3, -4); points.Add(d);
            Point m = new Point("M", 3, 0); points.Add(m);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(m);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(m);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);


            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, ad));
            
            given.Add(new GeometricCongruentSegments(ab, bc));
            given.Add(new GeometricCongruentSegments(cd, ad));

            goals.Add(new Strengthened(quad, new Kite(quad)));
        }
    }
}
