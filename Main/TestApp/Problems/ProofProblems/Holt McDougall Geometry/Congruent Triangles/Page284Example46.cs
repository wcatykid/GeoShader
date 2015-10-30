using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 284 example 4.6
    //
    public class Page284Example46 : CongruentTrianglesProblem
    {
        public Page284Example46(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 284 example 4.6";


            Point e = new Point("E", 1, 0); points.Add(e);
            Point f = new Point("F", 0, 8); points.Add(f);
            Point g = new Point("G", 5, 3); points.Add(g);
            Point h = new Point("H", 9, 6); points.Add(h);
            Point j = new Point("J", 10, -2); points.Add(j);

            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment hj = new Segment(h, j); segments.Add(hj);

            List<Point> pts = new List<Point>();
            pts.Add(f);
            pts.Add(g);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(g);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(g, f)), (Segment)parser.Get(new Segment(j, g))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(e, g)), (Segment)parser.Get(new Segment(h, g))));

            goals.Add(new GeometricCongruentSegments(ef, hj));
        }
    }
}