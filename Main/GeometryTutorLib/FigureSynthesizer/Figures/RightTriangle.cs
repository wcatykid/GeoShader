using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class RightTriangle : Triangle
    {
        public override double CoordinatizedArea()
        {
            Segment hyp = this.GetHypotenuse();
            Segment other1, other2;            
            GetOtherSides(hyp, out other1, out other2);

            return other1.Length * other2.Length / 2.0;
        }

        public new static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<Triangle> tris = Triangle.GetTrianglesFromPoints(points);

            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            foreach (Triangle tri in tris)
            {
                // Only create right triangles that are NOT the outershape.
                if (tri.isRightTriangle() && !tri.StructurallyEquals(outerShape))
                {
                    RightTriangle rTri = new RightTriangle(tri);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, rTri);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, rTri.points, rTri));
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

            // Acquire a set of lengths of the given segments.
            //
            List<int> lengths = new List<int>();
            segments.ForEach(s => Utilities.AddUnique<int>(lengths, (int)s.Length));

            // Acquire an isosceles triangle by looping.
            int newLength = -1;
            for (newLength = Figure.DefaultSideLength(); lengths.Contains(newLength); newLength = Figure.DefaultSideLength()) ;

            foreach (Segment seg in segments)
            {
                List<RightTriangle> tris;

                MakeRightTriangles(seg, newLength, out tris);

                foreach (RightTriangle rt in tris)
                {
                    FigSynthProblem prob = Figure.MakeAdditionProblem(outerShape, rt);
                    if (prob != null) composed.Add(prob);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public static void MakeRightTriangles(Segment side, int length, out List<RightTriangle> tris)
        {
            tris = new List<RightTriangle>();

            //
            // 1
            //
            Segment perp = side.GetPerpendicularByLength(side.Point1, length);
            tris.Add(new RightTriangle(new Triangle(side.Point1, side.Point2, perp.OtherPoint(side.Point1))));

            // 2
            Segment oppPerp = perp.GetOppositeSegment(side.Point1);
            tris.Add(new RightTriangle(new Triangle(side.Point1, side.Point2, oppPerp.OtherPoint(side.Point1))));
            
            // 3
            perp = side.GetPerpendicularByLength(side.Point2, length);
            tris.Add(new RightTriangle(new Triangle(side.Point1, side.Point2, perp.OtherPoint(side.Point2))));

            // 4
            oppPerp = perp.GetOppositeSegment(side.Point2);
            tris.Add(new RightTriangle(new Triangle(side.Point1, side.Point2, oppPerp.OtherPoint(side.Point2))));
        }

        public static RightTriangle ConstructDefaultRightTriangle()
        {
            int length = Figure.DefaultSideLength();
            int width = Figure.DefaultSideLength();

            Point topLeft = PointFactory.GeneratePoint(0, width);
            Point bottomRight = PointFactory.GeneratePoint(length, 0);

            Segment left = new Segment(origin, topLeft);
            Segment hypotenuse = new Segment(topLeft, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new RightTriangle(left, bottom, hypotenuse);
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
            // Create all relationship equations among the angles.
            //
            // Make simple equations where the angles are 90 degrees?
            //
            eqs.Add(new GeometricAngleEquation(this.rightAngle, new NumericValue(90)));

            List<Constraint> constraints = Constraint.MakeEquationsIntoConstraints(eqs);
            constraints.AddRange(Constraint.MakeCongruencesIntoConstraints(congs));

            constraints.Add(new FigureConstraint(this));

            return constraints;
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            Segment other1, other2;

            this.GetOtherSides(this.GetHypotenuse(), out other1, out other2);

            List<Segment> variables = new List<Segment>();
            variables.Add(other1);
            variables.Add(other2);

            return variables;
        }
    }
}
