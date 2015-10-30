using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 223 Problem 22
	//
	public class Page223Problem22 : CongruentTrianglesProblem
	{
        public Page223Problem22(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 223 Problem 22";


            Point x = new Point("X", 1, 0); points.Add(x);
			Point f = new Point("F", 3, 4); points.Add(f);
			Point s = new Point("S", 4, 6); points.Add(s);
			Point e = new Point("E", 5, 0); points.Add(e);
			Point r = new Point("R", 7, 0); points.Add(r);
			
			Segment rs = new Segment(r, s); segments.Add(rs);
			Segment ef = new Segment(e, f); segments.Add(ef);

			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(e);
			pts.Add(r);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(x);
			pts.Add(f);
			pts.Add(s);
            collinear.Add(new Collinear(pts));

			            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricParallel(rs, ef));

            goals.Add(new GeometricSimilarTriangles((Triangle)parser.Get(new Triangle(f, x, e)), (Triangle)parser.Get(new Triangle(s, x, r))));
        }
	}
}