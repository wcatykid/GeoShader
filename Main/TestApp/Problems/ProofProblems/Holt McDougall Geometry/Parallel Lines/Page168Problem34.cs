using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 168 Problem 34
    //
    public class Page168Problem34 : ParallelLinesProblem
    {
        public Page168Problem34(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 168 Problem 34";


            Point a = new Point("A", 1, 7); points.Add(a);
            Point b = new Point("B", 1, 4); points.Add(b);
            Point c = new Point("C", 8, 0); points.Add(c);
            Point d = new Point("D", 8, 4); points.Add(d);
            Point e = new Point("E", 4, 4); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, e)),
                                                   (Angle)parser.Get(new Angle(a, e, b))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, e, c)),
                                                   (Angle)parser.Get(new Angle(e, c, d))));

            goals.Add(new GeometricParallel(ab, cd));
        }
    }
}