using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class OsProb8 : ActualShadedAreaProblem
    {
        public OsProb8(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 40); points.Add(b);
            Point c = new Point("C", 40, 0); points.Add(c);
            Point n = new Point("N", 20, 20); points.Add(n);
            Point o = new Point("O", 0, 20); points.Add(o);
            Point p = new Point("P", 20, 0); points.Add(p);

            //Needed until triangles are sent for processing
            Segment on = new Segment(o, n); segments.Add(on);
            Segment pn = new Segment(p, n); segments.Add(pn);

            List<Point> pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(o);
            pnts.Add(b);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(p);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(n);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            Circle circle1 = new Circle(o, 20);
            Circle circle2 = new Circle(p, 20);
            circles.Add(circle1);
            circles.Add(circle2);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Triangle tri = (Triangle)parser.Get(new Triangle(a, b, c));
            given.Add(new Strengthened(tri, new RightTriangle(tri)));


            known.AddSegmentLength((Segment)parser.Get(new Segment(o, a)), 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(a, p)), 20);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 15, 14.8));
            wanted.Add(new Point("", 15, 15.2));
            wanted.Add(new Point("", 21, 19.1));
            wanted.Add(new Point("", 18, 22.2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(400 * System.Math.PI - 800);

            problemName = "Old School Problem 8";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}

