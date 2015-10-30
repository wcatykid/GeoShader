using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 90 Problem 23
    //
    public class Page90Problem23 : CongruentTrianglesProblem
    {
        public Page90Problem23(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 90 Problem 23";


            Point e = new Point("E", 0, 0); points.Add(e);
            Point f = new Point("F", 4, -3); points.Add(f);
            Point g = new Point("G", 4, 3); points.Add(g);
            Point h = new Point("H", 7, 0); points.Add(h);
            Point j = new Point("J", 4, 0); points.Add(j);

            Segment eg = new Segment(e, g); segments.Add(eg);
            Segment gh = new Segment(g, h); segments.Add(gh);
            Segment hf = new Segment(h, f); segments.Add(hf);
            Segment fe = new Segment(f, e); segments.Add(fe);

            List<Point> pts = new List<Point>();
            pts.Add(e);
            pts.Add(j);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(j);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentTriangles(new Triangle(g, h, j), new Triangle(f, h, j)));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(e, f)), (Segment)parser.Get(new Segment(e, g))));
        }
    }
}