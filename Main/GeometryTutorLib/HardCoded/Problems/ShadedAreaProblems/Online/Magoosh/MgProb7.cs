using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MgProb7 : ActualShadedAreaProblem
    {
        public MgProb7(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double y = 4 * System.Math.Sqrt(3);
            Point m = new Point("M", 0, 0); points.Add(m);
            Point j = new Point("J", -4, 0); points.Add(j);
            Point k = new Point("K", 0, y); points.Add(k);
            Point l = new Point("L", 4, 0); points.Add(l);
            Point n = new Point("N", -2, y / 2); points.Add(n);
            Point o = new Point("O", 2, y / 2); points.Add(o);

            Segment mk = new Segment(m, k); segments.Add(mk);
            Segment mn = new Segment(m, n); segments.Add(mn);
            Segment mo = new Segment(m, o); segments.Add(mo);

            List<Point> pnts = new List<Point>();
            pnts.Add(j);
            pnts.Add(n);
            pnts.Add(k);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(k);
            pnts.Add(o);
            pnts.Add(l);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(j);
            pnts.Add(m);
            pnts.Add(l);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(m, 4);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Triangle tri = (Triangle)parser.Get(new Triangle(j, k, l));

            given.Add(new Strengthened(tri, new EquilateralTriangle(tri)));

            known.AddSegmentLength((Segment)parser.Get(new Segment(m, j)), 4);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0, -1));
            wanted.Add(new Point("", -3.9, 0.2));
            wanted.Add(new Point("", 3.9, 0.2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((40/3.0) * System.Math.PI - 4 * y);

            problemName = "Magoosh Problem 7";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

