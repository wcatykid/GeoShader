using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page7Prob15 : ActualShadedAreaProblem
    {
        public Page7Prob15(bool onoff, bool complete)
            : base(onoff, complete)
        {
            double r = 5 / Math.Sqrt(2);
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 5*(System.Math.Sqrt(2))); points.Add(b);
            Point c = new Point("C", 10, 5*(System.Math.Sqrt(2))); points.Add(c);
            Point d = new Point("D", 10, 0); points.Add(d);
            Point e = new Point("E", r, r); points.Add(e);
            Point f = new Point("F", 10 - r, r); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment be = new Segment(b, e); segments.Add(be);
            Segment cf = new Segment(c, f); segments.Add(cf);
            Segment df = new Segment(d, f); segments.Add(df);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(bc, 10);
            known.AddSegmentLength(be, 5);
            known.AddSegmentLength(cd, 5*System.Math.Sqrt(2));

            given.Add(new GeometricCongruentSegments(ae, be));
            given.Add(new GeometricCongruentSegments(ae, cf));
            given.Add(new GeometricCongruentSegments(ae, df));

            Angle a1 = (Angle)parser.Get(new Angle(a, b, c));
            Angle a2 = (Angle)parser.Get(new Angle(b, c, d));
            Angle a3 = (Angle)parser.Get(new Angle(c, d, a));
            Angle a4 = (Angle)parser.Get(new Angle(d, a, b));
            Angle a5 = (Angle)parser.Get(new Angle(b, e, a));
            Angle a6 = (Angle)parser.Get(new Angle(c, f, d));

            given.Add(new Strengthened(a1, new RightAngle(a1)));
            given.Add(new Strengthened(a2, new RightAngle(a2)));
            given.Add(new Strengthened(a3, new RightAngle(a3)));
            given.Add(new Strengthened(a4, new RightAngle(a4)));
            given.Add(new Strengthened(a5, new RightAngle(a5)));
            given.Add(new Strengthened(a6, new RightAngle(a6)));

            //get rid of unwanted regions
            List<Point> unwanted = new List<Point>();
            unwanted.Add(new Point("", 1, 3));
            unwanted.Add(new Point("", 9, 3));
            goalRegions = parser.implied.GetAllAtomicRegionsWithoutPoints(unwanted);

            SetSolutionArea(50*System.Math.Sqrt(2) - 25);

            problemName = "Glencoe Page 7 Problem 15";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}