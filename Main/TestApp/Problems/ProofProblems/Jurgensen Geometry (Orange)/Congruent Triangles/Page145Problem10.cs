using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 145 Problem 10
	//
	public class Page145Problem10 : CongruentTrianglesProblem
	{
        public Page145Problem10(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 145 Problem 10";


            Point a = new Point("A", 0, 5); points.Add(a);
			Point b = new Point("B", 1, 6); points.Add(b);
			Point c = new Point("C", 5, 7); points.Add(c);
			Point d = new Point("D", 6, 5); points.Add(d);
			Point e = new Point("E", 5, 3); points.Add(e);
			Point f = new Point("F", 1, 4); points.Add(f);

			Segment ab = new Segment(a, b); segments.Add(ab);
			Segment bc = new Segment(b, c); segments.Add(bc);
			Segment cd = new Segment(c, d); segments.Add(cd);
			Segment de = new Segment(d, e); segments.Add(de);
			Segment ef = new Segment(e, f); segments.Add(ef);
			Segment af = new Segment(a, f); segments.Add(af);
            Segment ad = new Segment(a, d); segments.Add(ad);

            // Constructed
            Segment ae = new Segment(a, e); segments.Add(ae);
            Segment ac = new Segment(a, c); segments.Add(ac);

			            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(ab, af));
			given.Add(new GeometricCongruentSegments(cd, de));
			given.Add(new GeometricCongruentSegments(bc, ef));
  			given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(e, d, a)), (Angle)parser.Get(new Angle(c, d, a))));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(a, b, c)), (Angle)parser.Get(new Angle(a, f, e))));
        }
	}
}