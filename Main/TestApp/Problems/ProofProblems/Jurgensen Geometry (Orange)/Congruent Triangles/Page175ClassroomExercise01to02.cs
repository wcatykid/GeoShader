using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 175 Problems 1-2
	//
	public class Page175ClassroomExercise01to02 : CongruentTrianglesProblem
	{
        public Page175ClassroomExercise01to02(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 175 Classroom Ex 1-2";

			Point a = new Point("A", 0, 0); points.Add(a);
			Point b = new Point("B", 9, 0); points.Add(b);
			Point c = new Point("C", 7, 3); points.Add(c);
			Point d = new Point("D", 2, 3); points.Add(d);
			
			Segment ab = new Segment(a, b); segments.Add(ab);
			Segment ad = new Segment(a, d); segments.Add(ad);
			Segment bc = new Segment(b, c); segments.Add(bc);
			Segment cd = new Segment(c, d); segments.Add(cd);

		                parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(d, a, b)), (Angle)parser.Get(new Angle(a, b, c))));
		}
	}
}