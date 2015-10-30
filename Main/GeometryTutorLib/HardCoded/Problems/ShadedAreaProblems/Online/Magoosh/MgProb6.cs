using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MgProb6 : ActualShadedAreaProblem
    {
        public MgProb6(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", 1, 0); points.Add(a);
            Point b = new Point("B", 2, 0); points.Add(b);
            Point c = new Point("C", 3, 0); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point e = new Point("E", 5, 0); points.Add(e);
            Point f = new Point("F", 6, 0); points.Add(f);

            List<Point> pnts = new List<Point>();
            pnts.Add(o);
            pnts.Add(a);
            pnts.Add(b);
            pnts.Add(c);
            pnts.Add(d);
            pnts.Add(e);
            pnts.Add(f);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(a, 1));
            circles.Add(new Circle(b, 2));
            circles.Add(new Circle(c, 3));
            circles.Add(new Circle(d, 4));
            circles.Add(new Circle(e, 5));
            circles.Add(new Circle(f, 6));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, a)), 1);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, b)), 2);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, c)), 3);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, d)), 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, e)), 5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, f)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 2, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(21 * System.Math.PI);

            problemName = "Magoosh Problem 6";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


