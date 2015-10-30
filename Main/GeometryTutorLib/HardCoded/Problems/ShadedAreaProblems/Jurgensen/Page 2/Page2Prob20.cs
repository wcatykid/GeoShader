using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page2Prob20 : ActualShadedAreaProblem
    {
        public Page2Prob20(bool onoff, bool complete) : base(onoff, complete)
        {
            double x = System.Math.Sqrt(3);
            double y = 1;
            Point a = new Point("A", -x, y); points.Add(a);
            Point b = new Point("B", x, y); points.Add(b);
            Point c = new Point("C", -x, -y); points.Add(c);
            Point d = new Point("D", x, -y); points.Add(d);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment ac = new Segment(a, c); segments.Add(ac);
            //Segment bd = new Segment(b, d); segments.Add(bd);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(o);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 2);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            MinorArc arcAB = (MinorArc)parser.Get(new MinorArc(circle, a, b));
            MinorArc arcBD = (MinorArc)parser.Get(new MinorArc(circle, b, d));
            MinorArc arcDC = (MinorArc)parser.Get(new MinorArc(circle, d, c));
            MinorArc arcAC = (MinorArc)parser.Get(new MinorArc(circle, a, c));

            known.AddSegmentLength(ac, 2);
            known.AddArcMeasureDegree(arcAB, 120);
            known.AddArcMeasureDegree(arcBD, 60);
            known.AddArcMeasureDegree(arcDC, 120);
            known.AddArcMeasureDegree(arcAC, 60);

            given.Add(new GeometricArcEquation(arcAB, new NumericValue(120)));
            given.Add(new GeometricArcEquation(arcBD, new NumericValue(60)));
            given.Add(new GeometricArcEquation(arcDC, new NumericValue(120)));
            given.Add(new GeometricArcEquation(arcAC, new NumericValue(60)));

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 0, y+0.2));
            unwanted.Add(new Point("", 0, -y-0.2));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea((4.0 / 3.0) * System.Math.PI + 2.0 * System.Math.Sqrt(3.0));

            problemName = "Jurgensen Page 2 Problem 20";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}