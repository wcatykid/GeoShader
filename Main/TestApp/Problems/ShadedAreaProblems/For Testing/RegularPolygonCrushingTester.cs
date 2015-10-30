using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.Precomputer;

namespace GeometryTestbed
{
    public class RegularPolygonCrushingTester : ActualShadedAreaProblem
    {
        public RegularPolygonCrushingTester(bool onoff, bool complete) : base(onoff, complete)
        {
            Point p00 = new Point("P00", 0, 0); points.Add(p00);
            Point p01 = new Point("P01", 0, 1); points.Add(p01);
            Point p02 = new Point("P02", 0, 2); points.Add(p02);
            Point p03 = new Point("P03", 0, 3); points.Add(p03);
            Point p04 = new Point("P04", 0, 4); points.Add(p04);

            Point p10 = new Point("P10", 1, 0); points.Add(p10);
            Point p11 = new Point("P11", 1, 1); points.Add(p11);
            Point p12 = new Point("P12", 1, 2); points.Add(p12);
            Point p13 = new Point("P13", 1, 3); points.Add(p13);
            Point p14 = new Point("P14", 1, 4); points.Add(p14);

            Point p20 = new Point("P20", 2, 0); points.Add(p20);
            Point p21 = new Point("P21", 2, 1); points.Add(p21);
            Point p22 = new Point("P22", 2, 2); points.Add(p22);
            Point p23 = new Point("P23", 2, 3); points.Add(p23);
            Point p24 = new Point("P24", 2, 4); points.Add(p24);

            Point p30 = new Point("P30", 3, 0); points.Add(p30);
            Point p31 = new Point("P31", 3, 1); points.Add(p31);
            Point p32 = new Point("P32", 3, 2); points.Add(p32);
            Point p33 = new Point("P33", 3, 3); points.Add(p33);
            Point p34 = new Point("P34", 3, 4); points.Add(p34);

            List<Point> pts = new List<Point>();
            pts.Add(p00);
            pts.Add(p01);
            pts.Add(p02);
            pts.Add(p03);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p10);
            pts.Add(p11);
            pts.Add(p12);
            pts.Add(p13);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p20);
            pts.Add(p21);
            pts.Add(p22);
            pts.Add(p23);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p30);
            pts.Add(p31);
            pts.Add(p32);
            pts.Add(p33);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p00);
            pts.Add(p10);
            pts.Add(p20);
            pts.Add(p30);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p01);
            pts.Add(p11);
            pts.Add(p21);
            pts.Add(p31);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p02);
            pts.Add(p12);
            pts.Add(p22);
            pts.Add(p32);
            collinear.Add(new Collinear(pts));

            pts = new List<Point>();
            pts.Add(p03);
            pts.Add(p13);
            pts.Add(p23);
            pts.Add(p33);
            collinear.Add(new Collinear(pts));

            parser = new LiveGeometry.TutorParser.HardCodedParserMain(points, collinear, segments, circles, onoff);

            // The goal is the entire area of the figure.
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>(parser.implied.atomicRegions);
        }
    }
}