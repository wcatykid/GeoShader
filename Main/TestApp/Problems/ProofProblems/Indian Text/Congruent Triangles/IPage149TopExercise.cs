using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Book I Page 149 Exercise at the top of the page; it is not numbered.
    //
    public class IPage149TopExercise : ActualProofProblem
    {
        public IPage149TopExercise(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 149 Exercise at Top of Page";


            Point a = new Point("A", 2, 4); points.Add(a);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);
            Point d = new Point("D", 5, 2); points.Add(d);
            Point e = new Point("E", 1, 2); points.Add(e);
            Point f = new Point("F", 3, 2); points.Add(f);
            Point m = new Point("M", 7, 6); points.Add(m);

            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( e, (Segment)parser.Get(new Segment(a, b))))));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(e, d)), bc));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(c, m)), (Segment)parser.Get(new Segment(b, a))));
        }
    }
}