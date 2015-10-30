using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 168 Problem 35
    //
    public class Page168Problem35 : CongruentTrianglesProblem
    {
        public Page168Problem35(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 168 Problem 35";


            Point a = new Point("A", 1, 0); points.Add(a);
            Point b = new Point("B", 5, 0); points.Add(b);
            Point c = new Point("C", 9, 0); points.Add(c);
            Point d = new Point("D", 0, 2); points.Add(d);
            Point e = new Point("E", 4, 2); points.Add(e);
            Point f = new Point("F", 3, 4); points.Add(f);
            Point g = new Point("G", 7, 4); points.Add(g);
            Point h = new Point("H", 6, 6); points.Add(h);

            Point i = new Point("I", 0, 10); points.Add(i);
            Point j = new Point("J", 4, 10); points.Add(j);
            Point k = new Point("K", 12, 10); points.Add(k);
            Point l = new Point("L", 16, 10); points.Add(l);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(g);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(f);
            pts.Add(h);
            pts.Add(k);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(e);
            pts.Add(f);
            pts.Add(i);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(g);
            pts.Add(h);
            pts.Add(j);
            collinear.Add(new Collinear(pts));
            
                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(f, h, g)),
                                                   (Angle)parser.Get(new Angle(f, e, g))));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, l)), (Segment)parser.Get(new Segment(d, k))));

            goals.Add(new GeometricParallel(new Segment(b, i), new Segment(c, j)));
        }
    }
}