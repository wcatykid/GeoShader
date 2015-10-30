using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 146 Problem 13
	//
	public class Page146Problem13 : CongruentTrianglesProblem
	{
        public Page146Problem13(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 13";


			Point g = new Point("G", 0, 0); points.Add(g);
			Point d = new Point("D", 3, 2); points.Add(d);
			Point e = new Point("E", 7, 0); points.Add(e);
			Point h = new Point("H", 2, 0); points.Add(h);
			Point k = new Point("K", 5, 0); points.Add(k);
			Point f = new Point("F", 4, -2); points.Add(f);
			
			Segment dg = new Segment(d, g); segments.Add(dg);
			Segment de = new Segment(d, e); segments.Add(de);
			Segment dh = new Segment(d, h); segments.Add(dh);
			Segment fg = new Segment(f, g); segments.Add(fg);
			Segment fk = new Segment(f, k); segments.Add(fk);
			Segment ef = new Segment(e, f); segments.Add(ef);

			List<Point> pts = new List<Point>();
			pts.Add(g);
			pts.Add(h);
			pts.Add(k);
			pts.Add(e);
			collinear.Add(new Collinear(pts));

		                parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(de, fg));
            given.Add(new GeometricCongruentSegments(dg, ef));
            given.Add(new Strengthened((Angle)parser.Get(new Angle(h, d, e)), new RightAngle(h, d, e)));
            given.Add(new Strengthened((Angle)parser.Get(new Angle(k, f, g)), new RightAngle(k, f, g)));

            goals.Add(new GeometricCongruentSegments(dh, fk));
		}
	}
}