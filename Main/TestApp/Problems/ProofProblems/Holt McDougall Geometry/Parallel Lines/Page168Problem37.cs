using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 168 Problem 37
    //
    public class Page168Problem37 : ParallelLinesProblem
    {
        public Page168Problem37(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 168 Problem 37";


            Point a = new Point("A", 1, 1); points.Add(a);
            Point b = new Point("B", 6, 1); points.Add(b);
            Point c = new Point("C", 10, 1); points.Add(c);
            Point d = new Point("D", 1, 3); points.Add(d);
            Point e = new Point("E", 2, 3); points.Add(e);
            Point f = new Point("F", 10, 3); points.Add(f);
            Point g = new Point("G", 0, 4); points.Add(g);
            Point h = new Point("H", 8, 0); points.Add(h);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(e);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            List<Point> pts3 = new List<Point>();
            pts.Add(g);
            pts.Add(e);
            pts.Add(b);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Supplementary((Angle)parser.Get(new Angle(d, e, b)), (Angle)parser.Get(new Angle(a, b, e))));
            
            goals.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(d, f))));
        }
    }
}