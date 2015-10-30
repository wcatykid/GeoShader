using System;
using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTutorLib.GeometryTestbed
{
    //
    // Geometry; Page 301 problem 50
    //
    public class Page301Problem50 : CongruentTrianglesProblem
    {
        public Page301Problem50(bool onoff, bool complete) : base(onoff, complete)
        {
            problemName = "Page 301 Problem 50";


            Point w = new Point("W", 5, 0); points.Add(w);
            Point x = new Point("X", 0, 5.0 * Math.Sqrt(3.0)); points.Add(x);
            Point y = new Point("Y", 10, 5.0 * Math.Sqrt(3.0)); points.Add(y);
            Point z = new Point("Z", 15, 0); points.Add(z);

            Segment wx = new Segment(w, x); segments.Add(wx);
            Segment wy = new Segment(w, y); segments.Add(wy);
            Segment wz = new Segment(w, z); segments.Add(wz);
            Segment xy = new Segment(x, y); segments.Add(xy);
            Segment yz = new Segment(y, z); segments.Add(yz);

                        parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);
            
            given.Add(new GeometricCongruentSegments(wx, wz));
            given.Add(new GeometricCongruentSegments(wx, yz));
            given.Add(new GeometricCongruentSegments(wx, xy));

            goals.Add(new GeometricCongruentTriangles(new Triangle(w, x, y), new Triangle(w, y, z)));
        }
    }
}