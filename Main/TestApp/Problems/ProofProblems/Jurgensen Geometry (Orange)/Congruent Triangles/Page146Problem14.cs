using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 146 Problem 14
	//
	public class Page146Problem14 : CongruentTrianglesProblem
	{
        public Page146Problem14(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 14";


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

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel(dg, ef));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(g, d, e)), (Angle)parser.Get(new Angle(g, f, e))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(g, h)), (Segment)parser.Get(new Segment(e, k))));

            goals.Add(new GeometricParallel(dh, fk));
		}
	}
}