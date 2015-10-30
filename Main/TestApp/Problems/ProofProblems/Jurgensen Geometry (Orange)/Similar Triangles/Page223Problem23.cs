using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
	//
	// Geometry; Page 223 Problem 23
	//
	public class Page223Problem23 : SimilarTrianglesProblem
	{
        public Page223Problem23(bool onoff, bool complete) : base(onoff, complete)
		{
            problemName = "Page 223 Problem 23";


            Point j = new Point("J", 0, 0); points.Add(j);
			Point i = new Point("I", 3, 3); points.Add(i);
			Point y = new Point("Y", 7, 7); points.Add(y);
			Point g = new Point("G", 6, 0); points.Add(g);
			Point z = new Point("Z", 7, 0); points.Add(z);

			Segment ig = new Segment(i, g); segments.Add(ig);
            Segment yz = new Segment(y, z); segments.Add(yz);

			List<Point> pts = new List<Point>();
			pts.Add(j);
			pts.Add(i);
			pts.Add(y);
			collinear.Add(new Collinear(pts));

			pts = new List<Point>();
			pts.Add(j);
			pts.Add(g);
			pts.Add(z);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(j, g, i)), (Angle)parser.Get(new Angle(j, y, z))));

            goals.Add(new GeometricSimilarTriangles((Triangle)parser.Get(new Triangle(j, i, g)), (Triangle)parser.Get(new Triangle(j, z, y))));
		}
	}
}