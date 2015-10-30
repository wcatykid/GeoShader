using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
    //
    // Geometry; Midpoint Theorem for testing precomputation of segment bisector, etc.
    //
    public class SegmentBisectorTester: TransversalsProblem
    {
        public SegmentBisectorTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -3, 0);   points.Add(a);
            Point m = new Point("M", 0, 0);  points.Add(m);
            Point b = new Point("B", 3, 0);   points.Add(b);

            Point d = new Point("D", 12, 12); points.Add(d);
            Segment dm = new Segment(d, m); segments.Add(dm);

            Point q = new Point("Q", 0, 18); points.Add(q);
            Segment qm = new Segment(q, m); segments.Add(qm);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(m);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( m, (Segment)parser.Get(new Segment(a, b))))));
        }
    }
}