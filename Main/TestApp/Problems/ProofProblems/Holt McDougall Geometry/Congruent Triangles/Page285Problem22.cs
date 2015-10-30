using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 285 problem 22
    //
    public class Page285Problem22 : CongruentTrianglesProblem
    {
        public Page285Problem22(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 285 Problem 22";


            Point f = new Point("F", 0, 5); points.Add(f);
            Point g = new Point("G", 2.4, 1.8); points.Add(g);
            Point h = new Point("H", 0, 0); points.Add(h);
            Point k = new Point("K", -2.4, 1.8); points.Add(k);
            
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment fh = new Segment(f, h); segments.Add(fh);
            Segment fk = new Segment(f, k); segments.Add(fk);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment hk = new Segment(h, k); segments.Add(hk);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(hk, gh));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(f, k, h))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(f, g, h))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(k, f, h)), (Angle)parser.Get(new Angle(g, f, h))));
        }
    }
}