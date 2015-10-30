using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page9Prob10 : ActualShadedAreaProblem
    {
        public Page9Prob10(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 15); points.Add(b);
            Point c = new Point("C", -8, 25); points.Add(c);
            Point d = new Point("D", 0, 25); points.Add(d);
            Point e = new Point("E", 12, 25); points.Add(e);
            Point f = new Point("F", 12, 0); points.Add(f);

            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fa = new Segment(f, a); segments.Add(fa);


            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            parser = new TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(c, e)), 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(b, d)), 10);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 15);
            known.AddSegmentLength(fa, 12);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)),
                ef, (Segment)parser.Get(new Segment(d, e)), fa));

            given.Add(new Strengthened(quad, new Rectangle(quad)));

            goalRegions = parser.implied.GetAllAtomicRegions();

            SetSolutionArea(340);

            problemName = "Glencoe Page 9 Problem 10";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}