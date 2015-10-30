using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 145 Problem 9
	//
	public class Page145Problem09 : CongruentTrianglesProblem
	{
        public Page145Problem09(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 145 Problem 9";


			Point s = new Point("S", 0, 0); points.Add(s);
			Point r = new Point("R", 2, 4); points.Add(r);
			Point t = new Point("T", 8, 4); points.Add(t);
			Point k = new Point("K", 4, 4); points.Add(k);

			Point y = new Point("Y", 10, 0); points.Add(y);
			Point x = new Point("X", 12, 4); points.Add(x);
			Point z = new Point("Z", 18, 4); points.Add(z);
			Point l = new Point("L", 14, 4); points.Add(l);
			
			Segment rs = new Segment(r, s); segments.Add(rs);
			Segment st = new Segment(s, t); segments.Add(st);
			Segment ks = new Segment(k, s); segments.Add(ks);
			Segment xy = new Segment(x, y); segments.Add(xy);
			Segment yz = new Segment(y, z); segments.Add(yz);
			Segment ly = new Segment(l, y); segments.Add(ly);

			List<Point> pts = new List<Point>();
			pts.Add(r);
			pts.Add(k);
			pts.Add(t);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(x);
			pts.Add(l);
			pts.Add(z);
            collinear.Add(new Collinear(pts)); 

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentTriangles(new Triangle(r, s, t), new Triangle(x, y, z)));
			given.Add(new AngleBisector((Angle)parser.Get(new Angle(r, s, t)), ks));
            given.Add(new AngleBisector((Angle)parser.Get(new Angle(x, y, z)), ly));

            goals.Add(new GeometricCongruentSegments(ly, ks));
		}
	}
}