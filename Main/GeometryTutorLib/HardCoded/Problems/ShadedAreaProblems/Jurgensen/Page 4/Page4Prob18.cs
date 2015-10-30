using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page4prob18 : ActualShadedAreaProblem
    {
        public Page4prob18(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double CAB = System.Math.Acos((float)5/13); //angle measure
            Point a = new Point("A", -6.5, 0); points.Add(a);
            Point b = new Point("B", -6.5 + 5 * System.Math.Cos(CAB), 5*System.Math.Sin(CAB)); 
            points.Add(b);
            Point c = new Point("C", 6.5, 0); points.Add(c);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(o);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            circles.Add(new Circle(o, 6.5));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, c)), 13);
            known.AddSegmentLength(ab, 5);
            known.AddSegmentLength(bc, 12);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", -1, 1));
            unwanted.Add(new Point("", 1, 1));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(42.25 * System.Math.PI - 30);

            problemName = "Jurgensen Page 4 Problem 18";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}