using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;


namespace GeometryTutorLib.GeometryTestbed
{
    class Page171Problem17 : QuadrilateralsProblem
    {
        // Geometry; Page 171 Problem 17
        // Demonstrates: "If an angle of a parallelogram is a right angle, then the parallelogram is a rectangle." (Jurgensen 4-13)

        /*   Z ____________ Y
              |            |
              |            |
           U__|____________|
              |             X
              V
         */

        public Page171Problem17(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 171 Problem 17";

            Point u = new Point("U", -3, 0); points.Add(u);
            Point v = new Point("V", 0, -3); points.Add(v);
            Point w = new Point("W", 0, 0); points.Add(w);
            Point x = new Point("X", 10, 0); points.Add(x);
            Point y = new Point("Y", 10, 5); points.Add(y);
            Point z = new Point("Z", 0, 5); points.Add(z);

            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);

            List<Point> pnts = new List<Point>();
            pnts.Add(u); 
            pnts.Add(w); 
            pnts.Add(x);
            collinear.Add(new Collinear(pnts));

            List<Point> pnts2 = new List<Point>();
            pnts2.Add(v); 
            pnts2.Add(w); 
            pnts2.Add(z);
            collinear.Add(new Collinear(pnts2));

            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral((Segment)parser.Get(new Segment(w, z)),
                xy, yz, (Segment)parser.Get(new Segment(w, x))));
            Angle angle = (Angle)parser.Get(new Angle(u, w, v));

            given.Add(new Strengthened(quad, new Parallelogram(quad)));
            given.Add(new Strengthened(angle, new RightAngle(angle)));

            goals.Add(new Strengthened(quad, new Rectangle(quad)));
        }
    }
}
