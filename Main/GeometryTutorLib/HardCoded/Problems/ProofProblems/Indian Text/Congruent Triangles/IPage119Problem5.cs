using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Book i Page 119 Problem 5
    //
    public class IPage119Problem5 : ActualProofProblem
    {
        public IPage119Problem5(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "I Page 119 Problem 5";

            Point a = new Point("A", 0, 0);  points.Add(a);
            Point b = new Point("B", 10, 0); points.Add(b);
            Point p = new Point("P", 8, -4); points.Add(p);
            Point q = new Point("Q", 8, 4);  points.Add(q);

            Point x = new Point("X", 16, 8);    points.Add(x);
            Point y = new Point("Y", 16, 0);   points.Add(y);
            Point z = new Point("Z", 24, -12);   points.Add(z);
            Point o = new Point("O", -100, 0); points.Add(o);

            Segment bq = new Segment(b, q); segments.Add(bq);
            Segment bp = new Segment(b, p); segments.Add(bp);

            List<Point> pts = new List<Point>();
            pts.Add(o);
            pts.Add(a);
            pts.Add(b);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(q);
            pts.Add(x);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(p);
            pts.Add(z);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(q, a, p)), (Segment)parser.Get(new Segment(o, y))));
            given.Add(new Perpendicular(parser.GetIntersection(new Segment(b, p), new Segment(a, z))));
            given.Add(new Perpendicular(parser.GetIntersection(new Segment(b, q), new Segment(a, x))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(a, p, b), new Triangle(a, q, b)));
            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, p)), (Segment)parser.Get(new Segment(b, q))));
        }
    }
}