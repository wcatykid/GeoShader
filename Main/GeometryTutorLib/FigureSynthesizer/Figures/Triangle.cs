using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a triangle, which consists of 3 segments
    /// </summary>
    public partial class Triangle : Polygon
    {
        public override double CoordinatizedArea()
        {
            return HeroArea(SegmentA.Length, SegmentB.Length, SegmentC.Length);
        }

        //
        // Acquire the list of all possible triangles.
        //
        public static List<Triangle> GetTrianglesFromPoints(List<Point> points)
        {
            List<Triangle> tris = new List<Triangle>();

            for (int p1 = 0; p1 < points.Count - 2; p1++)
            {
                for (int p2 = p1 + 1; p2 < points.Count - 1; p2++)
                {
                    for (int p3 = p2 + 1; p3 < points.Count; p3++)
                    {
                        if (!points[p1].Collinear(points[p2], points[p3]))
                        {
                            tris.Add(new Triangle(points[p1], points[p2], points[p3]));
                        }
                    }
                }
            }

            return tris;
        }

        public static List<Triangle> GetTrianglesFromPoints(ConcavePolygon poly, List<Point> points)
        {
            List<Triangle> tris = GetTrianglesFromPoints(points);
            List<Triangle> containing = new List<Triangle>();

            foreach (Triangle tri in tris)
            {
                if (poly.Contains(tri)) containing.Add(tri);
            }

            return containing;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<FigSynthProblem> composed = new List<FigSynthProblem>();

            // Possible triangles.
            List<Triangle> tris = null;

            if (outerShape is ConcavePolygon) tris = Triangle.GetTrianglesFromPoints(outerShape as ConcavePolygon, points);
            else tris = Triangle.GetTrianglesFromPoints(points);

            // Check all triangles to determine applicability.
            foreach (Triangle tri in tris)
            {
                // Avoid equilateral, isosceles, and right triangles.
                if (!tri.IsEquilateral() && !tri.IsIsosceles() && !tri.isRightTriangle() && !tri.StructurallyEquals(outerShape))
                {
                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, tri);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, tri.points, tri));
                        composed.Add(subSynth);
                    }
                    catch (Exception) { }
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            List<FigSynthProblem> composed = new List<FigSynthProblem>();

            int length = Figure.DefaultSideLength();
            int angle = Figure.DefaultFirstQuadrantNonRightAngle();
            foreach (Segment seg in segments)
            {
                List<Triangle> tris;

                MakeTriangles(seg, length, angle, out tris);

                foreach (Triangle t in tris)
                {
                    FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, t);
                    if (prob != null) composed.Add(prob);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static void MakeTriangles(Segment side, int length, int angle, out List<Triangle> tris)
        {
            tris = new List<Triangle>();

            // 1
            Segment newSide1 = side.ConstructSegmentByAngle(side.Point1, angle, length);
            Point newPoint1 = newSide1.OtherPoint(side.Point1);
            tris.Add(new Triangle(side.Point1, side.Point2, newPoint1));

            // 2
            Segment newSide2 = side.ConstructSegmentByAngle(side.Point2, angle, length);
            Point newPoint2 = newSide2.OtherPoint(side.Point2);
            tris.Add(new Triangle(side.Point1, side.Point2, newPoint2));

            // 3
            Point oppNewPoint1 = side.GetReflectionPoint(newPoint1);
            tris.Add(new Triangle(side.Point1, side.Point2, oppNewPoint1));

            // 4
            Point oppNewPoint2 = side.GetReflectionPoint(newPoint1);
            tris.Add(new Triangle(side.Point1, side.Point2, oppNewPoint2));
        }

        public static Triangle ConstructDefaultTriangle()
        {
            Point other1 = PointFactory.GeneratePoint(Figure.GenerateIntValue(), 0);
            Point other2 = PointFactory.GeneratePoint(Figure.GenerateIntValue(), Figure.GenerateIntValue());

            return new Triangle(origin, other1, other2);
        }

        // By default return all the sides since we can use Hero's formula then.
        public override List<Segment> GetAreaVariables()
        {
            return this.orderedSides;
        }
    }
}
