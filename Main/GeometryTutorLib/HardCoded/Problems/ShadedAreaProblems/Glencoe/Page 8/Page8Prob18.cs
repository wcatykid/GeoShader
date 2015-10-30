using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Prob18 : ActualShadedAreaProblem
    {
        public Page8Prob18(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", -5, 0); points.Add(a);
            Point b = new Point("B", -2.5, 2.5*Math.Sqrt(3)); points.Add(b);
            Point c = new Point("C", 5, 0); points.Add(c);
            //Point d = new Point("D", 0, -5); points.Add(d);

            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            //Segment ad = new Segment(a, d); segments.Add(ad);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 5);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(ab, 5);
            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(a, c, b)), 30);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", -2, 1));
            unwanted.Add(new Point("", -0.5, 0.5));
            unwanted.Add(new Point("", 1, 1));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea((25)*Math.PI - (0.5 * 5 * Math.Sqrt(75)));

            problemName = "Glencoe Page 8 Problem 18";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
