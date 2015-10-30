using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page5Row6Prob19 : ActualShadedAreaProblem
    {

        public Page5Row6Prob19(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", -3.5, 3.5); points.Add(b);
            Point c = new Point("C", -3.5, 7); points.Add(c);
            Point d = new Point("D", 0, 7); points.Add(d);
            Point e = new Point("E", 0, 3.5); points.Add(e);
            Point f = new Point("F", 7, 3.5); points.Add(f);
            Point g = new Point("G", 7, 0); points.Add(g);

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment ag = new Segment(a, g); segments.Add(ag);

            circles.Add(new Circle(e, 3.5));
            //Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(bc, cd, de, eb));
            //Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ef, fg, ga, ae));


            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(e);
            pts.Add(d);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b);
            pts.Add(e);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Angle a1 =(Angle)parser.Get(new Angle(b, c, d));
            Angle a2 =(Angle)parser.Get(new Angle(a, g, f));
            Angle a3 =(Angle)parser.Get(new Angle(e, f, g));
            Angle a4 =(Angle)parser.Get(new Angle(d, e, f));
            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new GeometricSegmentEquation(bc, new NumericValue(3.5)));
            given.Add(new GeometricSegmentEquation(cd, new NumericValue(3.5)));
            given.Add(new GeometricSegmentEquation(fg, new NumericValue(3.5)));

            known.AddSegmentLength(bc, 3.5);
            known.AddSegmentLength(cd, 3.5);
            known.AddSegmentLength(fg, 3.5);
            known.AddSegmentLength((Segment)parser.Get(new Segment(e, f)), 7);

            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 1, 3.7));
            unwanted.Add(new Point("", 3.4, 3.7));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(36.75+System.Math.PI*(12.25/4));

            problemName = "McDougall Page 5 Row 6 Problem 19";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}