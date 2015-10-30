using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class WpfkProb46 : ActualShadedAreaProblem
    {
        public WpfkProb46(bool onoff, bool complete)
            : base(onoff, complete)
        {
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 8); points.Add(b);
            Point c = new Point("C", 8, 8); points.Add(c);
            Point d = new Point("D", 8, 0); points.Add(d);
            //Point e = new Point("E", 4 + System.Math.Sqrt(2), 4 + System.Math.Sqrt(2)); points.Add(e);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment bc = new Segment(b, c); segments.Add(bc);
            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment da = new Segment(d, a); segments.Add(da);
            //Segment ce = new Segment(c, e); segments.Add(ce);

            Circle tL = new Circle(b, 4);
            Circle bL = new Circle(a, 4);
            Circle tR = new Circle(c, 4);
            Circle bR = new Circle(d, 4);

            circles.Add(tL);
            circles.Add(bL);
            circles.Add(tR);
            circles.Add(bR);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, new Point("",0,4))), 4);

            //Angle a1 = (Angle)parser.Get(new Angle(a, b, c));
            //given.Add(new Strengthened(a1, new RightAngle(a1)));
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(ab, cd, bc, da));
            given.Add(new Strengthened(quad, new Square(quad)));
            given.Add(new GeometricCongruentCircles(tL, bL));
            given.Add(new GeometricCongruentCircles(tL, tR));
            given.Add(new GeometricCongruentCircles(tL, bR));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 4, 4));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(64 - 16 * System.Math.PI);

            problemName = "Word Problems For Kids - Grade 11 Prob 46";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}


