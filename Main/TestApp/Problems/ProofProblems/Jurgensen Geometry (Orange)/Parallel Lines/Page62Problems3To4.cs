﻿using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 62 Problems 3-4
    //
    // Parallel Transversals
    //
    public class Page62Problems3To4 : ParallelLinesProblem
    {
        public Page62Problems3To4(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 62 Problems 3-4";

            Point a = new Point("A", -1, 3); points.Add(a);
            Point b = new Point("B", 4, 3);  points.Add(b);
            Point c = new Point("C", 0, 0);  points.Add(c);
            Point d = new Point("D", 5, 0);  points.Add(d);

            Point e = new Point("E", -3, 3);  points.Add(e);
            Point f = new Point("F", -2, 6); points.Add(f);
            Point g = new Point("G", 3, 6);  points.Add(g);
            Point h = new Point("H", 8, 3);  points.Add(h);
            Point i = new Point("I", 10, 0); points.Add(i);
            Point j = new Point("J", 7, -6);  points.Add(j);
            Point k = new Point("K", 1, -3);  points.Add(k);
            Point l = new Point("L", -5, 0); points.Add(l);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(a);
            pts.Add(c);
            pts.Add(k);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(a);
            pts.Add(b);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(b);
            pts.Add(d);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(c);
            pts.Add(d);
            pts.Add(i);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(a, c)), (Segment)parser.Get(new Segment(b, d))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(b, a, f))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(c, a, e))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(d, b, a))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(a, c, d))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(b, d, i))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(l, c, k))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(c, d, j))));

            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(f, a, e))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(g, b, a))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(c, a, b))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(d, b, h))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(l, c, a))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(b, d, c))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(k, c, d))));
            goals.Add(new Supplementary((Angle)parser.Get(new Angle(h, b, g)), (Angle)parser.Get(new Angle(j, d, i))));
        }
    }
}