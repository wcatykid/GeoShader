using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class IsoscelesTrapezoid : Trapezoid
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
                // Select only isosceles trapezoids that don't match the outer shape.
                if (quad.VerifyIsoscelesTrapezoid() && !quad.HasSamePoints(outerShape as Polygon))
                {
                    IsoscelesTrapezoid isoTrap = new IsoscelesTrapezoid(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, isoTrap);
                    subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, isoTrap.points, isoTrap));

                    composed.Add(subSynth);
                }
            }

            return FigSynthProblem.RemoveSymmetric(composed);
        }

        public new static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            throw new NotImplementedException();
        }

        public static IsoscelesTrapezoid ConstructDefaultIsoscelesTrapezoid()
        {
            int base1 = Figure.DefaultSideLength();
            int side = Figure.DefaultSideLength();

            // Ensure a smaller side then base for a 'normal' look.
            while (base1 <= side)
            {
                base1 = Figure.DefaultSideLength();
                side = Figure.DefaultSideLength();
            }

            int baseAngle = Figure.DefaultFirstQuadrantNonRightAngle();
            Point topLeft = Figure.GetPointByLengthAndAngleInStandardPosition(side, baseAngle);
            topLeft = PointFactory.GeneratePoint(topLeft.X, topLeft.Y);

            Point topRight = Figure.GetPointByLengthAndAngleInThirdQuadrant(side, baseAngle);
            topRight = PointFactory.GeneratePoint(base1 + topRight.X, topRight.Y);

            Point bottomRight = PointFactory.GeneratePoint(base1, 0);

            Segment left = new Segment(origin, topLeft);
            Segment top = new Segment(topLeft, topRight);
            Segment right = new Segment(topRight, bottomRight);
            Segment bottom = new Segment(bottomRight, origin);

            return new IsoscelesTrapezoid(left, right, top, bottom);
        }

        //
        // The height of this trapezoid (ONLY used from known-acquisition with areas).
        //
        private double calculatedHeight = -1;

        //
        // Acquire the height of the isosceles trapezoid: h = Sqrt(c^2 - 1/4 (b - a)^2)
        //
        public void CalculateHeight(Area_Based_Analyses.KnownMeasurementsAggregator known)
        {
            // Check if the height has already been calculated.
            if (calculatedHeight > 0) return;

            // Calculate the height.
            double base1Length = known.GetSegmentLength(this.baseSegment);
            double base2Length = known.GetSegmentLength(this.oppBaseSegment);

            double isoSideLength = known.GetSegmentLength(this.leftLeg);
            if (isoSideLength < 0) isoSideLength = known.GetSegmentLength(this.rightLeg);

            if (base1Length < 0 || base2Length < 0 || isoSideLength < 0) return;

            this.calculatedHeight = Math.Sqrt(isoSideLength * isoSideLength - Math.Pow(base1Length - base2Length, 2) / 4.0);
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables()
        {
            throw new NotImplementedException();
        }
    }
}
