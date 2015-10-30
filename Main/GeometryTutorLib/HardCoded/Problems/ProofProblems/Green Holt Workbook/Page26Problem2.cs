using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 26 Problem 2
    //
    public class Page26Problem2 : CongruentTrianglesProblem
    {
        public Page26Problem2(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 26 Problem 2";


            Point l = new Point("L", 0, 0); points.Add(l);
            Point m = new Point("M", 4, 1); points.Add(m);
            Point k = new Point("K", 1, 6); points.Add(k);
            Point j = new Point("J", 5, 7); points.Add(j);

            Segment kj = new Segment(k, j); segments.Add(kj);
            Segment kl = new Segment(k, l); segments.Add(kl);
            Segment lm = new Segment(l, m); segments.Add(lm);
            Segment mj = new Segment(m, j); segments.Add(mj);
            Segment km = new Segment(k, m); segments.Add(km);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(k, l, m)), (Angle)parser.Get(new Angle(k, j, m))));
            given.Add(new GeometricParallel(kj, lm));

            goals.Add(new GeometricCongruentAngles((Angle)parser.Get(new Angle(l, k, m)), (Angle)parser.Get(new Angle(j, m, k))));
        }
    }
}