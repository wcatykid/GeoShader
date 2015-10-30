using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 144 CE #3
    //
    public class Page144ClassroomExercise03 : CongruentTrianglesProblem
    {
        public Page144ClassroomExercise03(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Classroom Ex 3";


            Point b = new Point("B", 0, 3); points.Add(b);
            Point l = new Point("L", 3, 5); points.Add(l);
            Point a = new Point("A", 5, 3); points.Add(a);
            Point o = new Point("O", 2, 3); points.Add(o);
            Point j = new Point("J", 3, 1); points.Add(j);

            Segment bl = new Segment(b, l); segments.Add(bl);
            Segment bj = new Segment(b, j); segments.Add(bj);
            Segment al = new Segment(a, l); segments.Add(al);
            Segment aj = new Segment(a, j); segments.Add(aj);
            Segment oj = new Segment(o, j); segments.Add(oj);
            Segment ol = new Segment(o, l); segments.Add(ol);

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(o);
            pts.Add(a);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(bl), (Segment)parser.Get(bj)));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(oj), (Segment)parser.Get(ol)));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(l, a, o)), (Angle)parser.Get(new Angle(o, a, j))));
        }
    }
}