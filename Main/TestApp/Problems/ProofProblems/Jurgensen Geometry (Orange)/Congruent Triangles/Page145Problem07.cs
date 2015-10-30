using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 145 Problem 7
	//
	public class Page145Problem07 : CongruentTrianglesProblem
	{
        public Page145Problem07(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 145 Problem 7";


			Point f = new Point("F", 0, 5); points.Add(f);
			Point l = new Point("L", 4, 7); points.Add(l);
			Point k = new Point("K", 4, 3); points.Add(k);
			Point a = new Point("A", 5, 5); points.Add(a);
			Point j = new Point("J", 3, 5); points.Add(j);

			Segment fl = new Segment(f, l); segments.Add(fl);
			Segment fk = new Segment(f, k); segments.Add(fk);
			Segment al = new Segment(a, l); segments.Add(al);
			Segment ak = new Segment(a, k); segments.Add(ak);
			Segment jl = new Segment(j, l); segments.Add(jl);
			Segment jk = new Segment(j, k); segments.Add(jk);

			List<Point> pts = new List<Point>();
			pts.Add(f);
			pts.Add(j);
			pts.Add(a);
			collinear.Add(new Collinear(pts));

		                parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(al, ak));
            given.Add(new GeometricCongruentSegments(fl, fk));

            goals.Add(new GeometricCongruentSegments(jl, jk));
		}
	}
}