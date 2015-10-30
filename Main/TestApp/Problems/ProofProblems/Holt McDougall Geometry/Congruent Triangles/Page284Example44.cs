using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 284 Example 4.4
    //
    public class Page284Example44 : CongruentTrianglesProblem
    {
        public Page284Example44(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 284 Example 4.4";


            Point d = new Point("D", 0, 4); points.Add(d);
            Point e = new Point("E", 3, 0); points.Add(e);
            Point f = new Point("F", 6, 0); points.Add(f);
            Point g = new Point("G", 12, 4); points.Add(g);
            Point h = new Point("H", 9, 0); points.Add(h);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment df = new Segment(d, f); segments.Add(df);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment fg = new Segment(f, g); segments.Add(fg);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(f);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(f, e)), (Segment)parser.Get(new Segment(f, h))));
            given.Add(new GeometricCongruentSegments(de, gh));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, e, f)), (Angle)parser.Get(new Angle(g, h, f))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(d, e, f), new Triangle(g, h, f)));
        }
    }
}