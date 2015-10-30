using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 160 Problem 43
    //
    public class Page160Problem43 : ParallelLinesProblem
    {
        public Page160Problem43(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 160 Problem 43";


            Point t = new Point("T", 15.0 - 5.0 * Math.Sqrt(3.0), 0); points.Add(t);
            Point s = new Point("S", 10, 0); points.Add(s);
            Point f = new Point("F", 15, 5.0 * Math.Sqrt(3.0)); points.Add(f); // This is the wrong coordinate pair......
            Point e = new Point("E", 15, 5.0 * Math.Sqrt(3.0)); points.Add(e);
            Point r = new Point("R", 20, 0); points.Add(r);

            Segment er = new Segment(e, r); segments.Add(er);
            Segment fs = new Segment(f, s); segments.Add(fs);
            Segment se = new Segment(s, e); segments.Add(se);

            List<Point> pts = new List<Point>();
            pts.Add(t);
            pts.Add(s);
            pts.Add(r);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(t);
            pts.Add(f);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(s, e, r)), (Angle)parser.Get(new Angle(s, r, e))));
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(r, s, f)), se));

            //goals.Add(new GeometricAngleEquation((Angle)parser.Get(new Angle(f, s, t)), new NumericValue(60))); 
        }
    }
}