using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
	//
	// Geometry; Page 146 Problem 15
	//
	public class Page146Problem15 : CongruentTrianglesProblem
	{
        public Page146Problem15(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 146 Problem 15";


			Point t = new Point("T", 0, 0); points.Add(t);
			Point p = new Point("P", 0.5, 1.5); points.Add(p);
			Point s = new Point("S", 1, 3); points.Add(s);
			Point x = new Point("X", 3, 1); points.Add(x);
			Point q = new Point("Q", 3.5, 2.5); points.Add(q);
			Point y = new Point("Y", 4, 4); points.Add(y);
			Point o = new Point("O", 2, 2); points.Add(o);

			List<Point> pts = new List<Point>();
			pts.Add(t);
			pts.Add(o);
			pts.Add(y);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(s);
			pts.Add(o);
			pts.Add(x);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(s);
			pts.Add(p);
			pts.Add(t);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(y);
			pts.Add(q);
			pts.Add(x);
            collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(p);
			pts.Add(o);
			pts.Add(q);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new SegmentBisector(parser.GetIntersection(new Segment(s, x), new Segment(t, y)), (Segment)parser.Get(new Segment(s, x))));
            given.Add(new SegmentBisector(parser.GetIntersection(new Segment(s, x), new Segment(t, y)), (Segment)parser.Get(new Segment(t, y))));

            InMiddle goalIm = (InMiddle)parser.Get(new InMiddle( o, new Segment(p, q)));
            goals.Add(new Strengthened(goalIm, new Midpoint(goalIm)));
		}
	}
}