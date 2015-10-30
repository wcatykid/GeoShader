using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MgProb2 : ActualShadedAreaProblem
    {
        public MgProb2(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double r = 2 * System.Math.Sqrt(3);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 4, 2 * r); points.Add(b);
            Point c = new Point("C", 8, 0); points.Add(c);
            Point d = new Point("D", 4, 0); points.Add(d);
            Point e = new Point("E", 4, r); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(d);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(e);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(e, r);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ad = (Segment)parser.Get(new Segment(a, d));
            InMiddle mid = (InMiddle)parser.Get(new InMiddle(d, (Segment)parser.Get(new Segment(a, c))));
            Angle a1 = (Angle)parser.Get(new Angle(a, d, b));
            Triangle tri = (Triangle)parser.Get(new Triangle(a, b, c));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(tri, new EquilateralTriangle(tri)));
            given.Add(new Strengthened(mid, new Midpoint(mid)));

            known.AddSegmentLength(ad, 4);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0.1, 0.1));
            unwanted.Add(new Point("", 7.9, 0.1));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(r * r * System.Math.PI);

            problemName = "Magoosh Problem 2";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

