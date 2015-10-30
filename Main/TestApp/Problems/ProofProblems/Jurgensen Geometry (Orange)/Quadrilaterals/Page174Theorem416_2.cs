using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page174Theorem416_2 : QuadrilateralsProblem
    {
        // Demonstration of theorem "Median of a trapezoid is half the sum of the bases"

        /*
         *         D_______E
         *         /       \
         * L______/M________\N
         *       /           \
         *      /_____________\
         *     A               B
         */

        public Page174Theorem416_2(bool onoff, bool complete)
            : base(onoff, complete)
        {
            problemName = "Page 174 Theorem 4-16 Part 2";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 10, 0); points.Add(b);
            Point c = new Point("C", 8, 4); points.Add(c);
            Point d = new Point("D", 2, 4); points.Add(d);

            Point l = new Point("L", -7, 2); points.Add(l);
            Point m = new Point("M", 1, 2); points.Add(m);
            Point n = new Point("N", 9, 2); points.Add(n);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(n);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(m);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ad = (Segment)parser.Get(new Segment(a, d));
            Segment bc = (Segment)parser.Get(new Segment(b, c));
            Segment ln = (Segment)parser.Get(new Segment(l, n));
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, cd, ab));
            Addition sum = new Addition(ab, cd);

            //If segment MN is the median of the trapezoid, and segment LN is congruent to the sum of AB and CD, then 
            //segment AD must bisect segment LN

            given.Add(new Strengthened(quad, new Trapezoid(quad)));
            given.Add(new SegmentBisector(parser.GetIntersection(ln, ad), ln));
            given.Add(new SegmentBisector(parser.GetIntersection(ln, bc), ln));
            given.Add(new GeometricSegmentEquation(ln, sum));

            goals.Add(new SegmentBisector(parser.GetIntersection(ad, ln), ad));

        }
    }
}
