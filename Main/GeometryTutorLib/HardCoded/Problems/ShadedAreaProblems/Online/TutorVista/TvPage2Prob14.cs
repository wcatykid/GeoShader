using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage2Prob14 : ActualShadedAreaProblem
    {
        public TvPage2Prob14(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double y = 8 * System.Math.Sqrt(5);
            Point a = new Point("A", 16, 0); points.Add(a);
            Point b = new Point("B", -16, 0); points.Add(b);
            Point c = new Point("C", -8, 0); points.Add(c);
            Point d = new Point("D", -8, -y); points.Add(d);
            Point e = new Point("E", -8, y); points.Add(e);
            Point o = new Point("O", 0, 0); points.Add(o);
            Point p = new Point("P", -12, 0); points.Add(p);

            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment ad = new Segment(a, d); segments.Add(ad);
            Segment oe = new Segment(o, e); segments.Add(oe);
            Segment od = new Segment(o, d); segments.Add(od);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(p);
            pnts.Add(c);
            pnts.Add(o);
            pnts.Add(a);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(e);
            pnts.Add(c);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle small = new Circle(p, 4);
            Circle big = new Circle(o, 16);
            circles.Add(small);
            circles.Add(big);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Intersection inter = (Intersection)parser.Get(new Intersection(c, (Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(e, d))));
            given.Add(new Strengthened(inter, new Perpendicular(inter)));

            known.AddSegmentLength(oe, 16);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, p)), 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -3, 1));
            wanted.Add(new Point("", -3, -1));
            wanted.Add(new Point("", 1, 1));
            wanted.Add(new Point("", 1, -1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(429.3250517);

            problemName = "Tutor Vista Page 2 Problem 14";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


