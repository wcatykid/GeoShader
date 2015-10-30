using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 147 Problem 21
	//
	public class Page147Problem21 : CongruentTrianglesProblem
	{
        public Page147Problem21(bool onoff, bool complete) : base(onoff, complete)
		{
			Point d = new Point("D", 6, 6); points.Add(d);
			Point b = new Point("B", 9, 0); points.Add(b);
			Point a = new Point("A", 3, 0); points.Add(a);
			Point c = new Point("C", 11, 4); points.Add(c);
			Point e = new Point("E", 1, 4); points.Add(e);

			Segment ab = new Segment(a, b); segments.Add(ab);
			Segment ac = new Segment(a, c); segments.Add(ac);
			Segment ad = new Segment(a, d); segments.Add(ad);
			Segment ae = new Segment(a, e); segments.Add(ae);
			Segment bc = new Segment(b, c); segments.Add(bc);
			Segment bd = new Segment(b, d); segments.Add(bd);
			Segment be = new Segment(b, e); segments.Add(be);
			Segment cd = new Segment(c, d); segments.Add(cd);
			Segment ce = new Segment(c, e); segments.Add(ce);
			Segment de = new Segment(d, e); segments.Add(de);
            
                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(ae), (Segment)parser.Get(bc)));
			given.Add(new GeometricCongruentSegments((Segment)parser.Get(ad), (Segment)parser.Get(bd)));
		}
	}
}