using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 226 problem 44
    //
    public class Page226Problem44 : CongruentTrianglesProblem
    {
        public Page226Problem44(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 226 Problem 44";

            Point v = new Point("V", 4, 12); points.Add(v);
            Point w = new Point("W", 12, 6); points.Add(w);
            Point x = new Point("X", 8, 4); points.Add(x);
            Point y = new Point("Y", 10, 0); points.Add(y);
            Point z = new Point("Z", 0, 0); points.Add(z);

            Segment vw = new Segment(v, w); segments.Add(vw);
            Segment yz = new Segment(y, z); segments.Add(yz);

            List<Point> pts = new List<Point>();
            pts.Add(z);
            pts.Add(x);
            pts.Add(w);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(v);
            pts.Add(x);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(x, v, w)), (Angle)parser.Get(new Angle(x, z, y))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(w, x)), (Segment)parser.Get(new Segment(x, y))));

            goals.Add(new GeometricCongruentSegments(vw, yz));
        }
    }
}