using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 316 problem 43
    //
    public class Page316Problem43 : CongruentTrianglesProblem
    {
        public Page316Problem43(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 316 Problem 43";


            Point f = new Point("F", 0, 5); points.Add(f);
            Point g = new Point("G", 10, 0); points.Add(g);
            Point h = new Point("H", 0, -5); points.Add(h);
            Point j = new Point("J", 5, 0); points.Add(j);

            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment fj = new Segment(f, j); segments.Add(fj);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment gj = new Segment(g, j); segments.Add(gj);
            Segment hj = new Segment(h, j); segments.Add(hj);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(fj, hj));
            given.Add(new GeometricCongruentSegments(fg, gh));

            goals.Add(new AngleBisector((Angle)parser.Get(new Angle(f, g, h)), gj));
        }
    }
}