using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    class KiteProblem02 : QuadrilateralsProblem
    {
        //Demonstrates usage of definition of kite

        public KiteProblem02(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Kite Problem 02";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 3, 3); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 3, -4); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);


            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, ad));
            given.Add(new Strengthened(quad, new Kite(quad)));

            goals.Add(new GeometricCongruentSegments(ab, bc));
            goals.Add(new GeometricCongruentSegments(cd, ad));
        }
    }
}
