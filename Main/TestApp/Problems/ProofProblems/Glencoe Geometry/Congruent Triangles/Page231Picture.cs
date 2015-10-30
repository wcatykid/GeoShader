using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 231 picture
    //
    public class Page231Picture : ParallelLinesProblem
    {
        public Page231Picture(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 231 Picture";

            Point f = new Point("F", 2, Math.Sqrt(12.0)); points.Add(f);
            Point g = new Point("G", 6, Math.Sqrt(12.0)); points.Add(g);   // Incorrect placement for problem scenario
            Point h = new Point("H", 4, 0); points.Add(h);
            Point j = new Point("J", 0, 0); points.Add(j);
            
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment fh = new Segment(f, h); segments.Add(fh);
            Segment fj = new Segment(f, j); segments.Add(fj);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment hj = new Segment(h, j); segments.Add(hj);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(fj, fh));
            given.Add(new GeometricCongruentSegments(fg, gh));
            given.Add(new GeometricCongruentSegments(fh, fg));

            goals.Add(new GeometricCongruentTriangles(new Triangle(j, f, h), new Triangle(f, g, h)));
        }
    }
}