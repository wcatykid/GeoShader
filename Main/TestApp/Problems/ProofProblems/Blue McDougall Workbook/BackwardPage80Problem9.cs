using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 80 Problem 9
    //
    public class BackwardPage80Problem9 : CongruentTrianglesProblem
    {
        public BackwardPage80Problem9(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 80 Problem 9";

            Point g = new Point("G", 0, 0); points.Add(g);
            Point h = new Point("H", 3, 0); points.Add(h);
            Point f = new Point("F", 10, 15); points.Add(f);
            Point i = new Point("I", 17, 0); points.Add(i);
            Point j = new Point("J", 20, 0); points.Add(j);

            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment fh = new Segment(f, h); segments.Add(fh);
            Segment fi = new Segment(f, i); segments.Add(fi);
            Segment fj = new Segment(f, j); segments.Add(fj);

            List<Point> pts = new List<Point>();
            pts.Add(g);
            pts.Add(h);
            pts.Add(i);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(fh, fi));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(h, g)), (Segment)parser.Get(new Segment(i, j))));

            goals.Add(new GeometricCongruentSegments(fg, fj));

        }
    }
}