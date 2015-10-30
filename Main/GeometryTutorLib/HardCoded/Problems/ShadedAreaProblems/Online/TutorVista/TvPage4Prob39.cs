using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage4Prob39 : ActualShadedAreaProblem
    {
        public TvPage4Prob39(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point d = new Point("D", 0, 0); points.Add(d);
            Point a = new Point("A", 0, 16); points.Add(a);
            Point b = new Point("B", 16, 16); points.Add(b);
            Point c = new Point("C", 16, 0); points.Add(c);
            Point o = new Point("O", 8, 8); points.Add(o);
            Point p = new Point("P", 4, 8); points.Add(p);

            Point e = new Point("E", 0, 4); points.Add(e);
            Point f = new Point("F", 0, 12); points.Add(f);
            Point g = new Point("G", 4, 16); points.Add(g);
            Point h = new Point("H", 12, 16); points.Add(h);
            Point i = new Point("I", 16, 12); points.Add(i);
            Point j = new Point("J", 16, 4); points.Add(j);
            Point k = new Point("K", 12, 0); points.Add(k);
            Point l = new Point("L", 4, 0); points.Add(l);

            Segment op = new Segment(o, p); segments.Add(op);

            List<Point> pnts = new List<Point>();
            pnts.Add(d);
            pnts.Add(e);
            pnts.Add(f);
            pnts.Add(a);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(g);
            pnts.Add(h);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(i);
            pnts.Add(j);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(k);
            pnts.Add(l);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle tL = new Circle(a, 4);
            Circle bL = new Circle(d, 4);
            Circle tR = new Circle(b, 4);
            Circle bR = new Circle(c, 4);
            Circle mid = new Circle(o, 4);

            circles.Add(tL);
            circles.Add(bL);
            circles.Add(tR);
            circles.Add(bR);
            circles.Add(mid);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(a, d)),
                (Segment)parser.Get(new Segment(b, c)), (Segment)parser.Get(new Segment(a, b)), (Segment)parser.Get(new Segment(c, d))));
            given.Add(new Strengthened(quad, new Square(quad)));
            given.Add(new GeometricCongruentCircles(tL, bL));
            given.Add(new GeometricCongruentCircles(tL, tR));
            given.Add(new GeometricCongruentCircles(tL, bR));
            given.Add(new GeometricCongruentCircles(tL, mid));

            known.AddSegmentLength((Segment)parser.Get(new Segment(o, p)), 4);
            known.AddSegmentLength((Segment)parser.Get(new Segment(d, a)), 16);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 5, 1));
            wanted.Add(new Point("", 5, 13));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(256 - 32 * System.Math.PI);

            problemName = "Tutor Vista Page 4 Problem 39";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


