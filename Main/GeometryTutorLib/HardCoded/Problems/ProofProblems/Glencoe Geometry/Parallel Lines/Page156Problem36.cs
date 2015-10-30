using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 156 problem 36
    //
    public class Page156Problem36 : ParallelLinesProblem
    {
        public Page156Problem36(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 156 Problem 36";

            Point j = new Point("J", 0, 10);  points.Add(j);
            Point k = new Point("K", 10, 10); points.Add(k);
            Point l = new Point("L", 20, 10); points.Add(l);
            Point m = new Point("M", 5, 0);   points.Add(m);
            Point n = new Point("N", 15, 0);  points.Add(n);

            Segment jm = new Segment(j, m); segments.Add(jm);
            Segment km = new Segment(k, m); segments.Add(km);
            Segment kn = new Segment(k, n); segments.Add(kn);
            Segment ln = new Segment(l, n); segments.Add(ln);

            List<Point> pts = new List<Point>();
            pts.Add(j);
            pts.Add(k);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(m, j, k)), (Angle)parser.Get(new Angle(m, k, j))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(n, k, l)), (Angle)parser.Get(new Angle(n, l, k))));
            given.Add(new GeometricParallel(jm, kn));

            goals.Add(new GeometricParallel(km, ln));
        }
    }
}