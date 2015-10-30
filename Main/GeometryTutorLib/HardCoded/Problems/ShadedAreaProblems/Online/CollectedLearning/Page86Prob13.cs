using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page86Prob13 : ActualShadedAreaProblem
    {
        public Page86Prob13(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", -5.5, 12); points.Add(b);
            Point c = new Point("C", 14.5, 12); points.Add(c);
            Point d = new Point("D", 9, 0); points.Add(d);
            Point o = new Point("O", 4.5, 12); points.Add(o);
            Point p = new Point("P", 4.5, 6); points.Add(p);
            Point q = new Point("Q", 4.5, 0); points.Add(q);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment cd = new Segment(c, d); segments.Add(cd);

            List<Point> pnts = new List<Point>();
            pnts.Add(b);
            pnts.Add(o);
            pnts.Add(c);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(a);
            pnts.Add(q);
            pnts.Add(d);
            collinear.Add(new Collinear(pnts));

            pnts = new List<Point>();
            pnts.Add(o);
            pnts.Add(p);
            pnts.Add(q);
            collinear.Add(new Collinear(pnts));

            Circle circle = new Circle(p, 6);
            circles.Add(circle);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Segment ad = (Segment)parser.Get(new Segment(a, d));
            Segment bc = (Segment)parser.Get(new Segment(b, c));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(q, circle, ad));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(o, circle, bc));

            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));

            known.AddSegmentLength(ad, 9);
            //This top base was length 10 in the original problem, but this wouldn't actually be possible since the 
            //inscribed circle has a total diameter of length 12. So changed the top base to length 20.
            known.AddSegmentLength(bc, 20);
            known.AddSegmentLength((Segment)parser.Get(new Segment(o, p)), 6);

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0.5, 0.2));
            wanted.Add(new Point("", -0.3, 11));
            wanted.Add(new Point("", 9.2, 11));
            wanted.Add(new Point("", 8.5, 0.2));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(174 - 36 * System.Math.PI);

            problemName = "Collected Learning Page 86 Prob 13";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


