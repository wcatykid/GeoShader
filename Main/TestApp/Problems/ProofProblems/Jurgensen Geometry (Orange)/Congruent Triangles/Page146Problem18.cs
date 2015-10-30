using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 146 Problem 18
	//
	public class Page146Problem18 : CongruentTrianglesProblem
	{
		public Page146Problem18(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 18";


			Point a = new Point("A", 5, 6); points.Add(a);
			Point b = new Point("B", 0, 2); points.Add(b);
			Point c = new Point("C", 2, 0); points.Add(c);
			Point d = new Point("D", 8, 0); points.Add(d);
			Point e = new Point("E", 10, 2); points.Add(e);
			Point g = new Point("G", 3, 2); points.Add(g);
			Point f = new Point("F", 7, 2); points.Add(f);

			Segment ab = new Segment(a, b); segments.Add(ab);
			Segment ae = new Segment(a, e); segments.Add(ae);
			Segment bc = new Segment(b, c); segments.Add(bc);
			Segment cd = new Segment(c, d); segments.Add(cd);
			Segment de = new Segment(d, e); segments.Add(de);

			List<Point> pts = new List<Point>();
			pts.Add(a);
			pts.Add(g);
			pts.Add(c);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(a);
			pts.Add(f);
			pts.Add(d);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(b);
			pts.Add(g);
			pts.Add(f);
			pts.Add(e);
            collinear.Add(new Collinear(pts));

			            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, g, f)), (Angle)parser.Get(new Angle(a, f, g))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, c, g)), (Angle)parser.Get(new Angle(f, d, c))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(g, a, b)), (Angle)parser.Get(new Angle(f, a, e))));

            goals.Add(new GeometricCongruentSegments(bc, de));
		}
	}
}