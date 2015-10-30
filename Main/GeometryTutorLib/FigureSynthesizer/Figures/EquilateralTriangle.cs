using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class EquilateralTriangle : IsoscelesTriangle
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            // Possible triangles.
            List<Triangle> tris = null;

            if (outerShape is ConcavePolygon) tris = Triangle.GetTrianglesFromPoints(outerShape as ConcavePolygon, points);
            else tris = Triangle.GetTrianglesFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Triangle tri in tris)
            {
                // Select only parallelograms that don't match the outer shape.
                if (tri.IsEquilateral() && !tri.StructurallyEquals(outerShape))
                {
                    EquilateralTriangle eqTri = new EquilateralTriangle(tri);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, eqTri);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, eqTri.points, eqTri));
                        composed.Add(subSynth);
                    }
                    catch (Exception) { }

                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        //
        // With appending in this case, we choose the given segment to be the base.
        //
        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            List<FigSynthProblem> composed = new List<FigSynthProblem>();

            //
            // Construct the triangles.
            //
            foreach (Segment seg in segments)
            {
                List<Triangle> tris;

                IsoscelesTriangle.MakeIsoscelesTriangles(seg, seg.Length, out tris);

                foreach (Triangle t in tris)
                {
                    FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, t);
                    if (prob != null) composed.Add(prob);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static EquilateralTriangle ConstructDefaultEquilateralTriangle()
        {
            int sideLength = Figure.DefaultSideLength();

            Point top = Figure.GetPointByLengthAndAngleInStandardPosition(sideLength, 60);
            top = PointFactory.GeneratePoint(top.X, top.Y);

            Point bottomRight = PointFactory.GeneratePoint(sideLength, 0);

            Segment left = new Segment(origin, top);
            Segment right = new Segment(top, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new EquilateralTriangle(left, right, bottom);
        }
    }
}
