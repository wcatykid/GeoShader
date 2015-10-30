using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 144 CE #3
    //
    public class Page144ClassroomExercise04 : CongruentTrianglesProblem
    {
        public Page144ClassroomExercise04(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 144 Classroom Ex 4";


            Point d = new Point("D", 0, 12); points.Add(d);
            Point c = new Point("C", 9, 0); points.Add(c);
            Point p = new Point("P", 9, 12); points.Add(p);
            Point e = new Point("E", 14, 12); points.Add(e);
            Point q = new Point("Q", 19, 12); points.Add(q);
            Point g = new Point("G", 19, 24); points.Add(g);
            Point f = new Point("F", 28, 12); points.Add(f);

            Segment cd = new Segment(c, d); segments.Add(cd);
            Segment cp = new Segment(c, p); segments.Add(cp);
            Segment gq = new Segment(g, q); segments.Add(gq);
            Segment fg = new Segment(f, g); segments.Add(fg);
            
            List<Point> pts = new List<Point>();
            pts.Add(c);
            pts.Add(e);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(d);
            pts.Add(p);
            pts.Add(e);
            pts.Add(q);
            pts.Add(f);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(cd, fg));
            given.Add(new GeometricCongruentSegments((Segment)parser.Get(new Segment(c, e)), (Segment)parser.Get(new Segment(e, g))));
            given.Add(new RightAngle(c, p, e));
            given.Add(new RightAngle(e, q, g));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(c, d, p)), (Angle)parser.Get(new Angle(q, f, g))));
        }
    }
}