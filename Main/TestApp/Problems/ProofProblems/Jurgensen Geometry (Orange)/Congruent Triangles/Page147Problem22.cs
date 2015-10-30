using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 147 Problem 22
	//
	public class Page147Problem22 : CongruentTrianglesProblem
	{
        public Page147Problem22(bool onoff, bool complete) : base(onoff, complete)
		{
			Point a = new Point("A", 3, 9); points.Add(a);
			Point b = new Point("B", 0, 0); points.Add(b);
			Point c = new Point("C", 12, 6); points.Add(c);
			Point d = new Point("D", 12, 0); points.Add(d);
			Point e = new Point("E", 3, 0); points.Add(e);
			Point f = new Point("F", 12, 9); points.Add(f);
			Point m = new Point("M", 6, 3); points.Add(m);

			Segment ab = new Segment(a, b); segments.Add(ab);
			Segment ac = new Segment(a, c); segments.Add(ac);
			Segment ad = new Segment(a, d); segments.Add(ad);
			Segment ae = new Segment(a, e); segments.Add(ae);
			Segment af = new Segment(a, f); segments.Add(af);
			Segment am = new Segment(a, m); segments.Add(am);
			Segment bc = new Segment(b, c); segments.Add(bc);

			List<Point> pts = new List<Point>();
			pts.Add(b);
			pts.Add(e);
			pts.Add(d);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(b);
			pts.Add(m);
			pts.Add(c);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(d);
			pts.Add(c);
			pts.Add(f);
            collinear.Add(new Collinear(pts));
			
		                parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(b, m)), (Segment)parser.Get(new Segment(m, c))));
			given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(e, d, a)), (Angle)parser.Get(new Angle(a, d, c))));		
			given.Add(new RightTriangle((Segment)parser.Get(ab), (Segment)parser.Get(ae), (Segment)parser.Get(new Segment(b, e))));
			given.Add(new RightTriangle((Segment)parser.Get(ac), (Segment)parser.Get(am), (Segment)parser.Get(new Segment(m, c))));
			given.Add(new RightTriangle((Segment)parser.Get(ac), (Segment)parser.Get(af), (Segment)parser.Get(new Segment(c, f))));
		}
	}
}