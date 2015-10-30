using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    //
    // Geometry; Page 144 CE #2
    //
    public class Page144ClassroomExercise02 : CongruentTrianglesProblem
    {
        public Page144ClassroomExercise02(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Classroom Ex 2";


            Point p = new Point("P", 0, 6); points.Add(p);
            Point x = new Point("X", 1, 4); points.Add(x);
            Point l = new Point("L", 2, 2); points.Add(l);

            Point a = new Point("A", 6, 4); points.Add(a);

            Point n = new Point("N", 10, 6); points.Add(n);
            Point y = new Point("Y", 11, 4); points.Add(y);
            Point k = new Point("K", 12, 2); points.Add(k);

            List<Point> pts = new List<Point>();
            pts.Add(p);
            pts.Add(x);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(y);
            pts.Add(k);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p);
            pts.Add(a);
            pts.Add(k);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(x);
            pts.Add(a);
            pts.Add(y);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(n);
            pts.Add(a);
            pts.Add(l);
            collinear.Add(new Collinear(pts));

                        parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(l, a)), (Segment)parser.Get(new Segment(a, n))));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(p, a)), (Segment)parser.Get(new Segment(a, k))));

            goals.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(a, x)), (Segment)parser.Get(new Segment(a, y))));
        }
    }
}