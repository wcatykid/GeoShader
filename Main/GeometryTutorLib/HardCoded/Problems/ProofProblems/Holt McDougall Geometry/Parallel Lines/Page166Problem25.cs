using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 166 Problem 25
    //
    public class Page166Problem25 : ParallelLinesProblem
    {
        public Page166Problem25(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 166 Problem 25";


            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 2, 4); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 8, 4); points.Add(d);
            Point e = new Point("E", 1, 2); points.Add(e);
            Point f = new Point("F", 7, 2); points.Add(f);
            Point g = new Point("G", 0, 2); points.Add(g);
            Point h = new Point("H", 10, 2); points.Add(h);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(f);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(e);
            pts.Add(f);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Supplementary((Angle)parser.Get(new Angle(b, e, g)), (Angle)parser.Get(new Angle(e, f, c))));

            goals.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));
        }
    }
}