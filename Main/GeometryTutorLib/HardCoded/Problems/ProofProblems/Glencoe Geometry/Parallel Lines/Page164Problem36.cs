using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 164 problem 36
    //
    public class Page164Problem36 : ParallelLinesProblem
    {
        public Page164Problem36(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 164 Problem 36";

            Point a = new Point("A", 11, 8); points.Add(a);
            Point b = new Point("B", 10, 8); points.Add(b);
            Point c = new Point("C", 10, 11); points.Add(c);
            Point d = new Point("D", 0, 8);  points.Add(d);
            Point e = new Point("E", 0, 0);   points.Add(e);
            Point f = new Point("F", 10, 0);  points.Add(f);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment ef = new Segment(e, f); segments.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(b);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, d, e)), (Angle)parser.Get(new Angle(c, b, d))));

            goals.Add(new GeometricParallel(de, new Segment(f, c)));
        }
    }
}