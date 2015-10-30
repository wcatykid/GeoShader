using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Book I: Page 123 Example 5
    //
    public class IPage123Example5 : CongruentTrianglesProblem
    {
        public IPage123Example5(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book I Page 123 Example 5";


            Point a = new Point("A", 3, 4); points.Add(a);
            Point e = new Point("E", 1.5, 2); points.Add(e);
            Point f = new Point("F", 4.5, 2); points.Add(f);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);

            Point x = new Point("X", 3, 4.0 / 3.0); points.Add(x);

            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(x);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(f);
            pts.Add(x);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( e, (Segment)parser.Get(new Segment(a, b))))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( f, (Segment)parser.Get(new Segment(a, c))))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a,b)), (Segment)parser.Get(new Segment(a, c))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, f)), (Segment)parser.Get(new Segment(c, e))));
        }
    }
}