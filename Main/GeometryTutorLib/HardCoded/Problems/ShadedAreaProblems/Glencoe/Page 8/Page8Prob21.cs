using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Prob21 : ActualShadedAreaProblem
    {
        public Page8Prob21(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double r = 10 / Math.Sqrt(2);
            Point a = new Point("A", -r, r); points.Add(a);
            Point b = new Point("B", r, r); points.Add(b);
            Point c = new Point("C", r, -r); points.Add(c);
            Point d = new Point("D", -r, -r); points.Add(d);
            Point e = new Point("E", 5, 5); points.Add(e);

            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ad = new Segment(a, d); segments.Add(ad);

            Circle outer = new Circle(o, 10);
            Circle inner = new Circle(o, r);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(e);
            pnts.Add(o);
            collinear.Add(new Collinear(pnts));

            circles.Add(outer);
            circles.Add(inner);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral q = (Quadrilateral)parser.Get(new Quadrilateral(ad, bc, ab, cd));
            given.Add(new Strengthened(q, new Square(q)));

            known.AddSegmentLength(new Segment(b, o), 10);

            //Not sure yet what the final regions will be,
            //problem crashes before exiting the hard coded parser
            goalRegions = parser.implied.GetAtomicRegionsByFigure(outer);

            SetSolutionArea(150*Math.PI - 200);

            problemName = "Glencoe Page 8 Problem 21";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
