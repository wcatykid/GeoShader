using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page4Prob19 : ActualShadedAreaProblem
    {
        public Page4Prob19(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 3); points.Add(b);
            Point c = new Point("C", 0, 6); points.Add(c);
            Point d = new Point("D", 4, 3); points.Add(d);
            Point e = new Point("E", 8, 0); points.Add(e);
            Point f = new Point("F", 4, 0); points.Add(f);

            Point x = new Point("X", -3, 3); points.Add(x);
            Point y = new Point("Y", 8, 6); points.Add(y);
            Point z = new Point("Z", 4, -4); points.Add(z);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(f);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            Circle circleI = new Circle(b, 3);
            Circle circleII = new Circle(f, 4);
            Circle circleIII = new Circle(d, 5);

            semicircles.Add(new Semicircle(circleI, a, c, x, new Segment(a, c)));
            semicircles.Add(new Semicircle(circleII, a, e, z, new Segment(a, e)));
            semicircles.Add(new Semicircle(circleIII, c, e, y, new Segment(c, e)));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 6);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, e)), 8);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, -3));
            wanted.Add(new Point("", 7, 5));
            wanted.Add(new Point("", -1, 3));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(42.25 * System.Math.PI - 30);
        }
    }
}