using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 146 Problem 17
	//
	public class Page146Problem17 : CongruentTrianglesProblem
	{
        public Page146Problem17(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 17";


            Point d = new Point("D", 2, 3); points.Add(d);
			Point b = new Point("B", 10, 0); points.Add(b);
			Point a = new Point("A", 0, 0); points.Add(a);
			Point c = new Point("C", 8, 3); points.Add(c);
			Point m = new Point("M", 5, 0); points.Add(m);

			Segment bc = new Segment(b, c); segments.Add(bc);
			Segment ad = new Segment(a, d); segments.Add(ad);
			Segment cd = new Segment(c, d); segments.Add(cd);
			Segment bd = new Segment(b, d); segments.Add(bd);
			Segment ac = new Segment(a, c); segments.Add(ac);
			Segment dm = new Segment(d, m); segments.Add(dm);
			Segment cm = new Segment(c, m); segments.Add(cm);
			

			List<Point> pts = new List<Point>();
			pts.Add(a);
			pts.Add(m);
			pts.Add(b);
			collinear.Add(new Collinear(pts));

			            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, m)), (Segment)parser.Get(new Segment(b, m))));
            given.Add(new GeometricCongruentSegments(ad, bc));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(m, d, c)), (Angle)parser.Get(new Angle(m, c, d))));
		}
	}
}