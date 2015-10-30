using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page3Prob24 : ActualShadedAreaProblem
    {
        public Page3Prob24(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 2, 0); points.Add(b);
            Point c = new Point("C", 6, 0); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            Point o = new Point("O", 4, 4); points.Add(o);
            Point e = new Point("E", 0, 4); points.Add(e);
            Point f = new Point("F", 2, 4); points.Add(f);
            Point g = new Point("G", 6, 4); points.Add(g);
            Point h = new Point("H", 8, 0); points.Add(h);


            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment bf = new Segment(b, f); segments.Add(bf);
            Segment cg = new Segment(c, g); segments.Add(cg);
            Segment dh = new Segment(d, h); segments.Add(dh);

            circles.Add(new Circle(o, 2));
            circles.Add(new Circle(o, 4));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(bf, 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, h)), 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, f)), 2);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 7));
            wanted.Add(new Point("", 1, 3));
            wanted.Add(new Point("", 7, 3));
            wanted.Add(new Point("", 0.5, 0.5));
            wanted.Add(new Point("", 7.5, 0.5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(4 * (4 + (3 / 2) * System.Math.PI));
        }
    }
}