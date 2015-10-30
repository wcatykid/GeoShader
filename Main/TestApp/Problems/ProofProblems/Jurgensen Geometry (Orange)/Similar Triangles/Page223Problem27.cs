using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 223 Problem 27
	//
	public class Page223Problem27 : CongruentTrianglesProblem
	{
        public Page223Problem27(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 223 Problem 27";


			Point r = new Point("R", 0, 0); points.Add(r);
            Point v = new Point("V", 6, 0); points.Add(v);
            Point s = new Point("S", 10, 0); points.Add(s);

            Point p = new Point("P", 4, 6); points.Add(p);

            Point q = new Point("Q", 2, 3); points.Add(q);
			Point u = new Point("U", 5, 3); points.Add(u);
            Point t = new Point("T", 7, 3); points.Add(t);
			
			List<Point> pts = new List<Point>();
			pts.Add(p);
			pts.Add(q);
			pts.Add(r);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(p);
			pts.Add(u);
			pts.Add(v);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
			pts.Add(p);
			pts.Add(t);
			pts.Add(s);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
			pts.Add(r);
			pts.Add(v);
			pts.Add(s);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(q);
            pts.Add(u);
            pts.Add(t);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel((Segment)parser.Get(new Segment(q, t)), (Segment)parser.Get(new Segment(r, s))));
		}
	}
}