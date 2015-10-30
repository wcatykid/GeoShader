using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 286 problem 9
    //
    public class Page286Problem9 : CongruentTrianglesProblem
    {
        public Page286Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 286 Problem 9";


            Point f = new Point("F", 0, 0); points.Add(f);
            Point g = new Point("G", 5, 5.0 * Math.Sqrt(3.0)); points.Add(g);
            Point h = new Point("H", 10, 0); points.Add(h);
            Point j = new Point("J", 15, 5.0 * Math.Sqrt(3.0)); points.Add(j);
            Point k = new Point("K", 20, 0); points.Add(k);
            Point l = new Point("L", 25, 5.0 * Math.Sqrt(3.0)); points.Add(l);

            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment fh = new Segment(f, h); segments.Add(fh);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment jk = new Segment(j, k); segments.Add(jk);
            Segment jl = new Segment(j, l); segments.Add(jl);
            Segment kl = new Segment(k, l); segments.Add(kl);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(fg, fh));
            given.Add(new GeometricCongruentSegments(fg, gh));
            given.Add(new GeometricCongruentSegments(fg, jk));
            given.Add(new GeometricCongruentSegments(fg, jl));
            given.Add(new GeometricCongruentSegments(fg, kl));
            
            goals.Add(new GeometricCongruentTriangles(new Triangle(f, g, h), new Triangle(j, k, l)));
        }
    }
}