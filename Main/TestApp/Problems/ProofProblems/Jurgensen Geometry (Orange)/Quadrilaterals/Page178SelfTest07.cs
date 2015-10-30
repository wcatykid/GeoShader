using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    class Page178SelfTest07 : QuadrilateralsProblem
    {
        // Geometry; Page 178 Self Test 07
        // CURRENTLY NOT WORKING
        // Given that a quad is a parallelogram with congruent diagonals, it should be possible to prove that the quad is also a rectangle
        // Right now, the missing step is the ability to prove that two angles are right angles if they are both supplementary and congruent
        // Once an angle is proved to be a right angle, the rectangle definiton instantiator can prove a parallelogram is a rectangle

        public Page178SelfTest07(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 178 Self Test Problem 7";

            Point w = new Point("W", 0, 0); points.Add(w);
            Point x = new Point("X", 7, 0); points.Add(x);
            Point y = new Point("Y", 7, 5); points.Add(y);
            Point z = new Point("Z", 0, 5); points.Add(z);
            Point q = new Point("Q", 3.5, 2.5); points.Add(q);

            //rectangle sides
            Segment wx = new Segment(w, x); segments.Add(wx);
            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);
            Segment zw = new Segment(z, w); segments.Add(zw);

            List<Point> pts = new List<Point>();
            pts.Add(z);
            pts.Add(q);
            pts.Add(x);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(w);
            pts.Add(q);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

            //rectangle diagonals
//            Segment wy = new Segment(w, y); segments.Add(wy);
//            Segment xz = new Segment(x, z); segments.Add(xz);

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            given.Add(new GeometricCongruentSegments(wx, yz));
            given.Add(new GeometricCongruentSegments(xy, zw));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(w, y)), (Segment)parser.Get(new Segment(x, z))));
            
            Quadrilateral quad = (Quadrilateral)parser.Get(new Quadrilateral(zw, xy, yz, wx));
            goals.Add(new Strengthened(quad, new Rectangle(quad)));
        }
    }
}
