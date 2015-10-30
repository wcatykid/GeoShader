using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 13 Problem 10
    //
    public class Page13Problem10 : TransversalsProblem
    {
        public Page13Problem10(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 13 Problem 10";


            Point h = new Point("H", 4, 0); points.Add(h);
            Point i = new Point("I", 0, 5); points.Add(i);
            Point j = new Point("J", -4, 0); points.Add(j);
            Point k = new Point("K", 0, 0); points.Add(k);

            Segment ik = new Segment(i, k); segments.Add(ik);

            List<Point> pts = new List<Point>();
            pts.Add(h);
            pts.Add(k);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new AngleBisector(new Angle(h, k, j), ik));

            goals.Add(new Strengthened((Angle)parser.Get(new Angle(i, k, j)), new RightAngle((Angle)parser.Get(new Angle(i, k, j)))));
        }
    }
}