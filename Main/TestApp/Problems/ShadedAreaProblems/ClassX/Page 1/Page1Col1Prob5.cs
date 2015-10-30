using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page1Col1Prob5 : ActualShadedAreaProblem
    {
        public Page1Col1Prob5(bool onoff, bool complete) : base(onoff, complete)
        {
            Point a = new Point("A", -3.5, 0); points.Add(a);
            Point b = new Point("B", 3.5, 0); points.Add(b);
            Point c = new Point("C", 0, -3.5 * System.Math.Sqrt(3)); points.Add(c);

            Point d = new Point("D", 0, 0); points.Add(d);
            Point e = new Point("E", -1.75, -3.5 * System.Math.Sqrt(3.0) / 2.0); points.Add(e);
            Point f = new Point("F", 1.75, -3.5 * System.Math.Sqrt(3.0) / 2.0); points.Add(f);

            //Segment ad = new Segment(a, d); segments.Add(ad);
            //Segment bd = new Segment(b, d); segments.Add(bd);
            //Segment ce = new Segment(c, e); segments.Add(ce);

            circles.Add(new Circle(a, 3.5));
            circles.Add(new Circle(b, 3.5));
            circles.Add(new Circle(c, 3.5));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength(new Segment(a, d), 3.5);
            //known.AddSegmentLength(new Segment(b, d), 3.5);
            //known.AddSegmentLength(new Segment(c, e), 3.5);

            // The goal is the entire area of the figure.
            goalRegions.Add(parser.implied.GetAtomicRegionByPoint(new Point("", 0, -1)));

            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, d)), (Segment)parser.Get(new Segment(b, d))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, d)), (Segment)parser.Get(new Segment(e, c))));

            SetSolutionArea(1.975367389);
        }
    }
}