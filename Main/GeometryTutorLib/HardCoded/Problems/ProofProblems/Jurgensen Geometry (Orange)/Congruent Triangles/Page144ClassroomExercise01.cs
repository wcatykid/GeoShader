using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 144 CE #1
    //
    public class Page144ClassroomExercise01: CongruentTrianglesProblem
    {
        public Page144ClassroomExercise01(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Classroom Ex Problem 1";


            Point a = new Point("A", 2, 6); points.Add(a);
            Point s = new Point("S", 3, 0); points.Add(s);
            Point b = new Point("B", 0, 0); points.Add(b);
            Point c = new Point("C", 5, 0); points.Add(c);

            Point d = new Point("D", 8, 7); points.Add(d);
            Point t = new Point("T", 9, 1); points.Add(t);
            Point e = new Point("E", 6, 1); points.Add(e);
            Point f = new Point("F", 11, 1); points.Add(f);

            Segment ab = new Segment(a, b); segments.Add(ab);
            Segment ac = new Segment(a, c); segments.Add(ac);
            Segment a_s = new Segment(a, s); segments.Add(a_s);

            Segment de = new Segment(d, e); segments.Add(de);
            Segment df = new Segment(d, f); segments.Add(df);
            Segment dt = new Segment(d, t); segments.Add(dt); 

            List<Point> pts = new List<Point>();
            pts.Add(b);
            pts.Add(s);
            pts.Add(c);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(e);
            pts.Add(t);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(ab), (Segment)parser.Get(de)));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(ac), (Segment)parser.Get(df)));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(s, c)), (Segment)parser.Get(new Segment(t, f))));
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(b, a, c)), (Angle)parser.Get(new Angle(e, d, f))));

            goals.Add(new GeometricCongruentSegments(a_s, dt));
        }
    }
}