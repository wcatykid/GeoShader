using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Book J: Page 152 Problem 1
    //
    public class JPage152Problem01 : SimilarTrianglesProblem
    {
        public JPage152Problem01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 152 Problem 1";


            Point p = new Point("P", 2.5, 3); points.Add(p);
            Point q = new Point("Q", 0, 0);   points.Add(q);
            Point s = new Point("S", 2.5, 0); points.Add(s);
            Point r = new Point("R", 5, 0);   points.Add(r);

            Segment pq = new Segment(p, q); segments.Add(pq);
            Segment ps = new Segment(p, s); segments.Add(ps);
            Segment pr = new Segment(p, r); segments.Add(pr);

            List<Point> pts = new List<Point>();
            pts.Add(q);
            pts.Add(s);
            pts.Add(r);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(q, p, r)), (Segment)parser.Get(new Segment(p, s))));
        }
    }
}