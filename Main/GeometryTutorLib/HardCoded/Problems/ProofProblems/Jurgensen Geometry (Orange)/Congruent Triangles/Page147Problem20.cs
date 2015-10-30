using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 147 Problem 20
	//
	public class Page147Problem20 : CongruentTrianglesProblem
	{
        public Page147Problem20(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 147 Problem 20";


			Point m = new Point("M", 8, 1); points.Add(m);
			Point n = new Point("N", 5, 1); points.Add(n);
			Point p = new Point("P", 0, 2); points.Add(p);
			Point q = new Point("Q", 12, 2); points.Add(q);
			Point r = new Point("R", 6, 2); points.Add(r);
			Point s = new Point("S", 10, 0); points.Add(s);
			Point t = new Point("T", 4, 0); points.Add(t);

			Segment st = new Segment(s, t); segments.Add(st);

			List<Point> pts = new List<Point>();
			pts.Add(t);
			pts.Add(n);
			pts.Add(r);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(r);
			pts.Add(m);
			pts.Add(s);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(p);
			pts.Add(n);
			pts.Add(s);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(q);
			pts.Add(m);
			pts.Add(t);
            collinear.Add(new Collinear(pts));

			            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(p, n)), (Segment)parser.Get(new Segment(n, s))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(t, m)), (Segment)parser.Get(new Segment(q, m))));
		}
	}
}