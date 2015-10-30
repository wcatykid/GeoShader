using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 223 Problem 26
	//
	public class Page223Problem26 : CongruentTrianglesProblem
	{
        public Page223Problem26(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 223 Problem 26";


			Point d = new Point("D", 0, 0); points.Add(d);
			Point g = new Point("G", 4, 0); points.Add(g);
			Point a = new Point("A", 0, 8); points.Add(a);
			Point h = new Point("H", 2, 4); points.Add(h);
			Point e = new Point("E", 0, 3); points.Add(e);

			Segment eh = new Segment(e, h); segments.Add(eh);
			Segment dg = new Segment(d, g); segments.Add(dg);

			List<Point> pts = new List<Point>();
			pts.Add(d);
			pts.Add(e);
			pts.Add(a);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(a);
			pts.Add(h);
			pts.Add(g);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new RightAngle(a, d, g));
			given.Add(new RightAngle(e, h, a));
		}
	}
}