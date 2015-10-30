using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 175 Problems 3-6
	//
	public class Page175ClassroomExercise03to06 : CongruentTrianglesProblem
	{
        public Page175ClassroomExercise03to06(bool onoff, bool complete)
            : base(onoff, complete)
		{
			Point e = new Point("E", 0, 0); points.Add(e);
			Point f = new Point("F", 6, 4); points.Add(f);
			Point g = new Point("G", 6, 6); points.Add(g);
			Point h = new Point("H", 0, 10); points.Add(h);
			Point m = new Point("M", 3, 2); points.Add(m);
			Point n = new Point("N", 3, 8); points.Add(n);


			Segment eh = new Segment(e, h); segments.Add(eh);
			Segment fg = new Segment(f, g); segments.Add(fg);

			List<Point> pts = new List<Point>();
			pts.Add(e);
			pts.Add(m);
			pts.Add(f);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(h);
			pts.Add(n);
			pts.Add(g);
            collinear.Add(new Collinear(pts));

		                parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(e, m)), (Segment)parser.Get(new Segment(m, f))));
			given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(h, n)), (Segment)parser.Get(new Segment(n, g))));
		}
	}
}