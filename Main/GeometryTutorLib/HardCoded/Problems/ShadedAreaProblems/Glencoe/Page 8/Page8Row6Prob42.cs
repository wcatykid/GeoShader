using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class Page8Row6Prob42 : ActualShadedAreaProblem
    {
        public Page8Row6Prob42(bool onoff, bool complete)
            : base(onoff, complete)
        {
            //Square vertices
            Point a = new Point("A", 0, 0); points.Add(a);
            Point b = new Point("B", 0, 6); points.Add(b);
            Point c = new Point("C", 6, 6); points.Add(c);
            Point d = new Point("D", 6, 0); points.Add(d);

            //Circle centers
            Point q = new Point("Q", 1.5, 1.5); points.Add(q);
            Point r = new Point("R", 1.5, 4.5); points.Add(r);
            Point s = new Point("S", 4.5, 4.5); points.Add(s);
            Point t = new Point("T", 4.5, 1.5); points.Add(t);

            //Circle-Square intersections
            Point e = new Point("E", 0, 1.5); points.Add(e);
            Point f = new Point("F", 0, 4.5); points.Add(f);
            Point g = new Point("G", 1.5, 6); points.Add(g);
            Point h = new Point("H", 4.5, 6); points.Add(h);
            Point i = new Point("I", 6, 4.5); points.Add(i);
            Point j = new Point("J", 6, 1.5); points.Add(j);
            Point k = new Point("K", 4.5, 0); points.Add(k);
            Point l = new Point("L", 1.5, 0); points.Add(l);

            //Circle-circle intersections
            Point m = new Point("M", 1.5, 3); points.Add(m);
            Point n = new Point("N", 3, 4.5); points.Add(n);
            Point o = new Point("O", 4.5, 3); points.Add(o);
            Point p = new Point("P", 3, 1.5); points.Add(p);


            //Square sides
            List<Point> pts = new List<Point>();
            pts.Add(a); 
            pts.Add(e); 
            pts.Add(f); 
            pts.Add(b);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(b); 
            pts.Add(g); 
            pts.Add(h); 
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(c); 
            pts.Add(i);
            pts.Add(j); 
            pts.Add(d); 
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d); 
            pts.Add(k); 
            pts.Add(l); 
            pts.Add(a); 
            collinear.Add(new Collinear(pts));

            //Diameters
            pts = new List<Point>();
            pts.Add(f);
            pts.Add(r);
            pts.Add(n);
            pts.Add(s);
            pts.Add(i);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(q);
            pts.Add(p);
            pts.Add(t);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(g);
            pts.Add(r);
            pts.Add(m);
            pts.Add(q);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(h);
            pts.Add(s);
            pts.Add(o);
            pts.Add(t);
            pts.Add(k);
            collinear.Add(new Collinear(pts));

            Circle tLeft = new Circle(r, 1.5);
            Circle bLeft = new Circle(q, 1.5);
            Circle tRight = new Circle(s, 1.5);
            Circle bRight = new Circle(t, 1.5);

            circles.Add(tLeft);
            circles.Add(bLeft);
            circles.Add(tRight);
            circles.Add(bRight);

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            known.AddSegmentLength((Segment)parser.Get(new Segment(a, b)), 6);

            Segment ab = (Segment)parser.Get(new Segment(a, b));
            Segment bc = (Segment)parser.Get(new Segment(b, c));
            Segment cd = (Segment)parser.Get(new Segment(c, d));
            Segment ad = (Segment)parser.Get(new Segment(a, d));
            CircleSegmentIntersection cInter1 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(e, bLeft, ab));
            CircleSegmentIntersection cInter2 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(f, tLeft, ab));
            CircleSegmentIntersection cInter3 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(g, tLeft, bc));
            CircleSegmentIntersection cInter4 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(h, tRight, bc));
            CircleSegmentIntersection cInter5 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(i, tRight, cd));
            CircleSegmentIntersection cInter6 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(j, bRight, cd));
            CircleSegmentIntersection cInter7 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(k, bRight, ad));
            CircleSegmentIntersection cInter8 = (CircleSegmentIntersection)parser.Get(new CircleSegmentIntersection(l, bLeft, ad));
            CircleCircleIntersection cInter9 = (CircleCircleIntersection)parser.Get(new CircleCircleIntersection(m, tLeft, bLeft));
            CircleCircleIntersection cInter10 = (CircleCircleIntersection)parser.Get(new CircleCircleIntersection(n, tLeft, tRight));
            CircleCircleIntersection cInter11 = (CircleCircleIntersection)parser.Get(new CircleCircleIntersection(o, tRight, bRight));
            CircleCircleIntersection cInter12 = (CircleCircleIntersection)parser.Get(new CircleCircleIntersection(p, bLeft, bRight));

            given.Add(new Strengthened(cInter1, new Tangent(cInter1)));
            given.Add(new Strengthened(cInter2, new Tangent(cInter2)));
            given.Add(new Strengthened(cInter3, new Tangent(cInter3)));
            given.Add(new Strengthened(cInter4, new Tangent(cInter4)));
            given.Add(new Strengthened(cInter5, new Tangent(cInter5)));
            given.Add(new Strengthened(cInter6, new Tangent(cInter6)));
            given.Add(new Strengthened(cInter7, new Tangent(cInter7)));
            given.Add(new Strengthened(cInter8, new Tangent(cInter8)));
            given.Add(new Strengthened(cInter9, new Tangent(cInter9)));
            given.Add(new Strengthened(cInter10, new Tangent(cInter10)));
            given.Add(new Strengthened(cInter11, new Tangent(cInter11)));
            given.Add(new Strengthened(cInter12, new Tangent(cInter12)));

            given.Add(new GeometricCongruentCircles(tLeft, bLeft));
            given.Add(new GeometricCongruentCircles(tLeft, tRight));
            given.Add(new GeometricCongruentCircles(tLeft, bRight));

            List<Point> wanted = new List<Point>();
            wanted.Add(new Point("", 0.1, 0.1));
            wanted.Add(new Point("", 0.1, 3));
            wanted.Add(new Point("", 0.1, 5.9));
            wanted.Add(new Point("", 3, 5.9));
            wanted.Add(new Point("", 3, 3));
            wanted.Add(new Point("", 3, 0.1));
            wanted.Add(new Point("", 5.9, 0.1));
            wanted.Add(new Point("", 5.9, 3));
            wanted.Add(new Point("", 5.9, 5.9));
            goalRegions = parser.implied.GetAtomicRegionsByPoints(wanted);

            SetSolutionArea(36 - (9 * System.Math.PI));

            problemName = "Glencoe Page 8 Row 6 Problem 42";
            GeometryTutorLib.EngineUIBridge.HardCodedProblemsToUI.AddProblem(problemName, points, circles, segments);
        }
    }
}
