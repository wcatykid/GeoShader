using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Quadrilateral : Polygon
    {
        //
        // Split the quadrilateral into two triangles and compute the area.
        //
        public override double CoordinatizedArea()
        {
            Point shared1 = orderedSides[0].SharedVertex(orderedSides[1]);
            Segment diagonal1 = new Segment(orderedSides[0].OtherPoint(shared1), orderedSides[1].OtherPoint(shared1));
            Triangle tri1 = new Triangle(orderedSides[0], orderedSides[1], diagonal1);

            Point shared2 = orderedSides[2].SharedVertex(orderedSides[3]);
            Segment diagonal2 = new Segment(orderedSides[2].OtherPoint(shared2), orderedSides[3].OtherPoint(shared2));
            Triangle tri2 = new Triangle(orderedSides[2], orderedSides[3], diagonal2);

            double area1 = tri1.CoordinatizedArea();
            double area2 = tri2.CoordinatizedArea();

            return area1 + area2;
        }

        //
        // Acquire the list of all possible quadrilaterals 
        //
        public static List<Quadrilateral> GetQuadrilateralsFromPoints(List<Point> points)
        {
            List<Quadrilateral> quads = new List<Quadrilateral>();

            for (int p1 = 0; p1 < points.Count - 3; p1++)
            {
                for (int p2 = p1 + 1; p2 < points.Count - 2; p2++)
                {
                    for (int p3 = p2 + 1; p3 < points.Count - 1; p3++)
                    {
                        // Disallow collinearity of the first 3 points.
                        if (!points[p1].Collinear(points[p2], points[p3]))
                        {
                            for (int p4 = p3 + 1; p4 < points.Count; p4++)
                            {
                                if (!points[p4].Collinear(points[p2], points[p1]) &&
                                    !points[p4].Collinear(points[p2], points[p3]) &&
                                    !points[p4].Collinear(points[p1], points[p3]))
                                {
                                    quads.Add(Quadrilateral.MakeQuadrilateral(points[p1], points[p2], points[p3], points[p4]));
                                }
                            }
                        }
                    }
                }
            }

            return quads;
        }

        public static List<Quadrilateral> GetQuadrilateralsFromPoints(ConcavePolygon poly, List<Point> points)
        {
            List<Quadrilateral> quads = GetQuadrilateralsFromPoints(points);
            List<Quadrilateral> containing = new List<Quadrilateral>();

            foreach (Quadrilateral quad in quads)
            {
                if (poly.Contains(quad)) containing.Add(quad);
            }

            return containing;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            throw new NotImplementedException();
        }

        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            throw new NotImplementedException();
        }

        public static Quadrilateral ConstructDefaultQuadrilateral()
        {
            return null;
        }
    }
}
