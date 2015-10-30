using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 226 problem 42
    //
    public class Page226Problem42 : ParallelLinesProblem
    {
        public Page226Problem42(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 226 Problem 42";

            Point j = new Point("J", 0, 0); points.Add(j);
            Point k = new Point("K", 0, 12); points.Add(k);
            Point l = new Point("L", 3, 2); points.Add(l);
            Point m = new Point("M", 3, 10); points.Add(m);
            Point n = new Point("N", 9, 6); points.Add(n);

            Segment jk = new Segment(j, k); segments.Add(jk);
            Segment lm = new Segment(l, m); segments.Add(lm);

            List<Point> pts = new List<Point>();
            pts.Add(j);
            pts.Add(l);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(m);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new IsoscelesTriangle((Segment)parser.Get(new Segment(k, n)), (Segment)parser.Get(new Segment(j, n)), jk));
            given.Add(new GeometricParallel(jk, lm));

            goals.Add(new Strengthened((Triangle)parser.Get(new Triangle(n, m, l)),  new IsoscelesTriangle((Triangle)parser.Get(new Triangle(n, m, l)))));
        }
    }
}