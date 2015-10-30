using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 32 Classroom Problems 11-14
    //
    public class Page32ClassroomProblem11To14 : TransversalsProblem
    {
        public Page32ClassroomProblem11To14(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 32 Classroom Problems 11-14";


            Point o = new Point("O", 0, 0);   points.Add(o);
            Point m = new Point("M", -4, 3);  points.Add(m);
            Point j = new Point("J", 4, -3);   points.Add(j);
            Point h = new Point("H", -1, -4); points.Add(h);
            Point l = new Point("L", 1, 4);   points.Add(l);
            Point g = new Point("G", -4, 0);  points.Add(g);
            Point k = new Point("K", 5, 0);   points.Add(k);

            List<Point> pts = new List<Point>();
            pts.Add(m);
            pts.Add(o);
            pts.Add(j);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(l);
            pts.Add(o);
            pts.Add(h);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(k);
            pts.Add(o);
            pts.Add(g);
            collinear.Add(new Collinear(pts));

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(g, o, h)), (Angle)parser.Get(new Angle(l, o, k))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(g, o, m)), (Angle)parser.Get(new Angle(k, o, j))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(m, o, k)), (Angle)parser.Get(new Angle(g, o, j))));
            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(l, o, g)), (Angle)parser.Get(new Angle(h, o, k))));
        }
    }
}