using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 62 Problems 1-6
    //
    // Parallel Transversals
    //
    public class Page60Theorem22 : ParallelLinesProblem
    {
        public Page60Theorem22(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 60 Theorem 22";


            Point a = new Point("A", -1, 3); points.Add(a);
            Point b = new Point("B", 4, 3); points.Add(b);
            Point c = new Point("C", 0, 0); points.Add(c);
            Point d = new Point("D", 5, 0); points.Add(d);

            Point x = new Point("X", 2, 3); points.Add(x);
            Point y = new Point("Y", 1, 0); points.Add(y);

            Segment xy = new Segment(x, y); segments.Add(xy);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(y);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, x, y)), (Angle)parser.Get(new Angle(x, y, d))));
        }
    }
}