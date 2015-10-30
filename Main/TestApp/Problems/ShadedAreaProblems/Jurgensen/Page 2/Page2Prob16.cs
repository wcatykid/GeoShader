using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class Page2Prob16 : ActualShadedAreaProblem
    {
        public Page2Prob16(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 1.5, 3 * System.Math.Sin(System.Math.PI / 3)); points.Add(a);
            Point b = new Point("B", 3, 0); points.Add(b);
            Point o = new Point("O", 0, 0); points.Add(o);

            Segment ob = new Segment(o, b); segments.Add(ob);
            Segment oa = new Segment(o, a); segments.Add(oa);
            Segment ab = new Segment(a, b); segments.Add(ab);

            circles.Add(new Circle(o, 3.0));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddAngleMeasureDegree((Angle)parser.Get(new Angle(a, o, b)), 60);

            known.AddSegmentLength(ob, 3);

            goalRegions.AddRange(parser.implied.GetAllAtomicRegionsWithoutPoint(new Point("", 1.3, 2.5)));

            SetSolutionArea(((3*System.Math.PI)/2) - (9/4)*System.Math.Sqrt(3));
        }
    }
}