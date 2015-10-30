using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class JPage141Problem11 : SimilarTrianglesProblem
    {
        public JPage141Problem11(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 141 Problem 11";


            Point a = new Point("A", 2, 5);    points.Add(a);
            Point b = new Point("B", 0, 0);    points.Add(b);
            Point c = new Point("C", 4, 0);    points.Add(c);
            Point d = new Point("D", 2, 0);    points.Add(d);
            Point e = new Point("E", -3.25, 0);  points.Add(e);
            Point f = new Point("F", 3, 2.5);  points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment ef = new Segment(e, f); segments.Add(ef);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(b);
            pts.Add(d);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ab, (Segment)parser.Get(new Segment(a, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, d, c))));
            given.Add(new RightAngle((Angle)parser.Get(new Angle(e, f, c))));

            goals.Add(new GeometricSimilarTriangles(new Triangle(a, b, d), new Triangle(e, c, f)));
        }
    }
}