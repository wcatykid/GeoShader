using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
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

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment af = new Segment(a, f); segments.Add(af);
            Segment be = new Segment(b, e); segments.Add(be);

            Point x = new Point("X", 10, 4); points.Add(x);

            circles.Add(new Circle(x, 4.0));
           // Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ac, cd, df, fa));

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts.Add(d);
            pts.Add(e);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new RightAngle((Angle)parser.Get(new Angle(a, c, d))));
            given.Add(new GeometricParallel(cd, be));
            given.Add(new GeometricParallel(cd, af));

            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(d, e)), (Segment)parser.Get(new Segment(e, f))));

            known.AddSegmentLength(af, 16);
            known.AddSegmentLength(cd, 20);
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