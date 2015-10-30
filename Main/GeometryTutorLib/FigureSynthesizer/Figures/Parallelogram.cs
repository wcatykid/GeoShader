using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Parallelogram : Quadrilateral
    {
        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            // Possible quadrilaterals.
            List<Quadrilateral> quads = null;

            if (outerShape is ConcavePolygon) quads = Quadrilateral.GetQuadrilateralsFromPoints(outerShape as ConcavePolygon, points);
            else quads = Quadrilateral.GetQuadrilateralsFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Quadrilateral quad in quads)
            {
                // Select only parallelograms that don't match the outer shape.
                if (quad.VerifyParallelogram() && !quad.HasSamePoints(outerShape as Polygon))
                {
                    Parallelogram para = new Parallelogram(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, para);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, para.points, para));
                        composed.Add(subSynth);
                    }
                    catch (Exception) { }

                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }


        //
        // Append parallelograms to appropriate segments.
        //
        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            // Acquire a set of lengths of the given segments.
            List<int> lengths = new List<int>();
            segments.ForEach(s => Utilities.AddUnique<int>(lengths, (int)s.Length));

            // Acquire the length of the rectangle so it is fixed among all appended shapes.
            // We avoid a rhombus by looping.
            int newLength = -1;
            for (newLength = Figure.DefaultSideLength(); lengths.Contains(newLength); newLength = Figure.DefaultSideLength()) ;

            int angle = Figure.DefaultFirstQuadrantNonRightAngle();

            // Create the shapes.
            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Segment seg in segments)
            {
                List<Parallelogram> parallelograms = new List<Parallelogram>();

                parallelograms.AddRange(MakeParallelograms(seg, newLength, angle));

                foreach (Parallelogram para in parallelograms)
                {
                    FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, para);
                    if (prob != null) composed.Add(prob);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static List<Parallelogram> MakeParallelograms(Segment side, int length, int angle)
        {
            List<Parallelogram> parallelograms = new List<Parallelogram>();

            parallelograms.AddRange(MakeParallelograms(side, side.Point1, angle, length));
            parallelograms.AddRange(MakeParallelograms(side, side.Point2, angle, length));

            return parallelograms;
        }

        public static List<Parallelogram> MakeParallelograms(Segment side, Point vertex, int angle, int length)
        {
            List<Parallelogram> parallelograms = new List<Parallelogram>();

            //Point otherVertex = side.OtherPoint(vertex);
            //double deltaX = vertex.X - otherVertex.X;
            //double deltaY = vertex.Y - otherVertex.Y;

            //// 1
            //Segment newSide1 = side.ConstructSegmentByAngle(vertex, angle, length);
            //Point newPoint1 = newSide1.OtherPoint(vertex);

            //// Use a temporary which is a continuation of the original side.
            //Segment tempOppOriginal = side.GetOppositeSegment(otherVertex);
            //Segment newSide2 = tempOppOriginal.ConstructSegmentByAngle(otherVertex, angle, length);
            //newSide2 = new Segment(otherVertex, newSide2.OtherPoint(otherVertex)

            //Point newPoint2 = newSide2.OtherPoint(otherVertex);
            //Segment oppOriginal = new Segment(newPoint1, newPoint2);
            //parallelograms.Add(new Parallelogram(side, oppOriginal, newSide1, newSide2));

            //// 2
            //Point oppNewPoint1 = side.GetReflectionPoint(newPoint1);
            //Point oppNewPoint2 = side.GetReflectionPoint(newPoint2);

            //Segment oppSide1 = new Segment(vertex, oppNewPoint1);
            //Segment oppSide2 = new Segment(otherVertex, oppNewPoint2);
            //Segment oppOppOriginal = new Segment(oppNewPoint1, oppNewPoint2);
            //parallelograms.Add(new Parallelogram(side, oppOppOriginal, oppSide1, oppSide2));

            return parallelograms;
        }


        public static Parallelogram ConstructDefaultParallelogram()
        {
            int baseLength = Figure.DefaultSideLength();
            int height = Figure.DefaultSideLength();
            int offset = Figure.SmallIntegerValue();

            Point topLeft = PointFactory.GeneratePoint(offset, height);
            Point topRight = PointFactory.GeneratePoint(offset + baseLength, height);
            Point bottomRight = PointFactory.GeneratePoint(baseLength, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Parallelogram(left, right, top, bottom);
        }
    }
}
