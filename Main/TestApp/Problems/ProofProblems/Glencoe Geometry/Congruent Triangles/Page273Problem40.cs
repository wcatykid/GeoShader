using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 273 problem 40
    //
    public class Page273Problem40 : CongruentTrianglesProblem
    {
        public Page273Problem40(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 273 Problem 40";

            Point l = new Point("L", 6, 0);               points.Add(l);
            Point m = new Point("M", 0, 0);               points.Add(m);
            Point n = new Point("N", 3, Math.Sqrt(27.0)); points.Add(n);
            Point o = new Point("O", Math.Sqrt(12.0), 2); points.Add(o);
            
            Segment lm = new Segment(l, m); segments.Add(lm);
            Segment lo = new Segment(l, o); segments.Add(lo);
            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment mo = new Segment(m, o); segments.Add(mo);
            Segment no = new Segment(n, o); segments.Add(no);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(l, m, n)), (Segment)parser.Get(new Segment(o, m))));
            given.Add(new GeometricCongruentSegments(lm, mn));

            goals.Add(new GeometricCongruentTriangles(new Triangle(m, o, l), new Triangle(m, o, n)));
        }
    }
}