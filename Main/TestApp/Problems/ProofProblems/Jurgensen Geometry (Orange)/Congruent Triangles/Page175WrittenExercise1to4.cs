using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 175 Problems 1 - 4 (bottom of page)
	//
	public class Page175WrittenExercise1to4 : CongruentTrianglesProblem
	{
        public Page175WrittenExercise1to4(bool onoff, bool complete) : base(onoff, complete)
		{
			Point a = new Point("A", 0, 2); points.Add(a);
			Point b = new Point("B", 1, 3); points.Add(b);
			Point c = new Point("C", 0, 0); points.Add(c);
			Point d = new Point("D", 2, 2); points.Add(d);
			Point e = new Point("E", 3, 0); points.Add(e);
			Point f = new Point("F", 4, 1); points.Add(f);
			Point x = new Point("X", 0, 4); points.Add(x);
			Point y = new Point("Y", 6, 0); points.Add(y);

			Segment ab = new Segment(a, b); segments.Add(ab);
			Segment cd = new Segment(c, d); segments.Add(cd);
			Segment ef = new Segment(e, f); segments.Add(ef);

			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(a);
			pts.Add(c);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(c);
			pts.Add(e);
			pts.Add(y);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(y);
			pts.Add(f);
			pts.Add(d);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(x);
			pts.Add(b);
			pts.Add(d);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff); given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(x, a)), (Segment)parser.Get(new Segment(a, c))));
	        
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(c, e)), (Segment)parser.Get(new Segment(e, y))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(y, f)), (Segment)parser.Get(new Segment(f, d))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(d, b)), (Segment)parser.Get(new Segment(b, x))));
		}
	}
}