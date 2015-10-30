using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Rectangle : Parallelogram
    {
        public override double CoordinatizedArea()
        {
            return this.orderedSides[0].Length * this.orderedSides[1].Length;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            // Possible quadrilaterals.
            List<Quadrilateral> quads = null;

            if (outerShape is ConcavePolygon) quads = Quadrilateral.GetQuadrilateralsFromPoints(outerShape as ConcavePolygon, points);
            else quads = Quadrilateral.GetQuadrilateralsFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Quadrilateral quad in quads)
            {
                // Select only rectangles that don't match the outer shape.
                if (quad.VerifyRectangle() && !quad.HasSamePoints(outerShape as Polygon))
                {
                    Rectangle rect = new Rectangle(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, rect);
                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, rect.points, rect));
                        composed.Add(subSynth);
                    }
                    catch (Exception) { }

                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            //
            // Acquire a set of lengths of the given segments.
            //
            List<int> lengths = new List<int>();
            segments.ForEach(s => Utilities.AddUnique<int>(lengths, (int)s.Length));

            // Acquire the length of the rectangle so it is fixed among all appended shapes.
            // We avoid a square by looping.
            int newLength = -1;
            for (newLength = Figure.DefaultSideLength(); lengths.Contains(newLength); newLength = Figure.DefaultSideLength()) ;

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Segment seg in segments)
            {
                Rectangle rect1;
                Rectangle rect2;

                MakeRectangles(seg, newLength, out rect1, out rect2);

                FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, rect1);
                if (prob != null) composed.Add(prob);

                prob = Figure.MakeAdditionProblem(outerShape, rect2);
                if (prob != null) composed.Add(prob);
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static void MakeRectangles(Segment side, double length, out Rectangle rect1, out Rectangle rect2)
        {
            //
            // First rectangle
            //
            Segment adj1 = side.GetPerpendicularByLength(side.Point1, length);
            Segment adj2 = side.GetPerpendicularByLength(side.Point2, length);

            Segment oppSide = new Segment(adj1.OtherPoint(side.Point1), adj2.OtherPoint(side.Point2));

            rect1 = new Rectangle(side, oppSide, adj1, adj2);

            //
            // Second rectangle
            //
            Segment otherAdj1 = adj1.GetOppositeSegment(side.Point1);
            Segment otherAdj2 = adj2.GetOppositeSegment(side.Point2);

            oppSide = new Segment(otherAdj1.OtherPoint(side.Point1), otherAdj2.OtherPoint(side.Point2));

            rect2 = new Rectangle(side, oppSide, otherAdj1, otherAdj2);
        }

        public static Quadrilateral ConstructDefaultRectangle()
        {
            int length = Figure.DefaultSideLength();
            int width = Figure.DefaultSideLength();

            // Ensure we don't have a square.
            while (width == length)
            {
                length = Figure.DefaultSideLength();
                width = Figure.DefaultSideLength();
            }

            Point topLeft = PointFactory.GeneratePoint(0, width);
            Point topRight = PointFactory.GeneratePoint(length, width);
            Point bottomRight = PointFactory.GeneratePoint(length, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Rectangle(left, right, top, bottom);
        }

        //
        // Construct and return all constraints.
        //
        public override List<Constraint> GetConstraints()
        {
            List<Equation> eqs = new List<Equation>();
            List<Congruent> congs = new List<Congruent>();

            //
            // Acquire the 'midpoint' equations from the polygon class.
            //
            GetGranularMidpointEquations(out eqs, out congs);

            //
            // Create all relationship equations among the sides of the rectangle.
            //
            congs.Add(new GeometricCongruentSegments(orderedSides[0], orderedSides[2]));
            congs.Add(new GeometricCongruentSegments(orderedSides[1], orderedSides[3]));

            //
            // Create all relationship equations among the angles.
            //
            // Make simple equations where the angles are 90 degrees?
            //
            for (int a1 = 0; a1 < angles.Count - 1; a1++)
            {
                for (int a2 = a1 + 1; a2 < angles.Count; a2++)
                {
                    eqs.Add(new GeometricAngleEquation(angles[a1], angles[a2]));
                }
            }

            List<Constraint> constraints = Constraint.MakeEquationsIntoConstraints(eqs);
            constraints.AddRange(Constraint.MakeCongruencesIntoConstraints(congs));

            return constraints;
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            List<Segment> variables = new List<Segment>();

            variables.Add(orderedSides[0]);
            variables.Add(orderedSides[1]);

            return variables;
        }
    }
}
