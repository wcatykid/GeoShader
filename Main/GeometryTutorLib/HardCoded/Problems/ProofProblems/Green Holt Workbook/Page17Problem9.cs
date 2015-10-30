using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 17 Problem 9
    //
    public class Page17Problem9 : TransversalsProblem
    {
        public Page17Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 17 Problem 9";


            Point h = new Point("H", 0, 0); points.Add(h);
            Point i = new Point("I", 2, 0); points.Add(i);
            Point j = new Point("J", 1.5, 2); points.Add(j);
            Point k = new Point("K", 4.5, 2); points.Add(k);

            Point x = new Point("X", 1, 0); points.Add(x);

            Point a = new Point("A", 3, 8); points.Add(a);
            Point m = new Point("M", 4, 0); points.Add(m);
            Point n = new Point("N", 5, 0); points.Add(n);
            Point p = new Point("P", 6, 0); points.Add(p);

            Segment jk = new Segment(j, k); segments.Add(jk);

            List<Point> pts = new List<Point>();
            pts.Add(h);
            pts.Add(x);
            pts.Add(i);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(j);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(k);
            pts.Add(n);
            collinear.Add(new Collinear(pts));
            
            pts = new List<Point>();
            pts.Add(m);
            pts.Add(n);
            pts.Add(p);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Supplementary((Angle)parser.Get(new Angle(h, x, j)), (Angle)parser.Get(new Angle(a, j, k))));

            goals.Add(new GeometricParallel((Segment)parser.Get(new Segment(h, i)), (Segment)parser.Get(new Segment(j, k))));
        }
    }
}