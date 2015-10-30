using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 146 Problem 12
	//
	public class Page146Problem12 : CongruentTrianglesProblem
	{
        public Page146Problem12(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 12";


			Point l = new Point("L", 0, 5); points.Add(l);
			Point m = new Point("M", 2, 0); points.Add(m);
			Point n = new Point("N", 6, 0); points.Add(n);
			Point x = new Point("X", 0, 0); points.Add(x);

			Point r = new Point("R", 10, 5); points.Add(r);
			Point s = new Point("S", 12, 0); points.Add(s);
			Point t = new Point("T", 16, 0); points.Add(t);
			Point y = new Point("Y", 10, 0); points.Add(y);
			
			Segment lm = new Segment(l, m); segments.Add(lm);
			Segment ln = new Segment(l, n); segments.Add(ln);
			Segment lx = new Segment(l, x); segments.Add(lx);
			Segment rs = new Segment(r, s); segments.Add(rs);
			Segment rt = new Segment(r, t); segments.Add(rt);
			Segment ry = new Segment(r, y); segments.Add(ry);

			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(m);
			pts.Add(n);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(y);
			pts.Add(s);
			pts.Add(t);
            collinear.Add(new Collinear(pts));

            Triangle tOne = new Triangle(l, m, n);
            Triangle tTwo = new Triangle(r, s, t);

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentTriangles((Triangle)parser.Get(tOne), (Triangle)parser.Get(tTwo)));
            given.Add(new Altitude((Triangle)parser.Get(tOne), lx));
            given.Add(new Altitude((Triangle)parser.Get(tTwo), ry));

            goals.Add(new GeometricCongruentSegments(lx, ry));
		}
	}
}