using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Square : Rhombus
    {
        public override double CoordinatizedArea()
        {
            return Math.Pow(this.orderedSides[0].Length, 2);
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
                // Select only squares that don't match the outer shape.
                if (quad.VerifySquare() && !quad.HasSamePoints(outerShape as Polygon))
                {
                    Square square = new Square(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, square);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, square.points, square));
                        composed.Add(subSynth);
                    }
                    catch (Exception) { }

                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
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
            // Create all relationship equations among the sides of the square.
            //
            for (int s1 = 0; s1 < orderedSides.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < orderedSides.Count; s2++)
                {
                    congs.Add(new GeometricCongruentSegments(orderedSides[s1], orderedSides[s2]));
                }
            }

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

        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Segment seg in segments)
            {
                Rectangle rect1;
                Rectangle rect2;

                Rectangle.MakeRectangles(seg, seg.Length, out rect1, out rect2);

                Square sq1 = new Square(rect1);
                Square sq2 = new Square(rect2);

                FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, sq2);
                if (prob != null) composed.Add(prob);

                prob = Figure.MakeAdditionProblem(outerShape, sq2);
                if (prob != null) composed.Add(prob);
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static Square ConstructDefaultSquare()
        {
            int length = Figure.DefaultSideLength();

            Point topLeft = PointFactory.GeneratePoint(0, length);
            Point topRight = PointFactory.GeneratePoint(length, length);
            Point bottomRight = PointFactory.GeneratePoint(length, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Square(left, right, top, bottom);
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            List<Segment> variables = new List<Segment>();

            variables.Add(orderedSides[0]);
            
            return variables;
        }
    }
}
