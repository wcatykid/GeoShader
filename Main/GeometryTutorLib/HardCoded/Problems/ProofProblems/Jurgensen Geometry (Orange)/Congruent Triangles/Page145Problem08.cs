using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 145 Problem 8
	//
	public class Page145Problem08 : CongruentTrianglesProblem
	{
        public Page145Problem08(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 145 Problem 8";


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

			            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(l, f, k)), (Segment)parser.Get(new Segment(f, a))));
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(l, a, k)), (Segment)parser.Get(new Segment(f, a))));

            goals.Add(new AngleBisector((Angle)parser.Get(new Angle(l, j, k)), (Segment)parser.Get(new Segment(f, a))));
		}
	}
}