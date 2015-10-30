using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page205 : ActualShadedAreaProblem
    {
        //
        // Polygons only.
        //
        public Page205(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0);                   points.Add(a);
            Point b = new Point("B", 5 / Math.Sqrt(2.0), 0);  points.Add(b);
            Point c = new Point("C", 10 / Math.Sqrt(2.0), 0); points.Add(c);
            Point d = new Point("D", 0, 5 / Math.Sqrt(2.0));  points.Add(d);
            Point e = new Point("E", 0, 10 / Math.Sqrt(2.0)); points.Add(e);

            Segment bd = new Segment(b, d); segments.Add(bd);
            Segment ce = new Segment(c, e); segments.Add(ce);

            List<Point> pts = new List<Point>();
            pts.Add(a);
            pts.Add(d);
            pts.Add(e);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(a);
            pts.Add(b);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Triangle iso = (Triangle)parser.Get(new Triangle(a, b, d));
            given.Add(new Strengthened(iso, new IsoscelesTriangle(iso)));

            iso = (Triangle)parser.Get(new Triangle(e, a, c));
            given.Add(new Strengthened(iso, new IsoscelesTriangle(iso)));

            given.Add(new RightAngle(e, a, c));

            known.AddSegmentLength(bd, 5);
            known.AddSegmentLength(ce, 10);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 6, 1));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(18.75);

            problemName = "Singapore Problem Page 205";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}