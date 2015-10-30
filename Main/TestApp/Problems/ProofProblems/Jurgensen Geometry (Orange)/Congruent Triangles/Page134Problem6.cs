using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 134 #6
    //
    public class Page134Problem6 : CongruentTrianglesProblem
    {
        public Page134Problem6(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 134 Problem 6";


            Point l = new Point("L", -3, 4); points.Add(l);
            Point o = new Point("O", 1.5, 1); points.Add(o);
            Point j = new Point("J", 6, 4); points.Add(j);
            Point m = new Point("M", 0, 0); points.Add(m);
            Point n = new Point("N", 3, 0); points.Add(n);

            Segment lm = new Segment(l, m); segments.Add(lm);
            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment nj = new Segment(n, j); segments.Add(nj);

            List<Point> pts = new List<Point>();
            pts.Add(l);
            pts.Add(o);
            pts.Add(n);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(m);
            pts.Add(o);
            pts.Add(j);
            collinear.Add(new Collinear(pts));
            
            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(o, m, n)),
                                                   (Angle)parser.Get(new Angle(o, n, m))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(l, m, o)),
                                                   (Angle)parser.Get(new Angle(j, n, o))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(m, j)),
                                                     (Segment)parser.Get(new Segment(n, l))));
        }
    }
}