using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Book J: Page 153 Problem 9
    //
    public class JPage153Problem09 : SimilarTrianglesProblem
    {
        public JPage153Problem09(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 153 Problem 9";


            Point a = new Point("A", 2, 3); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 5, 0); points.Add(c);
            Point d = new Point("D", 2.5, 0); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ac = new Segment(a, c); segments.Add(ac);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            // given.Add(new GeometricProportionalSegments((Segment)parser.Get(new Segment(b, d)), (Segment)parser.Get(new Segment(c, d))));
            // given.Add(new GeometricProportionalSegments(ab, ac));
        }
    }
}