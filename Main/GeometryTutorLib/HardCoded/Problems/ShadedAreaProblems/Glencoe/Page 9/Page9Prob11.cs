using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob11 : ActualShadedAreaProblem
    {
        public Page9Prob11(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 54, 31); points.Add(o);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 62); points.Add(b);
            Point c = new Point("C", 54, 62); points.Add(c);
            Point d = new Point("D", 81, 31); points.Add(d);
            Point e = new Point("E", 54, 0); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment de = new Segment(d, e); segments.Add(de);
            Segment ea = new Segment(e, a); segments.Add(ea);
            Segment od = new Segment(o, d); segments.Add(od);


            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(o);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 62);
            known.AddSegmentLength(ea, 54);
            known.AddSegmentLength(od, 27);

            Segment ce = (Segment)parser.Get(new Segment(c, e));
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, ce, bc, ea));

            Intersection inter = parser.GetIntersection(od, ce);
            given.Add(new Strengthened(quad, new Rectangle(quad)));
            given.Add(new Strengthened(inter, new PerpendicularBisector(inter, od)));

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(4185);

            problemName = "Glencoe Page 9 Problem 11";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}