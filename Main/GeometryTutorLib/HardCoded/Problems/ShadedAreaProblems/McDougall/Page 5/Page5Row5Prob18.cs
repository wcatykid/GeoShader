using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row5Prob18 : ActualShadedAreaProblem
    {

        public Page5Row5Prob18(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 4); points.Add(b);
            Point c = new Point("C", 0, 8); points.Add(c);
            Point d = new Point("D", 20, 8); points.Add(d);
            Point e = new Point("E", 18, 4); points.Add(e);
            Point f = new Point("F", 16, 0); points.Add(f);
            Point g = new Point("G", 16, 8); points.Add(g);
            Point x = new Point("X", 8, 4); points.Add(x);
            Point y = new Point("Y", 8, 0); points.Add(y);

            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment fg = new Segment(f, g); segments.Add(fg);

            Circle circle = new Circle(x, 4.0);
            circles.Add(circle);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(g);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(e);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(x);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(y);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle angle = (Angle)parser.Get(new Angle(a, c, d));
            Triangle tri = (Triangle)parser.Get(new Triangle(f, g, d));
            CircleSegmentIntersection inter = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(y, circle, (Segment)parser.Get(new Segment(a, f))));
            given.Add(new Strengthened(angle, new RightAngle(angle)));
            given.Add(new Strengthened(inter, new Tangent(inter)));
            given.Add(new Strengthened(tri, new RightTriangle(tri)));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(b, e))));
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(c, d)), (Segment)parser.Get(new Segment(a, f))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(d, e)), (Segment)parser.Get(new Segment(e, f))));

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, f)), 16);
            known.AddSegmentLength((Segment)parser.Get(new Segment(c, d)), 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", 15, 1));
            wanted.Add(new Point("", 3, 7));
            wanted.Add(new Point("", 10, 5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(144-System.Math.PI*8);
        }
    }
}