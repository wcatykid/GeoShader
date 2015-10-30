using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class TvPage4Prob40 : ActualShadedAreaProblem
    {
        public TvPage4Prob40(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 18.2); points.Add(b);
            Point c = new Point("C", 27.3, 18.2); points.Add(c);
            Point d = new Point("D", 27.3, 0); points.Add(d);
            Point e = new Point("E", 18.2, 0); points.Add(e);
            Point l = new Point("L", 27.3, 9.1); points.Add(l);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(e);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(c);
            pnts.Add(l);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            Circle left = new Circle(a, 18.2);
            Circle right = new Circle(d, 9.1);

            circles.Add(left);
            circles.Add(right);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, (Segment)parser.Get(new Segment(c, d)), 
                bc, (Segment)parser.Get(new Segment(a, d))));
            given.Add(new Strengthened(quad, new Rectangle(quad)));

            known.AddSegmentLength(ab, 18.2);
            known.AddSegmentLength(bc, 27.3);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 18.2, 5));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(171.6658904);

            problemName = "Tutor Vista Page 4 Problem 40";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}



