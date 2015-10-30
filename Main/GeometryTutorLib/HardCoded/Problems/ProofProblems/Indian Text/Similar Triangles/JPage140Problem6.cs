using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class JPage140Problem6 : SimilarTrianglesProblem
    {
            //       /\
            //      /  \
            //     /____\
            //    / \  / \
            //   /________\

            //
            //
        public JPage140Problem6(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Book J Page 140 Problem 6";


            Point a = new Point("A", 2, 12);       points.Add(a);
            Point d = new Point("D", 1.5, 9);      points.Add(d);
            Point e = new Point("E", 2.5, 9);      points.Add(e);
            Point x = new Point("X", 2, 18 / 2.5); points.Add(x);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 4, 0); points.Add(c);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(x);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(x);
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentTriangles(new Triangle(a, b, e), new Triangle(a, c, d)));

            goals.Add(new GeometricSimilarTriangles((Triangle)parser.Get(new Triangle(a, d, e)), (Triangle)parser.Get(new Triangle(a, b, c))));
        }
    }
}