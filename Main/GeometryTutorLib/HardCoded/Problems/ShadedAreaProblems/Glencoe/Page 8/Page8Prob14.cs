using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Prob14 : ActualShadedAreaProblem
    {
        public Page8Prob14(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -1 * System.Math.Sqrt(50), -1 * System.Math.Sqrt(50)); points.Add(a);
            Point b = new Point("B", -1 * System.Math.Sqrt(50), System.Math.Sqrt(50)); points.Add(b);
            Point c = new Point("C", System.Math.Sqrt(50), System.Math.Sqrt(50)); points.Add(c);
            Point d = new Point("D", System.Math.Sqrt(50), -1 * System.Math.Sqrt(50)); points.Add(d);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);

            Point o = new Point("O", 0, 0); points.Add(o);

            //Segment od = new Segment(o, d); segments.Add(od);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(o);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            circles.Add(new Circle(o, 10));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(new Segment(b, d), 20);
            known.AddAngleMeasureDegree(new Angle(b, a, d), 90);

            Quadrilateral q = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, da));
            given.Add(new Strengthened(q, new Square(q)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", -8, 0));
            wanted.Add(new Point("", 8, 0));
            wanted.Add(new Point("", 0, -8));
            wanted.Add(new Point("", 0, 8));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(100*System.Math.PI - 200);

            problemName = "Glencoe Page 8 Problem 14";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}