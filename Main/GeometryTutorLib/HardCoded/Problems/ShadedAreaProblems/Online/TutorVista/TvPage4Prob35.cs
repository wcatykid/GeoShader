using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage4Prob35 : ActualShadedAreaProblem
    {
        public TvPage4Prob35(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double x1 = 4;
            double y1 = 4 * System.Math.Sqrt(3);
            double x2 = 8 * System.Math.Cos(67.5 * System.Math.PI/180.0);
            double y2 = 8 * System.Math.Sin(67.5 * System.Math.PI / 180.0);

            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -x1, -y1); points.Add(a);
            Point b = new Point("B", x1, -y1); points.Add(b);
            Point c = new Point("C", -x2, -y2); points.Add(c);
            Point d = new Point("D", x2, -y2); points.Add(d);

            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ob = new Segment(o, b); segments.Add(ob);
            Segment oc = new Segment(o, c); segments.Add(oc);
            Segment od = new Segment(o, d); segments.Add(od);
            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            Circle circle = new Circle(o, 8);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(oa, 8);
            //known.AddArcMeasureDegree((MinorArc)parser.Get(new MinorArc(circle, a, b)), 60);
            //known.AddArcMeasureDegree((MinorArc)parser.Get(new MinorArc(circle, c, d)), 45);

            given.Add(new GeometricParallel(ab, cd));
            given.Add(new GeometricArcEquation((MinorArc)parser.Get(new MinorArc(circle, a, b)), new NumericValue(60)));
            given.Add(new GeometricArcEquation((MinorArc)parser.Get(new MinorArc(circle, c, d)), new NumericValue(45)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0, 1));
            unwanted.Add(new Point("", -x1 + 0.1, -y1+0.1));
            unwanted.Add(new Point("", x1 - 0.1, -y1+0.1));
            unwanted.Add(new Point("", 0, -1));
            unwanted.Add(new Point("", 0, -y2-0.1));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(3.292184486440);

            problemName = "Tutor Vista Page 4 Problem 35";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

