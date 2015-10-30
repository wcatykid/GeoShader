using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 284 problem 18
    //
    public class Page284Problem18 : CongruentTrianglesProblem
    {
        public Page284Problem18(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 284 Problem 18";


            Point d = new Point("D", 0, 0);  points.Add(d);
            Point e = new Point("E", 6, -2); points.Add(e);
            Point f = new Point("F", 6, 0);  points.Add(f);
            Point g = new Point("G", 6, 2);  points.Add(g);
            Point h = new Point("H", 12, 0); points.Add(h);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment gh = new Segment(g, h); segments.Add(gh);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(f);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(f);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(de, gh));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(e, f)), (Segment)parser.Get(new Segment(f, g))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(e, f, d))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(g, f, h))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(e, f, d), new Triangle(g, f, h)));
        }
    }
}