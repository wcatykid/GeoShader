using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 243 Problem 16
    //
    public class Page243Problem16 : CongruentTrianglesProblem
    {
        public Page243Problem16(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 243 Problem 16";


            Point a = new Point("A", 1.6, 1.2); points.Add(a);
            Point b = new Point("B", 2, 6); points.Add(b);
            Point c = new Point("C", 17, 6); points.Add(c);
            Point d = new Point("D", 17.6, 13.2); points.Add(d);
            Point x = new Point("X", 8, 6); points.Add(x);
            
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(x);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
        }
    }
}