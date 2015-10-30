using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Rhombus : Parallelogram
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
                // Select only rhombi that don't match the outer shape.
                if (quad.VerifyRhombus() && !quad.HasSamePoints(outerShape as Polygon))
                {
                    Rhombus rhombus = new Rhombus(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, rhombus);

                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, rhombus.points, rhombus));
                        composed.Add(subSynth);
                    }
                    catch (Exception) { }

                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            throw new NotImplementedException();
        }

        public static Rhombus ConstructDefaultRhombus()
        {
            int sideLength = Figure.DefaultSideLength();
            int baseAngle = Figure.DefaultFirstQuadrantNonRightAngle();

            Point topLeft = Figure.GetPointByLengthAndAngleInStandardPosition(sideLength, baseAngle);
            double offsetX = topLeft.X;
            double offsetY = topLeft.Y;

            topLeft = PointFactory.GeneratePoint(topLeft.X, topLeft.Y);
            Point topRight = PointFactory.GeneratePoint(offsetX + sideLength, offsetY);
            Point bottomRight = PointFactory.GeneratePoint(sideLength, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new Rhombus(left, right, top, bottom);
        }

        //
        // Construct and return all constraints.
        //
        public override List<Constraint> GetConstraints()
        {
            throw new NotImplementedException();
/*
            List<Equation> eqs = new List<Equation>();

            //
            // Acquire the 'midpoint' equations from the polygon class.
            //

            eqs.AddRange(GetGranularMidpointEquations());

            //
            // Create all relationship equations among the sides of the rhombus.
            //
            for (int s1 = 0; s1 < orderedSides.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < orderedSides.Count; s2++)
                {
                    eqs.Add(new GeometricSegmentEquation(orderedSides[s1], orderedSides[s2]));
                }
            }

            //
            // Create all relationship equations among the angles.
            //
            // Make simple equations where the angles are 90 degrees?
            //
            eqs.Add(new GeometricAngleEquation(angles[0], angles[2]));
            eqs.Add(new GeometricAngleEquation(angles[1], angles[3]));

            return Constraint.MakeEquationsIntoConstraints(eqs);
*/
        }
    }
}
