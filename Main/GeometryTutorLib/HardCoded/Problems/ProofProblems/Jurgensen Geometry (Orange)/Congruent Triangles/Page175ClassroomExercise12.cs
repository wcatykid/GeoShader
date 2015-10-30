using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.GeometryTestbed
{
	//
	// Geometry; Page 175 Problems 7-12
	//
	public class Page175ClassroomExercise12 : CongruentTrianglesProblem
	{
        public Page175ClassroomExercise12(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 175 CLassroom Ex 12";

            //Point z = new Point("Z", 0, 12); points.Add(z);
            //Point y = new Point("Y", 12, 0); points.Add(y);
            //Point x = new Point("X", 0, 0); points.Add(x);
            //Point m = new Point("M", 6, 0); points.Add(m);
            //Point n = new Point("N", 6, 6); points.Add(n);
            //Point t = new Point("T", 0, 6); points.Add(t);

            Point z = new Point("Z", 0, 6); points.Add(z);
            Point y = new Point("Y", 8, 0); points.Add(y);
            Point x = new Point("X", 4, 0); points.Add(x);
            Point m = new Point("M", 6, 0); points.Add(m);
            Point n = new Point("N", 4, 3); points.Add(n);
            Point t = new Point("T", 2, 3); points.Add(t);

			Segment mt = new Segment(m, t); segments.Add(mt);
			Segment mn = new Segment(m, n); segments.Add(mn);
			Segment nt = new Segment(n, t); segments.Add(nt);
			
			List<Point> pts = new List<Point>();
			pts.Add(x);
			pts.Add(m);
			pts.Add(y);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(y);
			pts.Add(n);
			pts.Add(z);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(z);
			pts.Add(t);
			pts.Add(x);
            collinear.Add(new Collinear(pts));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( m, new Segment(x, y)))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( n, new Segment(z, y)))));
            given.Add(new Midpoint((InMiddle)parser.Get(new InMiddle( t, new Segment(z, x)))));

            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(t, x, m)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(n, m, y)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(z, t, n), new Triangle(m, n, t)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(m, n, t), new Triangle(n, m, y)));
            goals.Add(new GeometricCongruentTriangles(new Triangle(m, n, t), new Triangle(t, x, m)));

            goals.Add(new GeometricCongruentTriangles(new Triangle(t, x, m), new Triangle(n, m, y)));
		}
	}
}