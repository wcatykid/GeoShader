using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Prob39 : ActualShadedAreaProblem
    {
        public Page8Prob39(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point o = new Point("O", 0, 0); points.Add(o);
            Point a = new Point("A", -3.5, -3.5); points.Add(a);
            Point b = new Point("B", -3.5, 3.5); points.Add(b);
            Point c = new Point("C", 3.5, 3.5); points.Add(c);
            Point d = new Point("D", 3.5, -3.5); points.Add(d);
            Point e = new Point("E", -1.5, 1.5 / System.Math.Sqrt(3)); points.Add(e);
            Point f = new Point("F", 1.5, 1.5 / System.Math.Sqrt(3)); points.Add(f);
            Point g = new Point("G", 0, 3 / System.Math.Sqrt(3)); points.Add(g);

            Point w = new Point("W", 0, -3.5); points.Add(w);
            Point x = new Point("X", -3.5, 0); points.Add(x);
            Point y = new Point("Y", 0, 3.5); points.Add(y);
            Point z = new Point("W", 3.5, 0); points.Add(w);

            //Segment ab = new Segment(a, b); segments.Add(ab);
            //Segment bc = new Segment(b, c); segments.Add(bc);
            //Segment cd = new Segment(c, d); segments.Add(cd);
            //Segment da = new Segment(d, a); segments.Add(da);
            Segment ef = new Segment(e, f); segments.Add(ef);
            Segment fg = new Segment(f, g); segments.Add(fg);
            Segment ge = new Segment(g, e); segments.Add(ge);

            List<Point> pnts = new List<Point>();
            pnts.Add(a); pnts.Add(x); pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b); pnts.Add(y); pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c); pnts.Add(z); pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(d); pnts.Add(w); pnts.Add(a);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(o, 3.5);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 7);
            known.AddSegmentLength(ef, 3);

            given.Add(new GeometricCongruentSegments(ef, fg));
            given.Add(new GeometricCongruentSegments(ef, ge));

            Angle angle = (Angle)parser.Get(new Angle(b, a, d));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(x, circle,
                (Segment)parser.Get(new Segment(a, b))));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(y, circle,
                (Segment)parser.Get(new Segment(b, c))));
            CircleSegmentIntersection cInter3 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(z, circle,
                (Segment)parser.Get(new Segment(c, d))));
            CircleSegmentIntersection cInter4 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(w, circle,
                (Segment)parser.Get(new Segment(d, a))));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 3, 0));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea((12.25 * System.Math.PI) - (1.5 * System.Math.Sqrt(6.75)));

            problemName = "Glencoe Page 8 Problem 39";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}