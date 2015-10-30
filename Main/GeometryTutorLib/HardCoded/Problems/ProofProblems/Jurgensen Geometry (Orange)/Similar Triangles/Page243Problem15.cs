using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 243 Problem 15
    //
    public class Page243Problem15 : CongruentTrianglesProblem
    {
        public Page243Problem15(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 243 Problem 15";


            Point d = new Point("D", 4, 24); points.Add(d);
            Point f = new Point("F", 3, 18); points.Add(f);
            Point h = new Point("H", 1, 6); points.Add(h);
            Point e = new Point("E", 5, 24); points.Add(e);
            Point g = new Point("G", 8, 18); points.Add(g);
            Point j = new Point("J", 14, 6); points.Add(j);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment hj = new Segment(h, j); segments.Add(hj);

            List<Point> pts = new List<Point>();
            pts.Add(d);
            pts.Add(f);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(g);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(de), (Segment)parser.Get(fg)));
            given.Add(new GeometricParallel((Segment)parser.Get(hj), (Segment)parser.Get(fg)));
        }
    }
}