using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Kite : Quadrilateral
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
                // Select only kites that don't match the outer shape.
                if (quad.VerifyKite() && !quad.HasSamePoints(outerShape as Polygon))
                {
                    Kite kite = new Kite(quad);

                    SubtractionSynth subSynth = new SubtractionSynth(outerShape, kite);
                    try
                    {
                        subSynth.SetOpenRegions(FigSynthProblem.AcquireOpenAtomicRegions(conns, kite.points, kite));
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

        public static Kite ConstructDefaultKite()
        {
            int height = Figure.DefaultSideLength() / 2;
            int width1 = Figure.DefaultSideLength() / 2;
            int width2 = Figure.DefaultSideLength() / 2;

            Point top = PointFactory.GeneratePoint(width1, height);
            Point right = PointFactory.GeneratePoint(width1 + width2, 0);
            Point bottom = PointFactory.GeneratePoint(width1, - height);

            Segment leftTop = new Segment(origin, top);
            Segment rightTop = new Segment(top, right);
            Segment rightBottom = new Segment(right, bottom);
            Segment leftBottom = new Segment(bottom, origin);

            return new Kite(leftTop, rightBottom, rightTop, leftBottom);
        }

        public override List<Segment> GetAreaVariables()
        {
            List<Segment> vars = new List<Segment>();

            vars.Add(bottomLeftTopRightDiagonal);
            vars.Add(topLeftBottomRightDiagonal);

            return vars;
        }
    }
}
