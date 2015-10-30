using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 223 Problem 24
	//
	public class Page223Problem24 : CongruentTrianglesProblem
	{
        public Page223Problem24(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 223 Problem 24";


			Point b = new Point("B", 2, 4); points.Add(b);
			Point n = new Point("N", 8, 4); points.Add(n);
			Point l = new Point("L", 0, 0); points.Add(l);
			Point c = new Point("C", 10, 0); points.Add(c);
			Point m = new Point("M", 5, 2.5); points.Add(m);

			Segment cl = new Segment(c, l); segments.Add(cl);
			Segment bn = new Segment(b, n); segments.Add(bn);

			List<Point> pts = new List<Point>();
			pts.Add(l);
			pts.Add(m);
			pts.Add(n);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(b);
			pts.Add(m);
			pts.Add(c);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(m, b, n)), (Angle)parser.Get(new Angle(m, c, l))));
		}
	}
}