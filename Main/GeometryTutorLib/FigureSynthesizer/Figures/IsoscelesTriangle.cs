using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class IsoscelesTriangle : Triangle
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
                // Select only isosceles triangle that don't match the outer shape.
                if (tri.IsIsosceles() && !tri.StructurallyEquals(outerShape))
                {
                    IsoscelesTriangle isoTri = new IsoscelesTriangle(tri);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, isoTri);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, isoTri.points, isoTri));
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

            // Acquire a set of lengths of the given segments.
            List<int> lengths = new List<int>();
            segments.ForEach(s => Utilities.AddUnique<int>(lengths, (int)s.Length));

            //
            // Acquire the length of the isosceles triangle so that it is longer than the half the distance of the segment.
            //
            int newLength = -1;
            for (newLength = Figure.DefaultSideLength(); ; newLength = Figure.DefaultSideLength())
            {
                bool longEnough = true;
                foreach (Segment side in segments)
                {
                    if (newLength < (side.Length / 2.0) + 1)
                    {
                        longEnough = false;
                        break;
                    }
                }
                if (longEnough) break;
            }
            
            //
            // Construct the triangles.
            //
            foreach (Segment seg in segments)
            {
                List<Triangle> tris;

                MakeIsoscelesTriangles(seg, newLength, out tris);

                foreach (Triangle t in tris)
                {
                    FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, t);
                    if (prob != null) composed.Add(prob);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static void MakeIsoscelesTriangles(Segment side, double length, out List<Triangle> tris)
        {
            tris = new List<Triangle>();

            // Define two circles or radius 'length'
            // One intersection defines a triangle 'above' the segment; the other 'below' the segment.
            Circle circle1 = new Circle(side.Point1, length);
            Circle circle2 = new Circle(side.Point2, length);

            Point inter1, inter2;
            circle1.FindIntersection(circle2, out inter1, out inter2);

            if (inter1 == null || inter2 == null)
            {
                throw new Exception("Circles do not have large enough radius.");
            }

            // 1
            tris.Add(new IsoscelesTriangle(new Triangle(side.Point1, side.Point2, inter1)));

            // 2
            tris.Add(new IsoscelesTriangle(new Triangle(side.Point1, side.Point2, inter2)));
        }

        public static IsoscelesTriangle ConstructDefaultIsoscelesTriangle()
        {
            int baseLength = Figure.DefaultSideLength();
            int height = Figure.DefaultSideLength();

            Point top = PointFactory.GeneratePoint(baseLength / 2.0, height);
            Point bottomRight = PointFactory.GeneratePoint(baseLength, 0);

            Segment left = new Segment(origin, top);
            Segment right = new Segment(top, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new IsoscelesTriangle(left, right, bottom);
        }
    }
}
